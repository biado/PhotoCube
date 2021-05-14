/**
 * Interface representing an option used in the hierarchy filter. 
 * Displayed in a dropdown as nodeName:parentnodeName.
 */
 export interface Option {
    NodeId: number,
    NodeName: string,
    ParentnodeName: string
}
