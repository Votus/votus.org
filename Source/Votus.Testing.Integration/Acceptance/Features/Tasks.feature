Feature: Tasks
	As a Voter
	I want to add tasks to an idea 
	So that I can describe to other Voters steps they can take to accomplish the Idea

	As a Voter
	I want to see tasks for an idea 
	So that I know what specific steps I can take to accomplish the Idea

Scenario: Voter can add a Task to an Idea
	Given at least 1 idea exists
	When a Voter navigates to the Homepage
	And a Voter submits a valid Task to the Idea
	Then the Task appears under the Idea

Scenario: Voter cannot add invalid Task to an Idea
	Given at least 1 idea exists
	When a Voter navigates to the Homepage
	And a Voter submits an invalid Task to the Idea
	Then the error "Please say a few words about your task" is displayed

Scenario: Task Completed Votes are shown on Tasks
	Given an Idea with a Task exists
	And the Voter navigates to the Homepage
	When a Voter votes a Task is Completed
	Then the Task Completed vote count is incremented

Scenario: A Voter can vote a Task is Completed Once
	Given a Voter has previously voted a Task is Completed
	When the Voter navigates to the Homepage
	Then the Voter cannot vote the Task is Completed again