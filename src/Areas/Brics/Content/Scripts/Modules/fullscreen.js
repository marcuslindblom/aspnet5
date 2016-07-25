class Fullscreen {
    constructor(elem) {

        elem.addEventListener('click', evt => this.log(evt), false);

    }

    log(evt) {

        document.body.classList.toggle('preview');

        console.log(document);

    }
}

module.exports = Fullscreen;