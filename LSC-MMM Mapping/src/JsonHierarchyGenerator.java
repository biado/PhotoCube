import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import com.google.gson.*;

public class JsonHierarchyGenerator {
    private static final String outputPath = FilepathReader.LSCHierarchiesOutput;
    private static final String jsonFile = FilepathReader.DraftJson;
    private Gson g;
    private JSTagset root;
    private Map<String, String> tag_tagset_map; // fx. Alex - People, cat - animal. Note the value is top tagset name.

    
    public JsonHierarchyGenerator() throws FileNotFoundException {
        BufferedReader br = new BufferedReader(new FileReader(new File(jsonFile)));
        this.g = new Gson();
        root = g.fromJson(br, JSTagset.class);
        tag_tagset_map = new HashMap<>();
        buildTagTagsetMap();
    }

    /**
     * Returns the tag_tagset_map.
     * Entry examples: (Alex, People) (Cat, Animal)
     * Note that the value is top tagset name.
     * @return
     */
    public Map<String, String> getTag_tagset_map() {
        if (tag_tagset_map.isEmpty()) {
            buildTagTagsetMap();
        }
        return tag_tagset_map;
    }

    private void buildTagTagsetMap() {
        JSTagset[] roots = this.root.getChildren();
        for (JSTagset jsTagset : roots) {
            String tagsetName = jsTagset.getName();
            buildTagTagsetMapRecursive(tagsetName, jsTagset);
        }
    }

    private void buildTagTagsetMapRecursive(String tagsetName, JSTagset current) {
        this.tag_tagset_map.put(current.getName(), tagsetName);
        if (current.getChildren() != null) {
            for (JSTagset child : current.getChildren()) {
                buildTagTagsetMapRecursive(tagsetName, child);
            }
        }
    }

    public String buildHierarchyString() {
        // We decided not to have "root" as a tagset, but the children of root as tagsets.
        JSTagset[] roots = this.root.getChildren();
        StringBuilder sb = new StringBuilder();
        for (JSTagset jsTagset : roots) {
            String tagsetName = jsTagset.getName();
            buildHierarchyStringRecursive(sb, tagsetName, jsTagset);
        }
        return sb.toString();
    }

    private void buildHierarchyStringRecursive(StringBuilder sb, String tagsetName, JSTagset current) {
        if (current.getChildren() != null) {
            current.getHierarchyString(sb, tagsetName);
            for (JSTagset child : current.getChildren()) {
                buildHierarchyStringRecursive(sb, tagsetName, child);
            }
        }
    }

    /**
     * Writes the hierarchy information in the LSC data set to a file.
     * The path to output file is to specified in config.properties file.
     */
    public void writeToHierarchyFile() {
        System.out.println("Started writing json tags (visual concepts) into the hierarchy file.");
            try {
                BufferedWriter writer = new BufferedWriter(new FileWriter(outputPath));
                writer.write(buildHierarchyString());
                writer.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
    }

    public static void main(String[] args) throws FileNotFoundException {
        JsonHierarchyGenerator jshg = new JsonHierarchyGenerator();
        // System.out.println(jshg.buildHierarchyString());
        // for (JSTagset child : jshg.root.getChildren()) {
        //     System.out.println(child.getName());
        // }
        // jshg.writeToHierarchyFile();

        Map<String, String> tag_tagset_map = jshg.getTag_tagset_map();
        tag_tagset_map.forEach((k,v) -> System.out.println(k + " : " + v));
    }
}
