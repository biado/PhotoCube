/**
 * Represents a Tag in the M^3 model.
 * Is similar to Tag.cs in the server implementation.
 */
export default interface Tag{
    id: number,
    name: string,
    tagset: any,
    tagsetId: number,
    objectTagRelations: any
}