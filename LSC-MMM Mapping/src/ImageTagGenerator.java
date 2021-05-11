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
import java.util.List;
import java.util.Map;
import java.util.Set;

/**
 * ImageTagGenerator generates imageTags.csv file from LSC data set.
 * 
 * First, it gets the solution set from SolutionListGenerator.
 * It was needed because we were experimenting with databases with different number of objects from LSC data set and wanted all the databases to contain the solution images.
 * If the filename is in solutionFilenames, we will append the imageTag lines in the front stringBuilder, otherwise in the back stringBuilder.
 * 
 * Then it generates hierarchies from the JsonHierarchyGenerator, and writes the hierarchy with new names(semantic duplicates handled) to a json file.
 * From JsonHierarchyGenerator, it gets the (tagName, tagsetName) Map.
 * 
 * Next step is to read in the LSC Filenames, Visual Concept and Metadata file, and create a (filename, metadata line) Map.
 * Not all the LSC images have metadata, as there are fewer lines in Metadata.csv file compared to LSC Filenames or VisualConcept.csv file.
 * We find the correct metadata line by finding the matching lines that has same 'minute_id' between VisualConcept and Metadata file.
 * 
 * And it generates imageTag strings for each LSC filename.
 * Using the featureFinder, we generate semantic tags.
 * Using the metadataFormatter, we generate metadata tags.
 */
public class ImageTagGenerator {
    private static final String delimiter = ",,";

    private Set<String> solutionFilenames;
    private Map<String, String> json_tag_tagset_map;
    private StringBuilder solutionsInFront;
    private StringBuilder othersAtBack;
    private String[] metadataColumnNames;
    private MetadataFormatter metadataFormatter;
    private FeatureFinder featureFinder;
    private Set<String> filenames;
    private Map<String, String> filename_metadataLine_map; // built by matching minute_id in Visual Concept and Metadata files.

    private static final String LSCFilename = FilepathReader.LSCFilename;
    private static final String LSCVisualConcept = FilepathReader.LSCVisualConcept; // Still needed, merely to check minute_id
    private static final String LSCmetadata = FilepathReader.LSCMetadata;
    private static final String outputPath = FilepathReader.LSCImageTagsOutput;

    public ImageTagGenerator() throws IOException, ParseException {
        this.solutionFilenames = new SolutionListGenerator().getSolutionSet();
        JsonHierarchyGenerator jshg = new JsonHierarchyGenerator();
        jshg.writeToJsonFile();
        this.json_tag_tagset_map = jshg.getTag_tagset_map();
        this.featureFinder = new FeatureFinder(jshg.getHomonyms());
        this.solutionsInFront = new StringBuilder();
        this.othersAtBack = new StringBuilder();
        buildFilenameSet();
        buildFilename_MetadataLineMap();
        buildStrings();
    }

    private void buildFilename_MetadataLineMap() throws IOException {
        this.filename_metadataLine_map = new HashMap<>();
        BufferedReader brVC = new BufferedReader(new FileReader(new File(LSCVisualConcept)));
        BufferedReader brMD = new BufferedReader(new FileReader(new File(LSCmetadata)));
        Map<String,String> minuteId_line_map = buildMinuteID_Line_Map(brMD);
        String line = brVC.readLine(); // Skip the first line
        while ((line = brVC.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            String filename = makeImagePathFromVisualConcept(input[2]);
            String minuteID = input[0];
            if (minuteId_line_map.containsKey(minuteID)) { // If the minute_id is in Metadata file, then add to filename_metadataLine_map
                String metadataLine = minuteId_line_map.get(minuteID);
                filename_metadataLine_map.put(filename, metadataLine);
            }
        }
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
    
    private void buildStrings() throws IOException, ParseException {
        System.out.println("Started building image tag strings.");
        for (String filename : this.filenames) {
            StringBuilder sb = getCorrectStringBuilder(filename); // Make sure to put the solution images in the beginning of the output file.
            // File format: "FileName,,TagSet,,Tag,,TagSet,,Tag,,(...)"
            sb.append(filename);
            sb.append(makeTagsFromVisualConcept(filename)); // semantic tags
            if (filename_metadataLine_map.containsKey(filename)) { // metadata tags
                String metadataLine = filename_metadataLine_map.get(filename);
                sb.append(makeTagsFromMetadata(metadataLine));
            }
            sb.append("\n");
        }
    }

    private void buildFilenameSet() throws IOException {
        this.filenames = new HashSet<>();
        BufferedReader br = new BufferedReader(new FileReader(new File(LSCFilename)));
        String line;
        while ((line = br.readLine()) != null && !line.equals("")) {
            filenames.add(makeImagePathFromLSCFilenames(line));
        }
        br.close();
    }

    private String makeTagsFromVisualConcept(String imagePath) { // semantic tags
        List<String> tagnames = featureFinder.findFeatures(imagePath);
        StringBuilder sb = new StringBuilder();
        // put all tags in a set to remove duplicate
        Set<String> concepts = new HashSet<>();
        for (String tagname : tagnames) {
            concepts.add(tagname);
        }
        // then look up the tagsetName and add to sb.
        for (String tagname : concepts) {
            if (json_tag_tagset_map.containsKey(tagname)) { // Only the tag that appears in the json hierarchy file is tagged to the image.
                String tagsetName = json_tag_tagset_map.get(tagname);
                sb.append(delimiter + tagsetName + delimiter + tagname);
            }
        }
        return sb.toString();
    }

    private StringBuilder getCorrectStringBuilder(String filename) {
        // First, check if filename is in the solution. if yes -> frontStringBuilder, no -> backStringBuilder.
        // Because we want the solution files in the front of the image-tag file so that it is always included to different databases.

        if (solutionFilenames.contains(filename)) {
            return solutionsInFront;
        } else {
            return othersAtBack;
        }
    }

    private String makeImagePathFromVisualConcept(String imagePath) {
        // LSCVisualConcept's image_path column looks like this: DATASETS/LSC2020/2015-02-23/b00000e.jpg
        // We want to store only '2015-02-23/b00000e.jpg' in the ImageTags.csv file.
        // Decided to concatenate "[Image Server address]\\lsc2020\\" in the PhotoCube client code.

        // (Windows) java uses \\, but C# uses \

        String newImagePath = imagePath.substring(17);
        Path newPath = Paths.get(newImagePath);
        return newPath.toString();
    }

    private String makeImagePathFromLSCFilenames(String imagePath) {
        // Omar's lsc2020.txt file contains filepaths looking like this: "./2015-02-23/b00000e.jpg"
        // We want to store only '2015-02-23/b00000e.jpg' in the ImageTags.csv file.
        // Decided to concatenate "[Image Server address]\\lsc2020\\" in the PhotoCube client code.

        // (Windows) java uses \\, but C# uses \
        imagePath = imagePath.replace("\"", ""); // Remove quotes
        String newImagePath = imagePath.substring(2);
        Path newPath = Paths.get(newImagePath);
        return newPath.toString();
    }

    private String makeTagsFromMetadata(String metadataLine) throws ParseException { // metadata tags
        String[] input = metadataLine.split(",");
        String[] formattedMetadataLine = metadataFormatter.formatMetadataLine(input);
        StringBuilder sb = new StringBuilder();

        // Ignore minute_id (i=0) and utc_time (i=1)

        // local_time (i=2) is broken down into Date & Time
        String[] date_time = formattedMetadataLine[2].split("_");
        sb.append(delimiter + "Date" + delimiter + date_time[0]);
        sb.append(delimiter + "Time" + delimiter + date_time[1]);

        addDateTimeRelatedTags(sb, formattedMetadataLine[2]);

        // timezone (i=3) only use the city name as the tag (Europe/Dublin -> Timezone,,Dublin)
        String[] region_city = formattedMetadataLine[3].split("/");
        sb.append(delimiter + metadataColumnNames[3] + delimiter + StringBeautifier.toPrettyFeatureName(region_city[1]));

        // Skip latitude (i=4) and longitude (i=5) - It will be useful in the future, though.

        // Everything else
        for (int i = 6; i < formattedMetadataLine.length; i++) {
            String columnName = metadataColumnNames[i];
            if (!formattedMetadataLine[i].equals("NULL")) {
                sb.append(delimiter+ columnName + delimiter + StringBeautifier.toPrettyFeatureName(formattedMetadataLine[i]));
            }
        }
        return sb.toString();
    }

    private void addDateTimeRelatedTags(StringBuilder sb, String timestamp) {
        DateTimeFormatter formatter = new DateTimeFormatter(timestamp);
        sb.append(delimiter + "Day of week (number)" + delimiter + formatter.getDayOfWeekNumber());
        sb.append(delimiter + "Day of week (string)" + delimiter + formatter.getDayOfWeekString());
        sb.append(delimiter + "Day within month" + delimiter + formatter.getDayWithinMonth());
        sb.append(delimiter + "Day within year" + delimiter + formatter.getDayWithinYear());
        sb.append(delimiter + "Month (number)" + delimiter + formatter.getMonthNumber());
        sb.append(delimiter + "Month (string)" + delimiter + formatter.getMonthString());
        sb.append(delimiter + "Year" + delimiter + formatter.getYear());
        sb.append(delimiter + "Hour" + delimiter + formatter.getHour());
        sb.append(delimiter + "Minute" + delimiter + formatter.getMinute());
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
                writer.write("FileName,,TagSet,,Tag,,TagSet,,Tag,,(...)\r\n"); // File format
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
            // itg.writeToImageTagFile();
            String path = Paths.get("2016-09-25/20160925_150243_000.jpg").toString();
            System.out.println(itg.makeTagsFromVisualConcept(path));
            
            System.out.println("Done.");
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
