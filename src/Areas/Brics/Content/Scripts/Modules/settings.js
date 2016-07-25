class Settings {

    constructor(elem) {

        var self = this;

        this.elem = elem;

        document.addEventListener('iframe-loaded', evt => this.load(evt), false);

        this.elem.addEventListener("keydown", function (e) {
            if (e.keyCode == 83 && (navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey)) {
                e.preventDefault();
                self.save();
            }
        }, false);

    }

    load(evt) {

        var self = this;
        
        fetch('/api/settings/1?url=' + evt.detail.url, {
            method: 'get'
        }).then(function (response) {
            response.text().then(function (text) {
                self.elem.innerHTML = text;
            })
        }).catch(function (err) {
            // Error :(
        });

    }

    save() {

        var form = document.querySelector('aside form');

        fetch(form.action, {
            method: 'post',
            body: new FormData(form)
        }).then(function (response) {
            //var event = new Event('app:saved');
            //document.dispatchEvent(event);
        }).catch(function (err) {
            // Error :(
        });

    }

}

module.exports = Settings;