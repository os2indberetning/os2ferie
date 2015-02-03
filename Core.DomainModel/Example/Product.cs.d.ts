declare module server {
	interface Product extends Entity {
		name: string;
		price: number;
	}
	interface Entity {
		id: any;
	}
}
