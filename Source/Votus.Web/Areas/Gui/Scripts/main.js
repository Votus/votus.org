var api            = new VotusApi();
var ideasViewModel = new IdeasViewModel();

// Initializes the KnockoutJS framework to handle UI plumbing
function initializeKnockout() {
    ko.applyBindings(ideasViewModel, document.getElementById('IdeasDisplay'));
}

// Defines the model for the Ideas view
function
IdeasViewModel() {
    var self            = this;
    self.etag           = '*';
    self.ideas          = ko.observableArray();
    self.nextPageToken  = null;
    self.loadNextButton = $('#LoadNextIdeasButton');

    self.submitNewIdea  = function (formElement) {
        var form = $(formElement);
        if (!form.valid()) return;

        var createIdeaCommand = {};
        var newIdeaId         = $('#NewIdeaId').val();

        // Populate the command with the form values
        form
            .serializeArray()
            .map(function (formInput) {
                createIdeaCommand[formInput.name] = formInput.value;
            });

        // Convert the command to a view model so it can be added to the view
        var ideaViewModel = ConvertCreateIdeaCommandToIdeaViewModel(
            createIdeaCommand
        );

        // Add the new idea to the ideas list and begin trying to submit it
        self.ideas.unshift(ideaViewModel);

        var requestStatusElement = $('#' + ideaViewModel.Id() + ' .RequestStatus');

        requestStatusElement.slideDown();

        // Try to submit the command to the API
        api.commands.send(
            newIdeaId,
            'CreateIdeaCommand',
            createIdeaCommand,
            function () {
                var savingStatus = requestStatusElement.find('.Saving');
                var savedStatus  = requestStatusElement.find('.Saved');

                savingStatus.fadeOut();
                savedStatus.fadeIn();

                setTimeout(
                    function () {
                        savedStatus.fadeOut();
                        requestStatusElement.slideUp();
                    },
                    4000
                );
            }
        );

        // Reset the form state for a new idea
        $('#NewIdeaTitle').val('');
        $('#Tag').val('');
        $('#NewIdeaId').val(generateGuid());
        $('#SubmitNewIdeaButton').hide();
    };

    self.loadNextIdeas = function (clearOnUpdate, onSuccess) {
        if (clearOnUpdate == true) {
            self.nextPageToken = null;
        }

        // Exit out if the button has already been disabled.
        // Mostly happens during automated testing...
        if (self.loadNextButton.is(':visible') && self.loadNextButton.is(":disabled"))
            return;
        
        // Then disable it...
        self.loadNextButton.attr('disabled', 'disabled');

        api.ideas.getPage(
            excludedTag,
            self.nextPageToken,
            self.etag,
            function (data, textStatus, jqXHR) {
                // Do nothing if there is no data
                if (data != undefined) {
                    // Handle the new page of ideas
                    self.onIdeasPageReceived(data, clearOnUpdate);

                    // Remember the etag so it can be reused
                    self.etag = jqXHR.getResponseHeader('Etag');
                }

                if (onSuccess) onSuccess();
            }
        );
    };

    self.onIdeasPageReceived = function (ideasPage, clearOnUpdate) {
        // Save the new page token.
        self.nextPageToken = ideasPage.NextPageToken;

        // Translate the data to the view model.
        var mappedData = $.map(ideasPage.Page, function (item) {
            return new IdeaViewModel(item);
        });

        if (clearOnUpdate == true) {
            self.ideas(mappedData);
        } else {
            // Append the new items to the ideas list.
            self.ideas.push.apply(self.ideas, mappedData);
        }

        // Show the data on the page
        $('#Ideas').fadeIn();

        if (self.nextPageToken == null) {
            self.loadNextButton.fadeOut();
        } else {
            self.loadNextButton.fadeIn();
        }

        self.loadNextButton.attr('disabled', false);
        ScrollToBottom();
    };
}

// Defines the model for an Idea in the Ideas view.
function
IdeaViewModel(ideaData) {
    var self = this;

    self.Id    = ko.observable(ideaData.Id);
    self.Title = ko.observable(ideaData.Title);
    self.Tag   = ko.observable(ideaData.Tag);
    self.Goals = ko.observableArray();
    self.Tasks = ko.observableArray();

    self.GoalsEtag = '*';
    self.TasksEtag = '*';

    self.toggleIdeaBody = function (idea) {
        var ideaId      = idea.Id();
        var ideaElement = $('#' + ideaId);
        var ideaBody    = ideaElement.find('.IdeaBody');

        if (ideaBody.is(':visible')) {
            ideaBody.hide();
            return;
        }

        // Register the form with the model validator
        $.validator.unobtrusive.parse($('#' + ideaId + ' .NewGoalForm'));
        $.validator.unobtrusive.parse($('#' + ideaId + ' .NewTaskForm'));

        // Show the data on the page
        ideaBody.show();

        ideaBody.find('.NewGoalTitle')
            .on('input', validateGoalTitle)
            .autosizeInput();

        ideaBody.find('.NewTaskTitle')
            .on('input', validateTaskTitle)
            .autosizeInput();
    };

    self.toggleGoalsDisplay = function (idea) {
        var ideaId = idea.Id();
        var goalsDisplay = $('#' + ideaId + ' .GoalsDisplay');

        if (goalsDisplay.is(':visible')) {
            goalsDisplay.hide();
        } else {
            self.updateGoals(ideaId);
            goalsDisplay.show();
        }
    };

    self.toggleTasksDisplay = function (idea) {
        var ideaId = idea.Id();
        var tasksDisplay = $('#' + ideaId + ' .TasksDisplay');

        if (tasksDisplay.is(':visible')) {
            tasksDisplay.hide();
        } else {
            self.updateTasks(ideaId);
            tasksDisplay.show();
        }
    };

    self.updateGoals = function (ideaId) {
        api.ideas.goals.getPage(
            ideaId,
            self.GoalsEtag,
            function (data, textStatus, jqXHR) {
                if (data == undefined) return;

                // Translate the data to the view model.
                var mappedData = $.map(data, function (goalData) {
                    return new GoalViewModel(goalData);
                });

                self.Goals(mappedData);

                // Remember the etag so it can be reused
                // TODO: Need to remember the etag on a per ideaId basis...
                self.GoalsEtag = jqXHR.getResponseHeader('Etag');
            }
        );
    };

    self.updateTasks = function (ideaId) {
        api.ideas.tasks.getPage(
            ideaId,
            self.TasksEtag,
            function (data, textStatus, jqXHR) {
                if (data == undefined) return;

                // Translate the data to the view model.
                var mappedData = $.map(data, function (taskData) {
                    return new TaskViewModel(taskData);
                });

                self.Tasks(mappedData);

                // Remember the etag so it can be reused
                // TODO: Need to remember the etag on a per ideaId basis...
                self.TasksEtag = jqXHR.getResponseHeader('Etag');
            }
        );
    };

    self.submitNewGoal = function (formElement) {
        var form = $(formElement);

        if (!form.valid()) return;

        var createGoalCommand = {};

        var newGoalId = form
            .find('.NewGoalId')
            .val();

        form
            .serializeArray()
            .map(function (formInput) {
                createGoalCommand[formInput.name] = formInput.value;
            });

        api.commands.send(
            newGoalId,
            'CreateGoalCommand',
            createGoalCommand,
            function () {
                var goalViewModel = ConvertCreateGoalCommandToGoalViewModel(
                    createGoalCommand
                );

                self.Goals.unshift(goalViewModel);

                // Reset the form state for a new goal
                form.find('.NewGoalId').val(generateGuid());
                form.find('.NewGoalTitle').val('');
                form.find('.NewGoalButton').fadeOut();
                form.find('.field-validation-error').fadeOut();
            }
        );
    };

    self.submitNewTask = function (formElement) {
        var form = $(formElement);

        if (!form.valid()) return;

        var createTaskCommand = {};

        var newTaskId = form
            .find('.NewTaskId')
            .val();

        form
            .serializeArray()
            .map(function (formInput) {
                createTaskCommand[formInput.name] = formInput.value;
            });

        api.commands.send(
            newTaskId,
            'CreateTaskCommand',
            createTaskCommand,
            function () {
                var taskViewModel = ConvertCreateTaskCommandToTaskViewModel(
                    createTaskCommand
                );

                self.Tasks.unshift(taskViewModel);

                // Reset the form state for a new task
                form.find('.NewTaskId').val(generateGuid());
                form.find('.NewTaskTitle').val('');
                form.find('.NewTaskButton').fadeOut();
                form.find('.field-validation-error').fadeOut();
            }
        );
    };
}

// Defines the model for a Goal in the Idea view.
function
GoalViewModel(goalData) {
    var self = this;

    self.Id    = ko.observable(goalData.Id);
    self.Title = ko.observable(goalData.Title);
}

function 
TaskViewModel(
    taskData) {
    var self = this;

    self.Id                      = ko.observable(taskData.Id);
    self.Title                   = ko.observable(taskData.Title);
    self.CompletedVoteCount      = ko.observable(taskData.CompletedVoteCount);
    self.NextVoteTaskCompletedId = generateGuid();

    self.voteTaskCompleted = function (task) {
        var taskId      = task.Id();
        var taskElement = $('#' + taskId);
        
        // Disable the button so the user cannot click more than once.
        taskElement
            .find('.VoteCompletedButton')
            .attr('disabled', 'disabled');

        var voteTaskCompletedCommand = {
            TaskId: taskId
        };

        api.commands.send(
            self.NextVoteTaskCompletedId,
            "VoteTaskCompletedCommand",
            voteTaskCompletedCommand,
            function() {
                // Calculate a new vote count.
                var newCount = task.CompletedVoteCount() + 1;

                // Update the UI with the new count.
                task.CompletedVoteCount(newCount);
                
                // Hide the status indicator.
                taskElement
                    .find('.RequestStatus')
                    .hide();
            }
        );
        
        // Show the request status indicator.
        taskElement
            .find('.RequestStatus')
            .show();
    };
}

function 
ConvertCreateTaskCommandToTaskViewModel(
    createTaskCommand) {
    return new TaskViewModel({
        Id:                 createTaskCommand.NewTaskId,
        Title:              createTaskCommand.NewTaskTitle,
        CompletedVoteCount: 0
    });
}

function 
ConvertCreateGoalCommandToGoalViewModel(
    createGoalCommand) {
    return new GoalViewModel({
        Id:     createGoalCommand.NewGoalId,
        Title:  createGoalCommand.NewGoalTitle
    });
}

function
ConvertCreateIdeaCommandToIdeaViewModel(
    createIdeaCommand) {

    return new IdeaViewModel({
        Id:     createIdeaCommand.NewIdeaId,
        Title:  createIdeaCommand.NewIdeaTitle,
        Tag:    createIdeaCommand.Tag
    });
}

var DefaultExcludedTag = 'votus-test';
var excludedTag        = DefaultExcludedTag;
var TagButtonVotusTest = $('#TagButtonVotusTest');

function configTagFilters() {
    var excludedTagQueryStringValue = $.url.param('excludeTag');

    if (excludedTagQueryStringValue != undefined) {
        excludedTag = excludedTagQueryStringValue;
    }

    setExcludeTag();

    TagButtonVotusTest.click(function () {
        var loadingIcon = $('#TagFilterLoadingIdeasIcon');

        if (excludedTag == DefaultExcludedTag) {
            excludedTag = '';
        } else {
            excludedTag = DefaultExcludedTag;
        }

        setExcludeTag();
        $('#Ideas').hide();
        loadingIcon.show();
        ideasViewModel.loadNextIdeas(
            true,
            function () { loadingIcon.hide(); }
        );
    });
}

function validateTaskTitle() {
    var form = $(this).closest('form');
    var button = form.find('.NewTaskButton');

    if (form.valid()) {
        button.fadeIn();
    } else {
        button.fadeOut();

        var currentFormVal = form.find('.NewTaskTitle').val();

        if (currentFormVal == '') {
            form.find('.field-validation-error').fadeOut();
        } else {
            form.find('.field-validation-error').fadeIn();
        }
    }
}

function validateGoalTitle() {
    var form = $(this).closest('form');
    var button = form.find('.NewGoalButton');

    if (form.valid()) {
        button.fadeIn();
    } else {
        button.fadeOut();

        var currentFormVal = form.find('.NewGoalTitle').val();

        if (currentFormVal == '') {
            form.find('.field-validation-error').fadeOut();
        } else {
            form.find('.field-validation-error').fadeIn();
        }
    }
}

function validateTitle() {
    if ($('#SubmitIdeaForm').valid()) {
        $('#SubmitNewIdeaButton').fadeIn();
    } else {
        $('#SubmitNewIdeaButton').fadeOut();
    }
}

function setExcludeTag() {
    if (excludedTag == DefaultExcludedTag) {
        TagButtonVotusTest.addClass('TagExcluded');
    } else {
        TagButtonVotusTest.removeClass('TagExcluded');
    }
}

// Runs after the DOM has finished loading
$(function () {
    initializeKnockout();
    configTagFilters();

    ideasViewModel.loadNextIdeas();

    $('#Tag').autosizeInput();
    $('#NewIdeaTitle').autosizeInput();
    $('#NewIdeaTitle').on('input', validateTitle);

    $('input[placeholder]').inputHints();
});

function positionFooter() {
    var mFoo = $("footer");

    if ((($(document.body).height() + mFoo.outerHeight()) < $(window).height() && mFoo.css("position") == "fixed") || ($(document.body).height() < $(window).height() && mFoo.css("position") != "fixed")) {
        mFoo.css({ position: "fixed", bottom: "0px" });
    } else {
        mFoo.css({ position: "static" });
    }
}

$(document).ready(function () { positionFooter(); $(window).scroll(positionFooter); $(window).resize(positionFooter); $(window).load(positionFooter); });