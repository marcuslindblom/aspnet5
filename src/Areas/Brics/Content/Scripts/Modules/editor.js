class Editor {

    constructor(elem) {

        var self = this;

        this.elem = elem;
        this.form = this.elem.querySelector('form');
        this.form.addEventListener("keydown", function (e) {
            if (e.keyCode == 83 && (navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey)) {
                e.preventDefault();
                self.save();
            }
        }, false);

    }

    save() {

        fetch(this.form.action, {
            method: 'post',
            body: new FormData(this.form)
        }).then(function (response) {
            //var event = new Event('app:saved');
            //document.dispatchEvent(event);
        }).catch(function (err) {
            // Error :(
        });

    }

}

module.exports = Editor;