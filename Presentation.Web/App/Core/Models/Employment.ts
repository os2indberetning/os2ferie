module app.core.models {
    "use strict";

    export class Employment {
        Id: number;
        EmploymentId: number;
        Position: string;
        IsLeader: boolean;
        StartDateTimstamp: number;
        EndDateTimestamp: number;
        EmploymentType: number;
        ExtraNumber: number;
        WorkDistanceOverride: number;
        HomeWorkDistance: number;
        //TODO Missing AlternativeWorkaddress
        PersonId: number;
        Person: Person;
        OrgUnitId: number;
        OrgUnit: OrgUnit;
        CostCenter: number;
    }
}