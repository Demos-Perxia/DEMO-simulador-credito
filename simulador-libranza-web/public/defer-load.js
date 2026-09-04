/**
 * Carga diferida de recursos no críticos (fuentes e iconos).
 * Se ejecuta con el atributo defer para no bloquear el primer pintado.
 */
(function () {
  'use strict';

  var TEXT_FONTS =
    'https://fonts.googleapis.com/css2?family=DM+Mono:wght@500;800&family=Manrope:wght@400;700;800&display=swap';
  var ICON_FONT = 'https://fonts.googleapis.com/css2?family=Material+Icons&display=swap';

  function loadStylesheet(href) {
    var link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = href;
    link.media = 'print';
    link.onload = function () {
      link.media = 'all';
    };
    document.head.appendChild(link);
  }

  function loadDeferredScript(src) {
    var script = document.createElement('script');
    script.src = src;
    script.defer = true;
    script.type = 'module';
    document.body.appendChild(script);
  }

  function whenIdle(callback) {
    if ('requestIdleCallback' in window) {
      window.requestIdleCallback(callback, { timeout: 1500 });
      return;
    }
    window.setTimeout(callback, 1);
  }

  loadStylesheet(TEXT_FONTS);
  loadStylesheet(ICON_FONT);

  whenIdle(function () {
    var pending = window.__APP_SCRIPTS__;
    if (Array.isArray(pending)) {
      pending.forEach(loadDeferredScript);
    }
  });
})();
