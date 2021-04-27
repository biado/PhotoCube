/**
 * JSTagset is the entity that we store information of which tags belong to which tagset. JS stands for Json, which is the input file format.
 * Currently used for the semantic tags and their hierarchies extracted using ImageNetShuffle and WordNet.
 */
public class JSTagset {
    private static final String delimiter = ",,"; // Using 2 commas because 1) semantic_name column has some values that use ',' and 2) timestamp has ':'.
    private String name;
    private JSTagset[] children;

    public JSTagset(String name, JSTagset[] children) {
        this.name = name;
        this.children = children;
    }

    /**
     * Returns the name of this tagset
     * @return the name of this tagset
     */
    public String getName() {
        return this.name;
    }

    /**
     * Returns the children of this tagset
     * @return the children of this tagset
     */
    public JSTagset[] getChildren() {
        return this.children;
    }
    
    /**
     * Returns the hierarchy information from this tagset as String.
     * Format: TagsetName,,HierarchyName,,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n
     */
    public void getHierarchyString(StringBuilder sb, String tagsetName) {
        // case: leaf -> no hierarchy starting from this jsTagset
        if (this.children != null) { // case: parentNode -> We want to build a hierarchy string like TagsetName,,HierarchyName(==TagsetName),,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n
            sb.append(tagsetName + delimiter + tagsetName + delimiter + this.name.replaceAll("_", " "));
            for (JSTagset child : children) {
                sb.append(delimiter + child.getName().replaceAll("_", " "));
            }
            sb.append("\n");
        }
    }
}
