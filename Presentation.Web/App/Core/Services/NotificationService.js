angular.module("app.core").factory("NotificationService", [function NotificationService() {
    return {
        AutoFadeNotification: function (type, title, message) {
            // Some backwards compatibility.
            if (type == "danger") {
                type = "error";
            }
            if (type == "warning") {
                type = "notice";
            }

            var effect = {
                effect_in: 'drop',
                options_in: {
                    easing: 'easeOutBounce',
                },
                options_out: {
                    easing: 'easeInCubic',
                },
                effect_out: 'drop',
            }

            new PNotify({
                remove: true,
                title: "<br/>" + title,
                text: message,
                min_height: "100px",
                width: "300px",
                type: type,
                icon: true,
                delay: 5000,
                shadow: true,
                animation: effect,
                mouse_reset: false,
                buttons: {
                    sticker: false
                }
            });
        }
    };
}]);