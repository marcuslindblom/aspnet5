import Module from './Modules/module';
import Iframe from './Modules/iframe';

class Application {

    constructor() {

        console.log('initialize application...');

        var m = new Module();
        var i = new Iframe();

    }

}

// Initialize application
new Application();
