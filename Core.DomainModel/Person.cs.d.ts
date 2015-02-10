﻿declare module server {
	interface Person {
		id: number;
		cprNumber: string;
		personId: number;
		firstName: string;
		middleName: string;
		lastName: string;
		mail: string;
		workDistanceOverride: number;
		personalAddresses: any[];
		personalRoutes: any[];
		licensePlates: any[];
		mobileTokens: any[];
		reports: any[];
		employments: any[];
		substitutes: any[];
		substituteFor: any[];
		substituteLeaders: any[];
	}
}