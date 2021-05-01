/**
 * JSTagset is the entity that we store information of which tags belong to which tagset. JS stands for Json, which is the input file format.
 * Currently used for the semantic tags and their hierarchies extracted using ImageNetShuffle and WordNet.
 */
public class JSTagset {
    private static final String delimiter = ",,"; // Using 2 commas because 1) semantic_name column has some values that use ',' and 2) timestamp has ':'.
    private String name;
    private int id; // -1 if this tag is not used as actual tag for an image
    private JSTagset[] children;

    public JSTagset(String name, int id, JSTagset[] children) {
        this.name = name; // Gson doesn't seem to use this constructor, so "_" in the name has to be handled in the hierarchy string and image tag.
        this.id = id;
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
     * Sets the name as the given parameter
     * @param newName
     */
    public void setName(String newName) {
        this.name = newName;
    }

    /**
     * Returns the children of this tagset
     * @return the children of this tagset
     */
    public JSTagset[] getChildren() {
        return this.children;
    }

    /**
     * Returns the id of this tagset
     * @return the id of this tagset
     */
    public int getId() {
        return this.id;
    }
    
    /**
     * Returns the hierarchy information from this tagset as String.
     * Format: TagsetName,,HierarchyName,,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n
     */
    public String getHierarchyString(String tagsetName) {
        StringBuilder sb = new StringBuilder();
        // case: leaf -> no hierarchy starting from this jsTagset
        if (this.children != null) { // case: parentNode -> We want to build a hierarchy string like TagsetName,,HierarchyName(==TagsetName),,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n
            sb.append(tagsetName + delimiter + tagsetName + delimiter + this.name.replaceAll("_", " "));
            for (JSTagset child : children) {
                sb.append(delimiter + child.getName().replaceAll("_", " "));
            }
            sb.append("\n");
        }
        return sb.toString();
    }
}
