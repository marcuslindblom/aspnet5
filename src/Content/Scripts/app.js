﻿import Iframe from './Modules/iframe';
import Actions from './Modules/actions';

class Application {

    constructor() {

        var iframeEl = document.getElementById('iframe');

        var iframe = new Iframe(iframeEl);

    }
}

export default new Application();