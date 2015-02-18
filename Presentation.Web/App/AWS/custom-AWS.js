/*
 * SpatialFind jQuery plugin
 *
 * Copyright 2011, Pimin Konstantin Kefaloukos pke@grontmij.dk
 * Dual licensed under the MIT or GPL Version 2 licenses.
 *
 */
(function ($) {

    $.fn.spatialfind = function (options) {

        // all valid settings are here: 
        var settings = {
            'baseurl': 'https://smartadresse.dk',
            'serviceurl': '/service/locations/3/',
            'apikey': 'replaceme',         // REST apikey - MUST be supplied
            'resource': 'detect',          // REST resource: streetname | addressaccess | addressspecific | mixed | detect
            'area': null,                  // REST area
            'limit': 15,                   // REST limit
            'crs': 'EPSG:25832',           // REST projection
            'nogeo': true,                 // REST nogeo
            'noadrspec': true,             // REST noadrspec
            'select': function () { },       // callback function: bliver kaldt når et element vælges
            'recieved': function () { },     // callback function: bliver kaldt når der kommer en ny liste af elementer
            'error': function () { },        // callback function: bliver kaldt når der opstår en fejl
            'doNotRoundTopCorners': false  // Afrund ikke de øvre hjørner i element listen (jqueryui corners)
        };

        return this.each(function () {
            // If options exist, lets merge them
            // with our default settings
            if (options) {
                $.extend(settings, options);
            }

            // only apply to input fields
            var $this = $(this);
            if ($this.is("input[type=text]")) {
                // Assume that jQuery UI and autocomplete has been loaded

                settings.baseurl = settings.baseurl + settings.serviceurl + settings.resource + "/json/";

                var parameters = {};
                parameters.apikey = settings.apikey;
                parameters.limit = settings.limit;
                parameters.crs = settings.crs;
                parameters.nogeo = settings.nogeo;
                parameters.noadrspec = settings.noadrspec;
                if (settings.area)
                    parameters.area = settings.area;

                $this.autocomplete({
                    selectFirst: true,
                    delay: 200,
                    minLength: 1,
                    source: function (request, response) {
                        $.ajax({
                            url: settings.baseurl + encodeURIComponent(request.term),
                            dataType: "jsonp",
                            data: parameters,
                            success: function (result) {
                                // add called url to result for convenience 
                                result.fromUrl = this.url;

                                if (result.status == "ERROR")
                                    // notify user defined handler for result
                                    settings.error("Service Error: " + result.message);
                                else {
                                    // place result in autocomplete
                                    response($.map(result.data, function (item) {
                                        displayLabel = item.presentationString;
                                        displayValue = item.presentationString;
                                        return {
                                            label: displayLabel,
                                            value: displayValue,
                                            data: item
                                        };
                                    }));
                                    // notify user defined handler for result
                                    settings.recieved(result);
                                }
                            },
                            error: function (result) {
                                settings.error("Service not reachable");
                            }
                        });
                    },
                    select: function (event, ui) {
                        settings.select(ui.item.data);
                    },
                    open: function () {
                        if (!settings.doNotRoundTopCorners)
                            $(this).siblings('ul.ui-autocomplete').removeClass("ui-corner-all").addClass("ui-corner-bottom");
                    },
                    close: function (event, ui) {
                        if (!settings.doNotRoundTopCorners)
                            $(this).siblings('ul.ui-autocomplete').removeClass("ui-corner-bottom").addClass("ui-corner-all");
                    }
                });
            }
        });

    };
})(jQuery);

/*
 * jQuery UI Autocomplete Select First Extension
 * 
 * Copyright 2010, Scott González (http://scottgonzalez.com) Dual licensed under
 * the MIT or GPL Version 2 licenses.
 * 
 * http://github.com/scottgonzalez/jquery-ui-extensions
 */
(function ($) {

    $(".ui-autocomplete-input").live("autocompleteopen", function () {
        var autocomplete = $(this).data("autocomplete"), menu = autocomplete.menu;

        if (!autocomplete.options.selectFirst) {
            return;
        }
        // the requested term no longer matches the search, so drop out of this
        // now
        if (autocomplete.term != $(this).val()) {
            // console.log("mismatch! "+autocomplete.term+'|'+$(this).val());
            return;
        }
        // hack to prevent clearing of value on mismatch
        menu.options.blur = function (event, ui) {
            return;
        };
        menu.activate($.Event({
            type: "mouseenter"
        }), menu.element.children().first());

    });

}(jQuery));