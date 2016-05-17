module app.core.models {
    "use strict";

    export enum VacationType {
        Regular,
        SixthVacationWeek,
        Other
    }

    export class VacationReport extends Report {
        StartTimestamp: number;
        EndTimestamp: number;
        StartTime: string;
        EndTime: string;
        VacationYear: number;
        VacationHours: number;
        VacationType: VacationType;
    }
}