module app.core.models {
    "use strict";

    export class VacationBalance {
        Id: number;
        Year: number;
        TotalVacationHours: number;
        VacationHours: number;
        TransferredHours: number;
        FreeVacationHours: number;
        UpdatedAt: number;
        EmploymentId: number;
        Employment: Employment;
        PersonId: number;
        Person: Person;
    }
}