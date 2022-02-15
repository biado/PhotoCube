import java.util.Objects;

/**
 * A JSTag represents a tag in the JSON hierarchy input file. JS stands for Json, which is the input file format. A JSTag stores information about the structure of the hierarchy by storing an array of child tags.
 * Currently used for the semantic tags and their hierarchies extracted using ImageNetShuffle and WordNet.
 */
public class JSTag {
    private String name;
    private Integer id; // -1 if this tag is not used as actual tag for an image. Not int but Integer to force null when writing to json file. (If int it writes 0)
    private JSTag[] children;

    public JSTag(String name, int id, JSTag[] children) {
        this.name = name; // Gson doesn't seem to use this constructor, so "_" in the name has to be handled in the hierarchy string and image tag.
        this.id = id;
        this.children = children;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) {
            return true;
        }
        if (!(o instanceof JSTag)) {
            return false;
        }
        JSTag that = (JSTag) o;
        return Objects.equals(this.id, that.id) && 
                Objects.equals(this.name, that.name);
        // We only consider name and id for tags to be equal.
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
    public JSTag[] getChildren() {
        return this.children;
    }

    /**
     * Returns the id of this tagset
     * @return the id of this tagset
     */
    public int getId() {
        return this.id;
    }
}
