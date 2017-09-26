module app.core.models {
    "use strict";

    export enum VacationType {
        Regular,
        SixthVacationWeek,
        Care,
        Senior,
        Optional,
        Other
    }

    export class VacationReport extends Report {
        StartTimestamp: number;
        EndTimestamp: number;
        StartTime: string;
        EndTime: string;
        Purpose: string;
        CareCpr: string;
        OptionalText: string;
        VacationYear: number;
        VacationHours: number;
        VacationType: VacationType;
    }
}