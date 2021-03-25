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

public class HierarchyGenerator {
    private Map<String,Tagset> tagsets;

    private static final String manualTagSetsVC = "C:\\lsc2020\\tags-and-hierarchies\\manual-grouping-scenes-objects.csv";
    private static final String manualTagSetsMD = "C:\\lsc2020\\tags-and-hierarchies\\manual-grouping-metadata.csv";
    private static final String LSCmetadata = "C:\\lsc2020\\lsc2020-metadata\\lsc2020-metadata.csv";
    private static final String outputPath = "C:\\lsc2020\\tags-and-hierarchies\\lscHierarchies_with_Timezone.csv";
    
    public HierarchyGenerator() throws IOException, ParseException {
        this.tagsets = new HashMap<>();
        BufferedReader br = new BufferedReader(new FileReader(new File(manualTagSetsVC)));
        buildTagsetsMap(br);
        br = new BufferedReader(new FileReader(new File(manualTagSetsMD)));
        buildTagsetsMap(br);
        // br = new BufferedReader(new FileReader(new File(LSCmetadata)));
        // readMetaData(br);
        // TODO: uppercase, lowercase, lat/lon -> latitude/longitude, _ -> " "
    }

    private void readMetaData(BufferedReader br) throws IOException, ParseException { // adding leaves to hierarchy
        String[] columns = br.readLine().split(","); // first line = column names
        String line;
        while ((line = br.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            input = sanitizeInput(input);
            
            Tagset utc_time = tagsets.get("utc_time");
            utc_time.extendTimeHierarchy(input[1].substring(4), columns[1], "UTC"); // utc_time
            Tagset local_time = tagsets.get("local_time");
            local_time.extendTimeHierarchy(input[2], columns[2], input[3]); // local_time
            
            for (int i = 3; i<columns.length; i++) {
                Tagset column_Tagset = tagsets.get(columns[i]);
                column_Tagset.extendHierarchy(input[i], columns[i]);
            }
        }
    }

    public static String[] sanitizeInput(String[] input) {
        if (input.length != 13) { // comma(,) in the 6th column. input[] length == 14
            String[] sanitized = new String[13];
            for(int i = 0; i<6; i++) {
                sanitized[i] = input[i];
            }
            sanitized[6] = String.join(",", input[6], input[7]);
            for(int i=7; i<sanitized.length; i++) {
                sanitized[i] = input[i+1];
            }
            return sanitized;
        } else {
            return input;
        }
    }

    public void buildTagsetsMap(BufferedReader br) throws IOException {
        String line = br.readLine(); // Skip the first line
        while ((line = br.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            String tagsetName = input[input.length-1];
            if (tagsets.containsKey(tagsetName)) {
                Tagset tagset = tagsets.get(tagsetName);
                tagset.putLineInMaps(line);
            } else {
                Tagset tagset = new Tagset(line);
                tagsets.put(tagsetName, tagset);
            }
        }
    }

    @Override
    public String toString() {
        // build string with proper format
        // # Format: TagsetName:HierarchyName:ParrentTagName:ChildTag:ChildTag:ChildTag:(...)
        // Note: maximum height of the tree is 2.
        StringBuilder sb = new StringBuilder();

        sb.append("# Format: TagsetName,,HierarchyName,,ParrentTagName,,ChildTag,,ChildTag,,ChildTag,,(...)\n");
        for (String tagsetName : tagsets.keySet()) {
            sb.append(tagsets.get(tagsetName));
        }
        
        return sb.toString();
    }

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

    public Map<String,Tagset> getTagsets() {
        return this.tagsets;
    }

    public Map<String,String> buildAndGetTag_Tagset_Map() {
        Map<String,String> map = new HashMap<>();
        tagsets.forEach((k,v) -> map.putAll(v.getTag_Tagset_Map()));
        // map.forEach((k,v) -> System.out.println(k + " : " + v));
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
