function ScrollToBottom() {
    window.scrollTo(0, document.body.scrollHeight);
}

function generateGuid() {
    var guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
    return guid;
}

function traceObject(obj) {
    var objString = '';

    for (var prop in obj) {
        objString += prop + ': ' + obj[prop] + '\n';
    }

    alert(objString);
}