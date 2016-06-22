module app.core.services {

    export class Principal {

        private _identity;
        private _authenticated = false;
        private _roles: string[] = [];
        private _accessibleModules: string[] = [];

        static $inject: string[] = [
            "$q",
            "$rootScope",
            "Person"
        ];

        constructor(private $q: any, private $rootScope: any, private Person: any) {
        }

        isIdentityResolved() {
            return angular.isDefined(this._identity);
        }

        isAuthenticated() {
            return this._authenticated;
        }

        accessibleModules() {
            return this._accessibleModules;
        }

        isInRole(role) {
            if (!this._authenticated) return false;
            return this._roles.indexOf(role) !== -1;
        }

        isInAnyRole(roles) {
            if (!this._authenticated) return false;

            for (let i = 0; i < roles.length; i++) {
                if (this.isInRole(roles[i])) return true;
            }

            return false;
        }

        authenticated(identity) {
            this._identity = identity;
            this._authenticated = identity != null;
        }

        identity(force = false) {
            var deferred = this.$q.defer();

            if (force) this._identity = undefined;

            if (angular.isDefined(this._identity)) {
                deferred.resolve(this._identity);

                return deferred.promise;
            }

            this.Person.GetCurrentUser((data) => {
                this._identity = data;

                if (data.IsAdmin) {
                    this._roles.push('Admin');
                }

                if (data.IsLeader || data.IsSubstitute) {
                    this._roles.push('Approver');
                }

                if (data.isLeader) {
                    this._roles.push('Leader');
                }

                // All org units can access the drive module.
                this._accessibleModules.push("drive");

                // Loop to find if the person has an employment with access to vacation
                for (let i = 0; i < data.Employments.length; i++) {
                    const employment = data.Employments[i];
                    if (employment.OrgUnit.HasAccessToVacation) {
                        this._accessibleModules.push("vacation");
                        break;
                    }
                }

                // The rootscope assignments is all used in (old)code for fallback safety
                // New code should use Principal.Identity() instead of $rootScope.CurrentUser
                this.$rootScope.CurrentUser = data;
                this.$rootScope.showAdministration = data.IsAdmin;
                this.$rootScope.showApproveReports = data.IsLeader || data.IsSubstitute;
                this.$rootScope.UserName = data.FullName;
                this.$rootScope.loaded = true;

                this._authenticated = true;
                deferred.resolve(this._identity);
            },
            () => {
                this._identity = null;
                this._authenticated = false;
                this._roles = [];
                this.$rootScope.loaded = false;
                deferred.resolve(this._identity);
            });

            return deferred.promise;
        }

    }

    angular.module("app.core").service("Principal", Principal);
}
