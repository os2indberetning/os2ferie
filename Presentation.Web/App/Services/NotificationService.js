angular.module("application").factory("NotificationService", ["$timeout", function NotificationService($timeout) {
    var animSpeed = 250;

    var Notification = function (type, title, message) {
        return {
            type: type,
            title: title,
            message: message
        }
    };

    var container = $(".errors-container");

    var closeElement = $("<button />").attr("role", "alert").attr("type", "button").addClass("close");
    closeElement.append($("<span />").attr("aria-hidden", "true").append("&times;"));
    closeElement.append($("<span />").addClass("sr-only").append("Close"));

    var strongElement = $("<strong />");

    var notificationElement = $("<div />").addClass(["alert", "alert-dismissable", "pointer-on-hover"].join(" ")).append(closeElement);

    var notificationFactory = function (type, title, message) {
        return new Notification(type, title, message);
    };

    var publishNotification = function (notification) {
        var n = notificationElement.clone();
        var title = strongElement.clone();

        title.append(notification.title);

        n.append(title);
        n.append(" ");
        n.append(notification.message);
        n.addClass(["alert", notification.type].join("-"));

        n.hide();

        n.on("click", function () {
            n.slideUp(animSpeed, function () {
                n.remove();
            });
        });

        container.prepend(n);

        n.slideDown(animSpeed);

        return n;
    };

    return {
        NotificationFactory: notificationFactory,
        PublishNotification: publishNotification,
        SimpleNotification: function (type, title, message) {
            publishNotification(notificationFactory(type, title, message));
        },
        AutoFadeNotification: function (type, title, message) {
            var n = publishNotification(notificationFactory(type, title, message));

            $timeout(function () {
                n.slideUp(function () {
                    n.remove();
                });
            }, 5000);
        }
    };
}]);