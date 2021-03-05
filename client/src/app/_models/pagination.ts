export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

// Here we can use PaginatedResult with any of our different types. For example an array of Members[]
export class PaginatedResult<T> {
    result: T;
    pagination: Pagination;
}