import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.text.ParseException;
import java.util.HashMap;
import java.util.Map;

/**
 * HierarchyGenerator keeps track of all the hierarchies and tagsets derived from the LSC Metadata.
 */
public class HierarchyGenerator {
    private Map<String,Tagset> tagsets; // All the tagsets derived from LSC Metadata. (tagsetName, Tagset)

    private static final String manualTagSetsMD = FilepathReader.manualTagSetsMD;
    private static final String outputPath = FilepathReader.LSCHierarchiesOutput;
    
    public HierarchyGenerator() throws IOException, ParseException {
        this.tagsets = new HashMap<>();
        BufferedReader br = new BufferedReader(new FileReader(new File(manualTagSetsMD)));
        buildTagsetsMap(br);
    }

    private void buildTagsetsMap(BufferedReader br) throws IOException {
        String line = br.readLine(); // Skip the first line
        while ((line = br.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            String tagsetName = input[input.length-1];
            if (tagsets.containsKey(tagsetName)) {
                Tagset tagset = tagsets.get(tagsetName);
                tagset.putLineInMaps(line);
            } else {
                Tagset tagset = new Tagset(line); // putLineInMaps(line) is called within the Tagset constructor.
                tagsets.put(tagsetName, tagset);
            }
        }
    }

    /**
     * Returns all the hierarchy information of all tagsets in the LSC Metadata.
     * Format: TagsetName,,HierarchyName,,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n
     */
    @Override
    public String toString() {
        // build string with proper format
        // # Format: TagsetName,,HierarchyName,,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)
        StringBuilder sb = new StringBuilder();
        sb.append("# Format: TagsetName,,HierarchyName,,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n");
        for (String tagsetName : tagsets.keySet()) {
            sb.append(tagsets.get(tagsetName));
        }
        
        return sb.toString();
    }

    /**
     * Writes the hierarchy information in the LSC Metadata to a file.
     * The path to output file is to specified in config.properties file.
     */
    public void writeToHierarchyFile() {
        System.out.println("Started writing tags into the hierarchy file.");
            try {
                BufferedWriter writer = new BufferedWriter(new FileWriter(outputPath));
                writer.write(this.toString());
                writer.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
    }

    /**
     * Returns the Map of (tagsetName, Tagset) pairs
     * @return the Map of (tagsetName, Tagset) pairs
     */
    public Map<String,Tagset> getTagsets() {
        return this.tagsets;
    }

    /**
     * Builds and returns the Map of all (tagName, tagsetName) pairs in the LSC Metadata.
     * @return the Map of (tagName, tagsetName) pairs
     */
    public Map<String,String> buildAndGetTag_Tagset_Map() {
        Map<String,String> map = new HashMap<>();
        tagsets.forEach((k,v) -> map.putAll(v.getTag_Tagset_Map()));
        return map;
    }

    public static void main(String[] args){
        System.out.println("Started generating Hierarchies.");
        try {
            HierarchyGenerator hg = new HierarchyGenerator();
            // hg.buildAndGetTag_Tagset_Map();
            hg.writeToHierarchyFile();
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException ie) {
            ie.printStackTrace();
        } catch (ParseException pe) {
            pe.printStackTrace();
        }

        System.out.println("Done.");
    }
}
