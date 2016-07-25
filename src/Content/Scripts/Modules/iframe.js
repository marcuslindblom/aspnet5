import Widget from './Widget';

class Iframe {

    constructor(elem) {

        var self = this;

       var iframe = document.createElement('iframe');

        iframe.style.display = 'flex';
        iframe.style.flexGrow = '1';
        iframe.style.border = '0';
        iframe.style.backgroundColor = 'transparent';
        iframe.setAttribute('allowtransparency', 'true');

        iframe.onload = function () {

            try {

                var doc = this.contentDocument || this.contentWindow.document;

                doc.addEventListener('click', function () {
                    var event = new Event('iframe-click');
                    document.dispatchEvent(event);
                });

                var hosts = doc.querySelectorAll('[data-module="widget"]');

                var template = document.querySelector('.widget-template');

                [].forEach.call(hosts, function (host) {
                    var root = host.createShadowRoot();
                    root.appendChild(document.importNode(template.content, true));
                    var configEl = host.querySelector('script[type="text/x-config"]');
                    new Widget(host, JSON.parse(configEl.textContent));
                });

                //var stateObj = {};
                //console.log('iframe is loaded: ' + doc.location.pathname); // det är är vi ska skicka meddelande
                //history.pushState(stateObj, "page 2", '/brics/editor' + doc.location.pathname);

                var event = new CustomEvent('iframe-loaded', { 'detail': '/' + doc.location.pathname.split('/').filter(Boolean).join('/') });
                document.dispatchEvent(event);

            } catch (e) {
                // This can happen if the src of the iframe is
                // on another domain
                alert('exception: ' + e);
            }

        };

        iframe.src = '/';

        elem.appendChild(iframe);

    }

}

module.exports = Iframe;