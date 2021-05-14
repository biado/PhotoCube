/**
 * Represents a Tagset in the M^3 datamodel.
 * Is similar to Tagset.cs in the server implementation.
 */
export default interface Tagset{
    Id: number;
    Name: string;
    Tags: [] | null;
}