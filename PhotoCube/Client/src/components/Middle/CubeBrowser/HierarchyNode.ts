import Tag from "./Tag";
import Hierarchy from "./Hierarchy";

/**
 * Represents a node in a hierarchy.
 * Is similar to Node.cs in server implementation.
 */
export default interface HierarchyNode{
    id: number;
    tagId: number;
    tag: Tag;
    hierarchyId: number;
    hierarchy: Hierarchy|null;
    children: HierarchyNode[];
}