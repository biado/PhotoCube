/**
 * An interface for Filter. Supports filtering with tags, tagsets and hierarchy nodes.
 */
 export interface Filter{
    id: number, //tagset
    type: string,
    name: string, 
    max?: string //optional opper bound used in slider component. String type to match name used as lower bound
}
