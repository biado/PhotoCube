import java.util.Objects;

/**
 * JSTagset is the entity that we store information of which tags belong to which tagset. JS stands for Json, which is the input file format.
 * Currently used for the semantic tags and their hierarchies extracted using ImageNetShuffle and WordNet.
 */
public class JSTagset {
    private static final String delimiter = ",,"; // Using 2 commas because 1) semantic_name column has some values that use ',' and 2) timestamp has ':'.
    private String name;
    private Integer id; // -1 if this tag is not used as actual tag for an image. Not int but Integer to force null when writing to json file. (If int it writes 0)
    private JSTagset[] children;

    public JSTagset(String name, int id, JSTagset[] children) {
        this.name = name; // Gson doesn't seem to use this constructor, so "_" in the name has to be handled in the hierarchy string and image tag.
        this.id = id;
        this.children = children;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) {
            return true;
        }
        if (!(o instanceof JSTagset)) {
            return false;
        }
        JSTagset that = (JSTagset) o;
        return Objects.equals(this.id, that.id) && 
                Objects.equals(this.name, that.name);
    }

    @Override
    public int hashCode() {
        return Objects.hash(name, id);
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
