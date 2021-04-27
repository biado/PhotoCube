import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

/**
 * Tagset is the entity that we store information of which tags belong to which tagset.
 * Currently used for the tagsets generated from LSC metadata. (Timezone, Day of week)
 */
public class Tagset {
    private static final String delimiter = ",,"; // Using 2 commas because 1) semantic_name column has some values that use ',' and 2) timestamp has ':'.
    private String tagsetName; // The top level tagset name.
    private Map<String, Set<String>> tagset_tags_map; // fx. People - [Adult, Family, Student]
    private Map<String, String> tag_tagset_map; // fx. Alex - People, cat - animal. Note the value is top tagset name.

    public Tagset(String line) {
        this.tagset_tags_map = new HashMap<>();
        this.tag_tagset_map = new HashMap<>();
        putLineInMaps(line);
    }

    /**
     * Using the given grouping information from the line, fills in the 2 Map fields.
     * @param line a line from manual grouping files. (We use manualTagsetMD.csv file)
     */
    public void putLineInMaps(String line) {
        String[] input = line.split(",");
        int key;
        int value;

        if (tagsetName == null)
            this.tagsetName = input[input.length - 1];

        if (!input[0].equals("")) {
            // fill in tag_tagset_map
            tag_tagset_map.put(input[0], tagsetName);

            // fill in tagset_tags_map

            // assigning initial pair
            key = input.length - 1;
            value = assignColumn(input, key - 1);

            // putting tagset-tag list into the map
            while ((value >= 0) && (key > value)) {
                String tagset = input[key];
                String tag = input[value];
                if (!tagset_tags_map.containsKey(tagset)) {
                    Set<String> tags = new HashSet<>();
                    tags.add(tag);
                    tagset_tags_map.put(tagset, tags);
                } else {
                    Set<String> tags = tagset_tags_map.get(tagset);
                    if (!tags.contains(tag)) {
                        tags.add(tag);
                        tagset_tags_map.put(tagset, tags);
                    }
                }

                key = assignColumn(input, key - 1);
                value = assignColumn(input, value - 1);
            }
        }

    }

    private int assignColumn(String[] input, int index) {
        // Manual grouping files have hierarchies of the LSC tags.
        // File format: Tag, ParentTag, PPTag, PPPTag, Tagset
        // The rightmost column is the highest tag.

        // Here we find the column index from the rightmost column, where the column has a value.
        while (index >= 0 && input[index].equals("")) {
            index = index-1;
        }
        return index;
    }

    /**
     * Returns the name of this tagset
     * @return the name of this tagset
     */
    public String getTagsetName() {
        return this.tagsetName;
    }

    /**
     * Returns the Map of (tagset, {tags}) pairs
     * Example: In 'Timezone' tagset, this map has (Timezone, [Europe, Asia]), (Europe, [Ireland, England, Belgium...Turkey]), (Asia, [China]), (China, [Shanghai]), (Ireland, [Dublin])... pairs.
     * @return the Map of (tagset, [tags]) pairs
     */
    public Map<String,Set<String>> getTagset_Tags_Map() {
        return this.tagset_tags_map;
    }

    /**
     * Returns the Map of (tag, tagset) pairs
     * Example: In 'Timezone' tagset, this map has (Dublin, Timezone), (Ireland, Timezone), (Shanghai, Timezone), (China, Timezone), (Europe, Timezone), (Asia, Timezone) ... pairs.
     * Note the value is top tagset name.
     * @return the Map of (tag, tagset) pairs
     */
    public Map<String,String> getTag_Tagset_Map() {
        return this.tag_tagset_map;
    }

    /**
     * Returns the hierarchy information from this tagset.
     * Format: TagsetName,,HierarchyName,,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n
     */
    @Override
    public String toString() {
        StringBuilder sb;
        Set<String> hierarchyLines = new HashSet<>(); // To avoid duplicates
        for (String tagset : tagset_tags_map.keySet()) {
            // Example: keySet = [Timezone, Europe, Asia, China, Ireland ...]
            // Note: Leaf tags (example: Dublin) are not in the keyset.
            Set<String> tags = tagset_tags_map.get(tagset); 

            // Example: Timezone,,Europe,,Asia\n
            // Example: Europe,,Ireland,,Norway,,Turkey\n
            // Example: Ireland,,Dublin\n
            sb = new StringBuilder();
            sb.append(tagsetName + delimiter + tagsetName + delimiter + tagset);
            for (String tag : tags) {
                sb.append(delimiter + tag);
            }
            sb.append("\n");
            hierarchyLines.add(sb.toString());
        }

        StringBuilder sb1 = new StringBuilder();
        for (String line : hierarchyLines) {
            sb1.append(line);
        }
        return sb1.toString();
    }
}
