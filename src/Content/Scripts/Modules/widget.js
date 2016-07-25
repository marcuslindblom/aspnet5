
class Widget {

    constructor(elem, config) {

        this.config = config;
            
        elem.addEventListener('click', evt => this.onclick(evt));        

    }

    onclick(evt) {

        evt.preventDefault();
        evt.stopPropagation();

        var request = new XMLHttpRequest();

        request.open('GET', '/api/foo/1?propertyName=' + this.config.name, true);

        request.onload = function () {

            if (request.status >= 200 && request.status < 400) {
                // Success!

                var data = request.responseText;

                var settings = document.querySelector('.settings-view');

                settings.innerHTML = data;

            } else {

                // We reached our target server, but it returned an error

            }
        };

        request.onerror = function () {
            // There was a connection error of some sort
        };

        request.send();

    }

}

module.exports = Widget;