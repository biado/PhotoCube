/**
 * Represent a page of result, returned from api/cell?.. call.
 * Is similar to Page.cs in the server implementation.
 */
 export default interface Page<T>{
    CurrentPage:number,
    PageCount:number,
    PageSize:number,
    FileCount:number,
    FirstRowOnPage:number,
    LastRowOnPage:number,

    // List of Type T. Our result is Cell[]
    Results:T[]
}