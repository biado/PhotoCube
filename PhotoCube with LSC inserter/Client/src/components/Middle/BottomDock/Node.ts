/**
 * Interface representing a node in a hierarchy. Based on PublicNode.cs at the server side. 
 */
 export interface Node {
    Id: number,
    Name: string,
    ParentNode: Node|null
}