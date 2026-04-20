import QGlobal from '@/global.js';
import {postJSON} from '@/utils/globalUtils.js';
import moment from 'moment';
import store from '../store';

export const QUtils = {
  log: function () { if (QGlobal.activeLog) { console.log(arguments); } },
  warn: function () { if (QGlobal.activeLog) { console.warn(arguments); } },
  error: function () { if (QGlobal.activeLog) { console.error(arguments); } },
  /**
  * Get complete URL for action with culture
  * @param {string} controller - Controller name
  * @param {string=} action - Action name
  * @param {Object=} queryString - Query String
  * @returns {string} - URL
  */
  apiActionURL: function (controller, action, queryString) {
    var year = store.getters.Year,
      lang = store.getters.Language,
      app = store.getters.App;

    var qs = $.extend(true, { culture: lang, system: year, appId: app }, queryString || {});

    //var url = String.format('api/{0}/{1}/{2}{3}?{4}', lang, year, controller, action ? '/' + action : '', $.param(qs));
	var url = `api/${lang}/${year}/${controller}${action ? '/' + action : ''}?${$.param(qs)}`;
	
    return url;
  },
  /**
  * AJAX GET with support of .Net DateTime objects
  * @param {string} url - Action URL
  * @param {boolean} asynchronously - Async request
  * @returns {requestDeferred} - Request callback
  */
  FetchData: function (url, asynchronously) {
    return $.ajax({
      url: url,
      dataType: 'json',
      async: (asynchronously || true),
      headers: {
        'Cache-Control': 'no-cache'
      },
      beforeSend: function () {

      },
      complete: function () {

      },
      cache: false,
      // Override jQuery´s strict JSON parsing
      converters: {
        'text json': function (result) {
          try {
            // First try to use native browser parsing
            if (typeof JSON === 'object' && typeof JSON.parse === 'function') {
              return JSON.parse(result, function (key, value) {
                if (typeof value === 'string') {
                  var pattern = /Date\(([^)]+)\)/; // Convert .Net DateTime to JavaScript date format (momentjs)
                  return pattern.test(value) ? moment(value) : value;
                }
                return value;
              });
            } else {
              // Fallback to jQuery´s parser
              QUtils.warn("parseJSON", result);
              return $.parseJSON(result);
            }
          } catch (e) {
            QUtils.log("Warning: Could not parse expected JSON response.");
            return { }; // Empty JS object.
          }
        }
      }
    });
  },
      /**
    * POST data to Server
    * @param {string} controller - Controller name
    * @param {string} action - Action name
    * @param {Object} data - Data in Json format
    * @param {Object=} queryString - Query String (Json Format)
    * @param {Function=} callback - Callback function
    * @returns {DeferredPermissionRequest} - Request callback
    */
  postData: function (controller, action, data, queryString, callback) {
    var url = QUtils.apiActionURL(controller, action, queryString);
    return postJSON(url, data, callback).done(function (data, textStatus, jqXHR) {
      // var system = jqXHR.getResponseHeader('system');
      // if (!$.isEmptyObject(system)) { store.dispatch('changeYear', system); }
    });
  },
    createGuid: function () {
        function _p8(s) {
            var p = (Math.random().toString(16) + "000000000").substr(2, 8);
            return s ? "-" + p.substr(0, 4) + "-" + p.substr(4, 4) : p;
        }
        return _p8() + _p8(true) + _p8(true) + _p8();
    },
    tryParseDate: function (value) {
        if (jQuery.type(value) === "string") {
            // Try convert Csharp string to JS date
            var patternCSharp = /Date\(([^)]+)\)/,
                patternJSON = /(\d{4}-\d{2}-\d{2})[T](\d{2}:\d{2}:\d{2}.?(\d{3})?)[Z]?/;
            if (patternCSharp.test(value)) {
                return moment.utc(value);
            } else if (patternJSON.test(value)) {
                return moment.utc(value);
            } else if (value === "") {
                return null; // Null is the default value of empty Date control
            }
        }
        return null;
    }
};
