public class JSTagset {
    private static final String delimiter = ",,"; // Using 2 commas because 1) semantic_name column has some values that use ',' and 2) timestamp has ':'.
    private String name;
    private JSTagset[] children;

    public JSTagset(String name, JSTagset[] children) {
        this.name = name;
        this.children = children;
    }

    public String getName() {
        return this.name;
    }

    public JSTagset[] getChildren() {
        return this.children;
    }
    
    public void getHierarchyString(StringBuilder sb, String tagsetName) {
        // case: leaf -> no hierarchy starting from this jsTagset
        if (this.children != null) { // case: parentNode -> We want to build a hierarchy string like TagsetName,,HierarchyName(==TagsetName),,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n
            sb.append(tagsetName + delimiter + tagsetName + delimiter + this.name);
            for (JSTagset child : children) {
                sb.append(delimiter + child.getName());
            }
            sb.append("\n");
        }
    }
}
