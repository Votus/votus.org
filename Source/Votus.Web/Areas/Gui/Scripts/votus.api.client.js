function VotusApi() {
    var basePath = '/api/';
    
    this.ideas    = new Ideas(basePath);
    this.commands = new Commands(basePath);
}

function Ideas(basePath) {
    this.basePath = basePath + 'ideas/';
    this.goals    = new Goals(this.basePath);
    this.tasks    = new Tasks(this.basePath);

    this.getPage = function(excludeTag, nextPageToken, etag, onSuccess) {
        var queryString = '';

        if (nextPageToken) {
            queryString += 'nextPageToken=' + nextPageToken;
        }

        if (excludedTag) {
            if (queryString.length > 0)
                queryString += '&';

            queryString += 'excludeTag=' + excludedTag;
        }

        if (queryString.length > 0) {
            queryString = '?' + queryString;
        }

        var headers = {};
        headers['If-None-Match'] = etag;

        $.ajax({
            url: this.basePath + queryString,
            type: 'GET',
            headers: headers,
            success: onSuccess
        });
    };
}

function Goals(basePath) {
    this.basePath = basePath;

    this.getPage = function (ideaId, etag, onSuccess) {
        var headers = {};
        headers['If-None-Match'] = etag;

        // Call the API to get the goals for this idea.
        $.ajax({
            url:        this.basePath + ideaId + '/goals',
            type:       'get',
            headers:    headers,
            success:    onSuccess
        });
    };
}

function Tasks(basePath) {
    this.basePath = basePath;
    
    this.getPage = function (ideaId, etag, onSuccess) {
        var headers = {};
        headers['If-None-Match'] = etag;

        // Call the API to get the tasks for this idea.
        $.ajax({
            url:        this.basePath + ideaId + '/tasks',
            type:       'get',
            headers:    headers,
            success:    onSuccess
        });
    };
}

function Commands(basePath) {
    this.basePath = basePath + 'commands/';
    
    this.send = function (commandId, name, payload, onSuccess) {
        var ajaxParams = {
            url:        this.basePath + commandId,
            type:       'PUT',
            data:       { name: name, payload: JSON.stringify(payload) },
            success:    onSuccess
        };

        $.ajax(ajaxParams);
    };
}