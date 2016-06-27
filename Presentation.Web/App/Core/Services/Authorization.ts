module app.core.services {

    class Authorization {

        private _identity;
        private _authenticated = false;
        private enabledModules: string[] = [];

        static $inject: string[] = [
            "$rootScope",
            "$state",
            "Principal",
            "EnabledApplications"
        ];

        constructor(private $rootScope: any, private $state, private principal: Principal, EnabledApplications: any) {
            this.enabledModules = EnabledApplications.toLowerCase().split(', ');
        }

        private getModule(state) {
            return state.split('.')[0];
        }

        private isModuleEnabled(module) {
            return this.enabledModules.indexOf(module) !== -1;
        }

        authorize() {
            return this.principal.identity().then(() => {
                var isAuthenticated = this.principal.isAuthenticated();

                var module = this.getModule(this.$rootScope.toState.name);
                var modules = this.intersect(this.enabledModules, this.principal.accessibleModules());

                if (isAuthenticated) {
                    if (modules.length == 0) {
                        this.$rootScope.hasAccess = false;
                    }
                    else if (module != 'default' && !this.canAccessModule(module, modules)) {
                        // Trying to access inaccessible module, so redirect to start
                        this.$state.go("default");
                    }
                    else if (module == 'default' && modules.length == 1) {
                        // Only has access to one module, so redirect to it.
                        if (modules == 'drive') {
                            this.$state.go("drive.driving");
                        }
                        else if (modules == 'vacation') {
                            this.$state.go("vacation.report");
                        }
                    } else {
                        // Has access to module, so see if user has access to state otherwise redirect to start
                        if (this.$rootScope.toState.data && this.$rootScope.toState.data.roles
                            && this.$rootScope.toState
                                .data.roles.length > 0
                            && !this.principal.isInAnyRole(
                                this.$rootScope.toState.data.roles)) {
                            this.$state.go("default");
                        }
                    }
                }
            });
        }

        private canAccessModule(module, modules) {
            return modules.indexOf(module) !== -1;
        }

        private intersect(a, b) {
            var t;
            if (b.length > a.length) t = b, b = a, a = t; // indexOf to loop over shorter
            return a.filter(function (e) {
                if (b.indexOf(e) !== -1) return true;
            });
        }

    }

    angular.module("app.core").service("Authorization", Authorization);
}
