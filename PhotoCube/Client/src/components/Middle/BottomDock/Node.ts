/**
 * Interface representing a node in the hierarchy browser. 
 * Based on PublicNode.cs at the server side. 
 */
 export interface Node {
    id: number,
    name: string, //both alpha- and numerical tags have a getTagName() that returns a string
    parentNode: Node|null
}
