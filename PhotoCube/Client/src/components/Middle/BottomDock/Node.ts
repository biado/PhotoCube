/**
 * Interface representing a node in the hierarchy browser. 
 * Based on PublicNode.cs at the server side. 
 */
 export interface Node {
    id: number,
    name: string, //| number, //both alpha- and numerical tags
    parentNode: Node|null
}
