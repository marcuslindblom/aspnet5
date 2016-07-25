import Iframe from './Modules/iframe';
import Actions from './Modules/actions';
import Settings from './Modules/settings';

class Application {

    constructor() {

        var iframeEl = document.getElementById('iframe');

        var iframe = new Iframe(iframeEl);

        var settingsEl = document.getElementById('settings');

        var settings = new Settings(settingsEl);

    }
}

export default new Application();