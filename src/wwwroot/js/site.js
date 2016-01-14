Box.Application.addModule('widget', function (context) {

    'use strict';

    var service,
        moduleEl;

    return {
        /**
        * Initializes the module and caches the module element
        * @returns {void}
        */
        init: function () {
            console.log('init');
            moduleEl = context.getElement();

            var config = context.getConfig(),
                url = config.root,
                itemsPerPage = config.itemsPerPage;

            console.log(itemsPerPage);
            console.log(url);
        },

        /**
        * Destroys the module and clears references
        * @returns {void}
        */
        destroy: function () {
            moduleEl = null;
        },

        /**
        * Handles the click event
        * @param {Event} event The event object
        * @param {HTMLElement} element The nearest element with a data-type
        * @param {string} elementType The data-type attribute of the element
        * @returns {void}
        */
        onclick: function (event, element, elementType) {

            this.loadEditor();

        },

        loadEditor: function () {

            moduleEl.innerText = 'hej knekt';
            console.log('loading editor...');

        },

        ondblclick: function (event, element, elementType) {

            moduleEl.setAttribute('contenteditable','true');

        },

        focusin: function () {
            console.log('blur');
        },

        toggleBrics: function () {
            moduleEl.classList.toggle('open');
        }
    };

});

Box.Application.addModule('editor', function (context) {

    'use strict';

    var service,
        moduleEl;

    return {
        /**
        * Initializes the module and caches the module element
        * @returns {void}
        */
        init: function () {
            console.log('init');
            moduleEl = context.getElement();
        },

        /**
        * Destroys the module and clears references
        * @returns {void}
        */
        destroy: function () {
            moduleEl = null;
        },

        /**
        * Handles the click event
        * @param {Event} event The event object
        * @param {HTMLElement} element The nearest element with a data-type
        * @param {string} elementType The data-type attribute of the element
        * @returns {void}
        */
        onclick: function (event, element, elementType) {

            this.togglePanel();

            if (elementType === 'btn-save') {

                $("#page-form").submit(function (e) {

                    var url = "/api/page"; // the script where you handle the form input.

                    $.ajax({
                        type: "POST",
                        url: url,
                        //dataType: 'json',
                        data: $("#page-form").serialize(), // serializes the form's elements.
                        success: function (data) {
                            alert(data); // show response from the php script.
                        }
                    });

                    e.preventDefault(); // avoid to execute the actual submit of the form.
                });
                
            }

        },

        togglePanel : function() {

            var aside = root.querySelector('aside');

            aside.setAttribute('class', 'expanded');
            //aside.classList.toggle('expanded');

        }
    };

});



var hosts = document.querySelectorAll('[id^=_]');

var template = document.querySelector('.widget-template');

[].forEach.call(hosts, function (host) {
    var root = host.createShadowRoot();
    root.appendChild(document.importNode(template.content, true));
    Box.Application.startAll(root);
});


// Fire up the application
Box.Application.init();