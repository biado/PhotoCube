/**
 * Interface representing a tag that can be used as a filter.
 * Based on PublicTag.cs at the server side.
 */
 export interface Tag {
    id: number,
    name: string
    tagset: number // to identify which tagset slider is working on
}
