import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.text.ParseException;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

/**
 * ImageTagGenerator generates imageTags.csv file from LSC data set.
 * 
 * First, it gets the solution set from SolutionListGenerator.
 * It was needed because we were experimenting with databases with different number of objects from LSC data set and wanted all the databases to contain the solution images.
 * 
 * Then it gets the (tagName, tagsetName) Map from HierarchyGenerator, and writes the hierarchy lines to a file.
 * 
 * Next step is to read in the LSC Metadata file, and create a (minute_id, metadata line) Map.
 * It is because LSC data set has a Visual Concept and Metadata file where they both have minute_id, but the number of lines are different.
 * 
 * And it generates imageTag strings line by line from the Visual Concept file.
 * Visual Concept has the image filepath and the tags associated to the image.
 * If there is minute_id corresponding to the image, it uses MetadataFormatter to also use the metadata as tags.
 */
public class ImageTagGenerator {
    private static final String delimiter = ",,";

    private Set<String> solutionFilenames;
    private Map<String, String> tag_tagset_map;
    private Map<String, String> minuteId_line_map;
    private StringBuilder solutionsInFront;
    private StringBuilder othersAtBack;
    private String[] metadataColumnNames;
    private MetadataFormatter metadataFormatter;

    private static final String LSCVisualConcept = FilepathReader.LSCVisualConcept;
    private static final String LSCmetadata = FilepathReader.LSCMetadata;
    private static final String outputPath = FilepathReader.LSCImageTagsOutput;

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

    /**
     * Reads in LSC Visual Concept file, and process line by line.
     * @param brVC BufferedReader that reads in Visual Concept file
     * @throws IOException
     * @throws ParseException
     */
    public void buildStrings(BufferedReader brVC) throws IOException, ParseException {
        String line = brVC.readLine(); // Skip the first line
        while ((line = brVC.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            StringBuilder sb = getCorrectStringBuilder(input[2]); // Make sure to put the solution images in the beginning of the output file.

            // File format: "FileName,,TagSet,,Tag,,TagSet,,Tag,,(...)"
            sb.append(makeImagePath(input[2]));
            sb.append(makeTagsFromVisualConceptAttributes(input));
            sb.append(makeTagsFromVisualConceptConcepts(input));
            String minuteID = input[0];
            if (minuteId_line_map.containsKey(minuteID)) { // If the minute_id is in Metadata file, then make tags from metadata.
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
        // LSCVisualConcept's image_path column looks like this: DATASETS/LSC2020/2015-02-23/b00000e.jpg
        // We want to store only '2015-02-23/b00000e.jpg' in the ImageTags.csv file.
        // Decided to concatenate "[Image Server address]\\lsc2020\\" in the PhotoCube client code.

        // (Windows) java uses \\, but C# uses \

        String newImagePath = imagePath.substring(17);
        Path newPath = Paths.get(newImagePath);
        return newPath.toString();
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
                sb.append(delimiter + tagset + delimiter + tag);
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

    /**
     * Writes the fileName, tagset, tag information to imageTags.csv file.
     * The path to output file is to be specified in config.properties file.
     */
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
            // String pathString = ImageTagGenerator.makeImagePath("DATASETS/LSC2020/2015-02-23/b00000e.jpg");
            // System.out.println(pathString);
            ImageTagGenerator itg = new ImageTagGenerator();
            itg.writeToImageTagFile();
            
            System.out.println("Done.");

            //TODO: other columns in Visual Concept?

        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
