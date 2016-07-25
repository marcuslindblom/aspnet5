import Editor from './editor';

class Widget {

    constructor(elem, config) {

        this.elem = elem;
        this.config = config;
        var self = this.elem;

        elem.addEventListener('click', evt => this.onclick(evt));
        document.addEventListener('iframe-click', evt => this.elem.classList.remove('focus'), false);

    }

    onclick(evt) {

        evt.preventDefault();
        evt.stopPropagation();

        console.log(evt.target);
        console.log(evt.currentTarget);

        if(evt.target != evt.currentTarget) {
            
        }

        this.elem.classList.add('focus');

        var event = new Event('widget:click');
        document.dispatchEvent(event);

        // url (required), options (optional)
        fetch('/api/foo/1?propertyName=' + this.config.name, {
            method: 'get'
        }).then(function (response) {
            response.text().then(function (text) {

                var settings = document.querySelector('.nav-body');

                settings.innerHTML = text;

                var editor = new Editor(settings);

            })
        }).catch(function (err) {
            // Error :(
        });

    }

}

module.exports = Widget;