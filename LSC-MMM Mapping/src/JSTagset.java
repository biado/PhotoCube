public class JSTagset {
    public String name;
    public JSTagset[] children;

    public JSTagset(String name, JSTagset[] children) {
        this.name = name;
        this.children = children;
    }
}
