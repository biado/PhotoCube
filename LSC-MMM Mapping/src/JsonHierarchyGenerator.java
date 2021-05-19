import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import com.google.gson.*;

/**
 * JsonHierarchyGenerator keeps track of all the hierarchies for semantic tags in json, including their semantic duplicates. Semantic tags have been derived from the ImageNet Shuffle and WordNet.
 */
public class JsonHierarchyGenerator {
    private static final String jsonOutput = FilepathReader.UniqueTagHierarchy;
    private static final String jsonFile = FilepathReader.JsonHierarchy;
    private Gson g;
    private JSTagset root;
    private Map<String, String> tag_tagset_map; // fx. Alex - People, cat - animal. Note the value is top tagset name. No "_" in the tagName.
    private Map<String, List<JSTagset>> tagName_duplicateTagsetList_map; // fx. (white, < white-person, white-color ...>) as an entry. No "_" in the tagName.
    private Set<String> homonyms; // If there were more than 1 entry in tagName_duplicateTagsetSet_map, we put the tagname here to check if it had duplicates. 'white', not 'white(id)'. No "_" in the tagName.

    public JsonHierarchyGenerator() throws FileNotFoundException {
        BufferedReader br = new BufferedReader(new FileReader(new File(jsonFile)));
        this.g = new Gson();
        this.root = g.fromJson(br, JSTagset.class);
        this.tagName_duplicateTagsetList_map = new HashMap<>();
        System.out.println("Started building tagname - Set<JSTagset> map.");
        buildTagNameDuplicateTagsetsMapRecursive(root);
        this.homonyms = new HashSet<>();
        changeTagNameOfHomonyms();
        this.tag_tagset_map = new HashMap<>();
        buildTagTagsetMap();
    }

    /**
     * Writes the hierarchy tree information in the WordNet Hierarchy to a json file.
     * The path to output file is to specified in config.properties file.
     */
    public void writeToJsonFile() {
        Gson gson = new GsonBuilder().setPrettyPrinting().create();
        try (FileWriter writer = new FileWriter(jsonOutput)) {
            gson.toJson(this.root, writer);
        } catch (IOException e) {
            e.printStackTrace();;
        }
    }

    private void buildTagNameDuplicateTagsetsMapRecursive(JSTagset current) {
        // traverse children of "root" tree and put each JStagsets into tagname-tagsetSet map
        // This method also cleans up the name, such that it replaces "_" to " ".
        String tagsetName = StringBeautifier.toPrettyTagsetName(current);
        current.setName(tagsetName);
        putInTagNameDuplicateTagsetListMap(tagsetName, current);
        if (current.getChildren() != null) { // if it has children (not a leaf)
            for (JSTagset child : current.getChildren()) {
                buildTagNameDuplicateTagsetsMapRecursive(child);
            }
        }
    }

    private void putInTagNameDuplicateTagsetListMap(String tagsetName, JSTagset current) {
        List<JSTagset> tagsets = (this.tagName_duplicateTagsetList_map.containsKey(tagsetName)) ? tagName_duplicateTagsetList_map.get(tagsetName) : new ArrayList<>();
        tagsets.add(current);
        tagName_duplicateTagsetList_map.put(tagsetName, tagsets);
    }

    private void changeTagNameOfHomonyms() {
        System.out.println("Started making duplicate tag names unique, by concatenating id to the name.");
        for (String tagName : tagName_duplicateTagsetList_map.keySet()) {
            List<JSTagset> tagsets = tagName_duplicateTagsetList_map.get(tagName); // The list can have many tagsets that has same name and id, but different memory locations.
            if (new HashSet<>(tagsets).size() > 1) { // Put the list in set, so the 'equal' tagset duplicates are removed.
            // More than 1 tagsets in this hashset for a tagname means there are semantic duplicates (homonyms).
                homonyms.add(tagName);
                for (JSTagset jsTagset : tagsets) { // Looping the list, not the set. Because we want to change the name for the tagsets in different memory location with the same name.
                    String newName = jsTagset.getName() + "(" + jsTagset.getId() + ")";
                    jsTagset.setName(newName); // now the tagset name is name(id)
                }
            }
        }
    }

    /**
     * Returns the homonyms set. (The tag that had more than 2 meanings.)
     * @return the homonyms set
     */
    public Set<String> getHomonyms() {
        return this.homonyms;
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
        // ROOT - [timezone, day of week, enity]
        // We do not include "ROOT"
        for (JSTagset child : this.root.getChildren()) {
            String tagsetName = child.getName().replaceAll("_", " ");
            buildTagTagsetMapRecursive(tagsetName, child);
        }
    }

    private void buildTagTagsetMapRecursive(String tagsetName, JSTagset current) {
        this.tag_tagset_map.put(current.getName().replaceAll("_", " "), tagsetName);
        if (current.getChildren() != null) {
            for (JSTagset child : current.getChildren()) {
                buildTagTagsetMapRecursive(tagsetName, child);
            }
        }
    }

    public static void main(String[] args) throws IOException {
        JsonHierarchyGenerator jshg = new JsonHierarchyGenerator();
        List<JSTagset> fathers = jshg.tagName_duplicateTagsetList_map.get("Father");
        Set<JSTagset> fatherset = new HashSet<>(fathers);
        System.out.println(jshg.getHomonyms().contains("Father"));
        for (JSTagset jsTagset : fathers) {
            System.out.println(jsTagset.getName() + ": " + jsTagset.getId());
        }
    }
}
