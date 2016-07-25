import Widget from './Widget';
import Fullscreen from './Fullscreen';

class Iframe {

    trimUrl(url) {
        var segments = url.split('/');
        return segments.filter(Boolean).splice(2).join('/');
    }

    constructor(elem) {

        var self = this;

        var iframe = document.createElement('iframe');

        iframe.style.display = 'flex';
        iframe.style.flexGrow = '1';
        iframe.style.border = '0';
        iframe.style.backgroundColor = 'transparent';
        iframe.setAttribute('allowtransparency', 'true');
        iframe.onload = this.onload;
        iframe.src = '/';

        elem.appendChild(iframe);

        document.addEventListener('app:saved', () => {
            iframe.src = '/' + self.trimUrl(document.location.pathname);
        }, false);
    }

    onload() {

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

            var t = document.querySelector('.full-screen-widget');

            var h = doc.querySelector('html body');

            var container = doc.createElement('span');

            h.appendChild(container);

            var r = container.createShadowRoot();

            r.appendChild(document.importNode(t.content, true));
            new Fullscreen(container);

            //var stateObj = {};
            history.pushState({}, "page 2", '/brics/editor' + doc.location.pathname);

            var event = new CustomEvent('iframe-loaded', { 'detail': { url: '/' + doc.location.pathname.split('/').filter(Boolean).join('/') }});
            document.dispatchEvent(event);

        } catch (e) {
            // This can happen if the src of the iframe is
            // on another domain
            alert('exception: ' + e);
        }
    }

}

module.exports = Iframe;