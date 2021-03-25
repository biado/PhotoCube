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
    private String[] metadataColumnNames;
    private MetadataFormatter metadataFormatter;

    private static final String LSCVisualConcept = "C:\\lsc2020\\lsc2020_visual_concepts\\lsc2020-visual-concepts.csv";
    private static final String LSCmetadata = "C:\\lsc2020\\lsc2020-metadata\\lsc2020-metadata.csv";
    private static final String outputPath = "C:\\lsc2020\\tags-and-hierarchies\\lscImageTags_with_Metadata.csv";

    public ImageTagGenerator() throws IOException, ParseException {
        this.solutionFilenames = new SolutionListGenerator().getSolutionSet();
        HierarchyGenerator hg = new HierarchyGenerator();
        this.tag_tagset_map = hg.buildAndGetTag_Tagset_Map();
        hg.writeToHierarchyFile();
        this.solutionsInFront = new StringBuilder();
        this.othersAtBack = new StringBuilder();
        BufferedReader brVC = new BufferedReader(new FileReader(new File(LSCVisualConcept)));
        BufferedReader brMD = new BufferedReader(new FileReader(new File(LSCmetadata)));
        this.minuteId_line_map = buildMinuteID_Line_Map(brMD);
        buildStrings(brVC);
    }

    private Map<String,String> buildMinuteID_Line_Map(BufferedReader brMD) throws IOException {
        Map<String,String> minuteId_line_map = new HashMap<>();
        String line = brMD.readLine();
        String[] metadataColumns = line.split(",");
        this.metadataFormatter = new MetadataFormatter(metadataColumns);
        this.metadataColumnNames = metadataFormatter.formatMetadataColumnNames(metadataColumns);  // Store the first line

        while ((line = brMD.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            minuteId_line_map.put(input[0], line);
        }
        return minuteId_line_map;
    }

    // private String[] storeColumnNamesInFirstLetterUppercase(String line) {
    //     String[] lowercaseColumnNames = line.split(",");
    //     String[] firstLetterUppercaseColumnNames = new String[lowercaseColumnNames.length];
    //     for (int i = 0; i < lowercaseColumnNames.length; i++) {
    //         String lowercaseColumnName = lowercaseColumnNames[i];
    //         String firstLetterUppercaseColumnName = setFirstUppercase(lowercaseColumnName);
    //         firstLetterUppercaseColumnNames[i] = firstLetterUppercaseColumnName;
    //     }
    //     return firstLetterUppercaseColumnNames;
    // }

    // private String setFirstUppercase(String s) {
    //     return s.substring(0, 1).toUpperCase() + s.substring(1).toLowerCase();
    // }

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
        String[] formattedMetadataLine = metadataFormatter.formatMetadataLine(input);
        StringBuilder sb = new StringBuilder();

        // Ignore minute_id (i=0) and utc_time (i=1)

        // local_time (i=2) is broken down into Date & Time
        String[] date_time = formattedMetadataLine[2].split("_");
        sb.append(delimiter + "Date" + delimiter + date_time[0]);
        sb.append(delimiter + "Time" + delimiter + date_time[1]);

        // timezone (i=3) only use the city name as the tag (Europe/Dublin -> Timezone,,Dublin)
        String[] region_city = formattedMetadataLine[3].split("/");
        sb.append(delimiter + metadataColumnNames[3] + delimiter + region_city[1]);

        // Everything else
        for (int i = 4; i < formattedMetadataLine.length; i++) {
            if (!formattedMetadataLine[i].equals("NULL")) {
                sb.append(delimiter+ metadataColumnNames[i] + delimiter + formattedMetadataLine[i]);
            }
        }
        return sb.toString();
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
