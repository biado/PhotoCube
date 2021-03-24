import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.text.ParseException;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

public class ImageTagGenerator {
    private static final String delimiter = ",,";

    private Set<String> solutionFilenames;
    private Map<String, String> tag_tagset_map;
    private Map<String, String> minuteId_line_map;
    private StringBuilder solutionsInFront;
    private StringBuilder othersAtBack;
    private String[] metadataColumns;

    private static final String visualConcept = "C:\\lsc2020\\lsc2020_visual_concepts\\lsc2020-visual-concepts.csv";
    private static final String LSCmetadata = "C:\\lsc2020\\lsc2020-metadata\\lsc2020-metadata.csv";
    private static final String outputPath = "C:\\lsc2020\\tags-and-hierarchies\\lscImageTags_Range.csv";

    public ImageTagGenerator() throws IOException, ParseException {
        this.solutionFilenames = new SolutionListGenerator().getSolutionSet();
        this.tag_tagset_map = new HierarchyGenerator().buildAndGetTag_Tagset_Map();
        this.solutionsInFront = new StringBuilder();
        this.othersAtBack = new StringBuilder();
        BufferedReader brVC = new BufferedReader(new FileReader(new File(visualConcept)));
        BufferedReader brMD = new BufferedReader(new FileReader(new File(LSCmetadata)));
        this.minuteId_line_map = buildMinuteID_Line_Map(brMD);
        buildStrings(brVC);
    }

    private Map<String,String> buildMinuteID_Line_Map(BufferedReader brMD) throws IOException {
        Map<String,String> minuteId_line_map = new HashMap<>();
        String line = brMD.readLine();
        this.metadataColumns = line.split(","); // Store the first line
        while ((line = brMD.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            minuteId_line_map.put(input[0], line);
        }
        return minuteId_line_map;
    }

    public void buildStrings(BufferedReader brVC) throws IOException, ParseException {
        // read in Visual Concept file and process line by line.
        String line = brVC.readLine(); // Skip the first line
        while ((line = brVC.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            StringBuilder sb = getCorrectStringBuilder(input[2]);
            // File format: "FileName:TagSet:Tag:TagSet:Tag:(...)"
            // Make sure to put the correct filepath in front of the filename.
            sb.append(makeImagePath(input[2]));
            sb.append(makeTagsFromVisualConceptAttributes(input));
            sb.append(makeTagsFromVisualConceptConcepts(input));
            String minuteID = input[0];
            if (minuteId_line_map.containsKey(minuteID)) {
                String metadataLine = minuteId_line_map.get(minuteID);
                sb.append(makeTagsFromMetadata(metadataLine));
            }
            sb.append("\n");
        }
    }

    private StringBuilder getCorrectStringBuilder(String imagePath) {
        // First, check if filename is in the solution. if yes -> frontStringBuilder, no -> backStringBuilder.
        // Because we want the solution files in the front of the image-tag file so that it is always included to different databases.

        String filename = imagePath.substring(28);
        if (solutionFilenames.contains(filename)) {
            return solutionsInFront;
        } else {
            return othersAtBack;
        }
    }

    private String makeImagePath(String imagePath) {
        // Note: Need to add the correct path when using the output file in C#. (fx. Jihye has to add C:/ in the beginning.)
        // java uses \\, but C# uses \
        // Considering PhotoCube is written in C#, make filepath using \
        // String newImagePath = "lsc2020\\" + imagePath.substring(17).replace("/","\\");

        // Decided to concatenate "[Image Server address]\\lsc2020\\" in the PhotoCube client code.
        String newImagePath = imagePath.substring(17).replace("/","\\");
        return newImagePath;
    }

    private String makeTagsFromVisualConceptAttributes(String[] input) {
        // tag 10 attributes, using tag_tagset_map. (probability of the tags are ignored.)
        // put all 10 tags in a set to remove duplicate
        Set<String> attributes = new HashSet<>();
        for (int i=3; i<13; i++) {
            if (!input[i].equals("NULL")) {
                attributes.add(input[i]);
            }
        }
        // then search the tagset and add to sb.
        StringBuilder sb = new StringBuilder();
        for (String tag : attributes) {
            String tagset = tag_tagset_map.get(tag);
            if (tagset != null) {
                sb.append(delimiter + tagset + delimiter + tag);
            }
        }
        return sb.toString();
    }

    private String makeTagsFromVisualConceptConcepts(String[] input) {
        // tag 10 concepts, using tag_tagset_map. (probability of the tags are ignored.)
        // put all 10 tags in a set to remove duplicate
        Set<String> attributes = new HashSet<>();
        for (int i=23; i<54; i=i+3) {
            if (!input[i].equals("NULL")) {
                attributes.add(input[i]);
            }
        }
        // then search the tagset and add to sb.
        StringBuilder sb = new StringBuilder();
        for (String tag : attributes) {
            String tagset = tag_tagset_map.get(tag);
            if (tagset != null) {
                sb.append(delimiter + tagset + delimiter + tag); // After we made [attribute, concept, metadata] tagsets as the highest, this just becomes adding ":Concept:tag"
            }
        }
        return sb.toString();
    }

    private String makeTagsFromMetadata(String metadataLine) throws ParseException {
        String[] input = metadataLine.split(",");
        input = sanitizeInput(input);
        StringBuilder sb = new StringBuilder();

        // utc_time
        String[] utc_time = input[1].split("_");
        String[] ymd = utc_time[1].split("-");
        sb.append(delimiter+ metadataColumns[1] + delimiter + ymd[0]); // Year
        sb.append(delimiter+ metadataColumns[1] + delimiter + Tagset.getMonth(input[1].substring(4), "UTC")); // Month
        sb.append(delimiter+ metadataColumns[1] + delimiter + ymd[2]); // Date
        sb.append(delimiter+ metadataColumns[1] + delimiter + utc_time[2].replace(":", ".")); // Timestamp
        sb.append(delimiter+ metadataColumns[1] + delimiter + Tagset.getDay(input[1].substring(4), "UTC")); // Day

        // local_time
        String[] local_time = input[2].split("_");
        String[] ymdLocal = local_time[0].split("-");
        sb.append(delimiter+ metadataColumns[2] + delimiter + ymdLocal[0]); // Year
        sb.append(delimiter+ metadataColumns[2] + delimiter + Tagset.getMonth(input[2], input[3])); // Month
        sb.append(delimiter+ metadataColumns[2] + delimiter + ymdLocal[2]); // Date
        sb.append(delimiter+ metadataColumns[2] + delimiter + local_time[1].replace(":", ".")); // Timestamp
        sb.append(delimiter+ metadataColumns[2] + delimiter + Tagset.getDay(input[2], input[3])); // Day

        // Everything else
        for (int i = 3; i < input.length; i++) {
            if (!input[i].equals("NULL")) {
                sb.append(delimiter+ metadataColumns[i] + delimiter + input[i]);
            }
        }
        return sb.toString();
    }

    private String[] sanitizeInput(String[] input) {
        if (input.length != 13) { // comma(,) in the 6th column. input[] length == 14
            String[] sanitized = new String[13];
            for(int i = 0; i<6; i++) {
                sanitized[i] = input[i];
            }
            sanitized[6] = String.join(",", input[6], input[7]).replace(", ","-");
            for(int i=7; i<sanitized.length; i++) {
                sanitized[i] = input[i+1];
            }
            return sanitized;
        } else {
            return input;
        }
    }

    public void writeToImageTagFile() {
        // write to file: frontStringBuilder first (has solutions in) -> then backStringBuilder
        System.out.println("Started writing tags into the image tag file.");
            try {
                BufferedWriter writer = new BufferedWriter(new FileWriter(outputPath));
                writer.write("FileName,,TagSet,,Tag,,TagSet,,Tag,,(...)\n"); // File format
                writer.write(this.solutionsInFront.toString());
                writer.write(this.othersAtBack.toString());
                writer.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
    }

    public static void main(String[] args) throws ParseException {
        try {
            ImageTagGenerator itg = new ImageTagGenerator();
            itg.writeToImageTagFile();
            
            System.out.println("Done.");

            // TODO: other columns in Visual Concept?
            
        } catch (IOException e) {
            e.printStackTrace();
        }
        
    }
}
