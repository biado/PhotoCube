/**
 * An interface for Filter. Now only supports tagId filtering, but can be expanded.
 */
 export interface Filter{
    type: string, //Can be either "hierarchy" or "tag"
    Id: number,
    name: string
}
