import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import com.google.gson.*;

/**
 * HierarchyGenerator keeps track of all the hierarchies and tagsets for semantic tags, derived from the ImageNet Shuffle and WordNet.
 */
public class JsonHierarchyGenerator {
    private static final String outputPath = FilepathReader.LSCHierarchiesOutput;
    private static final String jsonFile = FilepathReader.JsonHierarchy;
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
     * @return the tag_tagset_map
     */
    public Map<String, String> getTag_tagset_map() {
        if (tag_tagset_map.isEmpty()) {
            buildTagTagsetMap();
        }
        return tag_tagset_map;
    }

    private void buildTagTagsetMap() {
        String tagsetName = this.root.getName();
        buildTagTagsetMapRecursive(tagsetName, this.root);
    }

    private void buildTagTagsetMapRecursive(String tagsetName, JSTagset current) {
        this.tag_tagset_map.put(current.getName().replaceAll("_", " "), tagsetName);
        if (current.getChildren() != null) {
            for (JSTagset child : current.getChildren()) {
                buildTagTagsetMapRecursive(tagsetName, child);
            }
        }
    }

    /**
     * Returns all the hierarchy information of all tagsets WordNet Hierarchy json file (hierarchy.1.0.json).
     * Format: TagsetName,,HierarchyName,,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n
     */
    public String buildHierarchyString() {
        // We decided to have "entity" as a tagset/rootNode of all tags.
        Set<String> hierarchyStrings = new HashSet<>();
        buildHierarchyStringRecursive(hierarchyStrings, this.root.getName(), this.root);
        StringBuilder sb = new StringBuilder();
        for (String hierarchyString : hierarchyStrings) {
            sb.append(hierarchyString);
        }
        return sb.toString();
    }

    private void buildHierarchyStringRecursive(Set<String> hierarchyStrings, String tagsetName, JSTagset current) {
        if (current.getChildren() != null) {
            String hierarchyString = current.getHierarchyString(tagsetName);
            hierarchyStrings.add(hierarchyString);
            for (JSTagset child : current.getChildren()) {
                buildHierarchyStringRecursive(hierarchyStrings, tagsetName, child);
            }
        }
    }

    /**
     * Writes the hierarchy information in the WordNet Hierarchy to a file.
     * The path to output file is to specified in config.properties file.
     */
    public void writeToHierarchyFile() {
        System.out.println("Started writing json tags (visual concepts) into the hierarchy file.");
            try {
                BufferedWriter writer = new BufferedWriter(new FileWriter(outputPath, true));
                writer.write(buildHierarchyString());
                writer.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
    }

    public String findNumDuplicateTags() {
        Set<String> tags = new HashSet<>();
        StringBuilder sb = new StringBuilder();
        findNumDuplicateTagsRecursive(sb, tags, "root", this.root);
        return sb.toString();
    }

    private void findNumDuplicateTagsRecursive(StringBuilder sb, Set<String> tags, String parentName, JSTagset current) {
        String currentTagName = current.getName();
        if(tags.contains(currentTagName)) {
            sb.append(parentName + " : " + currentTagName + "\n");
        } else {
            tags.add(currentTagName);
        }
        if (current.getChildren() != null) {
            for (JSTagset child : current.getChildren()) {
                findNumDuplicateTagsRecursive(sb, tags, currentTagName, child);
            }
        }
    }

    public static void main(String[] args) throws IOException {
        JsonHierarchyGenerator jshg = new JsonHierarchyGenerator();
        BufferedWriter writer = new BufferedWriter(new FileWriter("C:\\lsc2020\\tags-and-hierarchies\\duplicatesFromJson.txt"));
        writer.write(jshg.findNumDuplicateTags());
        writer.close();
        // System.out.println(jshg.buildHierarchyString());
        // for (JSTagset child : jshg.root.getChildren()) {
        //     System.out.println(child.getName());
        // }
        // jshg.writeToHierarchyFile();

        // Map<String, String> tag_tagset_map = jshg.getTag_tagset_map();
        // tag_tagset_map.forEach((k,v) -> System.out.println(k + " : " + v));
    }
}
