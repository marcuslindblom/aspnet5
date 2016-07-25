class Actions {

    constructor() {

        var elem = document.querySelector('section.view-actions button[data-type="btn-open"]'); // todo

        elem.addEventListener('click', evt => this.openEditor(evt), false);

        document.addEventListener('iframe-click', evt => this.closeEditor(evt), false);

    }


    /*
        clean up how we fetch aside
    */
    openEditor(evt) {

        var aside = document.querySelector('aside');
        aside.classList.add('open');

    }

    /*
        clean up how we fetch aside
    */
    closeEditor(evt) {

        var aside = document.querySelector('aside');
        aside.classList.remove('open');

    }

}

export default new Actions();