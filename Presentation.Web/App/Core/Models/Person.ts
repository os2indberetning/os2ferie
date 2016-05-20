module app.core.models {
    "use strict";

    export class Person {
        Id: number;
        CprNumber: string;
        FirstName: string;
        LastName: string;
        Mail: string;
        RecieveMail: boolean;
        DistanceFromHomeToBorder: number;
        Initials: string;
        FullName: string;
        IsAdmin: boolean;
        IsSubstitute: boolean;
        IsActive: boolean;
        HasAppPassword: boolean;
        Employments: Employment[];

        //TODO Missing naviation properties
    }
}