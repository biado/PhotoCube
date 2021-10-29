/**
 * Represents a Tagset in the M^3 datamodel.
 * Is similar to Tagset.cs in the server implementation.
 */
export default interface Tagset{
    id: number;
    name: string;
    tags: [] | null;
}