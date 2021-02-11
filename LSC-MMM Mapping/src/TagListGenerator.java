import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.HashSet;
import java.util.Set;

/**
 * TagListGenerator
 */
public class TagListGenerator {
    private BufferedReader br;
    private StringBuilder sb = new StringBuilder();
    private BufferedWriter writer;
    private final String visualConceptPath = "C:\\lsc2020\\lsc2020_visual_concepts\\lsc2020-visual-concepts.csv";
    private final String metadataPath = "C:\\lsc2020\\lsc2020-metadata\\lsc2020-metadata.csv";
    private final String outputPath = "C:\\lsc2020\\TagList.txt";

    private Set<String> tags = new HashSet<>();

    public static void main(String[] args) {
        System.out.println("Starting.");
        TagListGenerator tlg = new TagListGenerator();
        tlg.addTagsFromVisualConcept();
        tlg.addTagsFromMetadata();
        tlg.writeToTXTFile();
        System.out.println("Done.");
    }

    private void addTagsFromVisualConcept() {
        System.out.println("Started adding tags from Visual Concept file.");
        try {
            br = new BufferedReader(new FileReader(new File(visualConceptPath)));
            sb.append("***** Tag list from Visual Concept file (Attributes 01-10) *****\n");
            String line = br.readLine(); // Skipping the first row, which is the column name.
            while((line = br.readLine()) != null && !line.equals("")) {
                String[] words = line.split(",");
                for (int i = 3; i <= 12; i++) {
                    if (!words[i].equals("NULL")) tags.add(words[i]);
                }
            }       
            appendToStringBuilder();    
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
        System.out.println("Finished adding tags from Visual Concept file.");
    }

    private void addTagsFromMetadata() {
        System.out.println("Started adding tags from Metadata file.");
        try {
            br = new BufferedReader(new FileReader(new File(metadataPath)));
            sb.append("\n***** Tag list from Metadata file (timezone, semantic_name, activity_type) *****\n");
            String line = br.readLine(); // Skipping the first row, which is the column name.
            while((line = br.readLine()) != null && !line.equals("")) {
                String[] words = line.split(",");
                String[] timezone = words[3].split("/"); // timezone: Europe/Dublin
                if (!timezone[0].equals("NULL")) tags.add(timezone[0]);
                if (!timezone[1].equals("NULL")) tags.add(timezone[1]);
                if (words.length == 13) {
                    if (!words[6].equals("NULL")) tags.add(words[6]); // semantic_name
                    if (!words[11].equals("NULL")) tags.add(words[11]); // activity_type
                } else { // comma(,) in the 6th column. words[] length == 14
                    if (!words[6].equals("NULL") && !words[7].equals("NULL")) tags.add(String.join(",", words[6], words[7])); // semantic_name
                    if (!words[12].equals("NULL")) tags.add(words[12]); // activity_type
                }
                
            }           
            appendToStringBuilder();
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
        System.out.println("Finished adding tags from Metadata file.");
    }

    private void appendToStringBuilder() {
        for (String tag : tags) {
            sb.append(tag + "\n");
        }
        tags.clear();
    }

    private void writeToTXTFile() {
        System.out.println("Started writing tags into the output file.");
        try {
            writer = new BufferedWriter(new FileWriter(outputPath));
            writer.write(sb.toString());
            writer.close();
        } catch (IOException e) {
            e.printStackTrace();
        }        
        System.out.println("Done writing tags into the output file.");
    }
}