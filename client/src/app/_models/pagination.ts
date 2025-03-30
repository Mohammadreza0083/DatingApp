export interface pagination {
    currentpage: number;
    itemsperpage: number;
    totalItems:number;
    totalpages:number;
}

export class paginatedResult<T>{
    item?: T;
    pagination?:pagination
}