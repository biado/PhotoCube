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
 * SolutionListGenerator reads in LSCTopic (xml file) and generates a list of solution files without duplicates.
 */
public class SolutionListGenerator {
    private static final String topicXmlPath = FilepathReader.LSCTopic;
    private static final String outputFileName = "lsc2019-solution-list-test.txt";
    
    private Set<String> set = new HashSet<>();

    public SolutionListGenerator() throws IOException {
        this.set = new HashSet<>();
        BufferedReader br = new BufferedReader(new FileReader(new File(topicXmlPath)));
        buildSet(br);
    }

    private void buildSet(BufferedReader br) throws IOException {
        // TODO: There's probably a better way to read in xml rather than using regex as we did.
        String line;
        while((line = br.readLine()) != null && !line.equals("")) {
            if (line.contains(".jpg")) {
                String[] splitedTexts = line.split("(?=<)|(?<=>)");
                // line is splited into 4 parts.
                //                  , <ImageID> , 20160822_061808_000.jpg , </ImageID>
                
                String filename = splitedTexts[2];
                if (set.contains(filename)) {
                    System.out.println("Duplicate file in the solution: " + filename);
                } else {
                    set.add(filename);
                }
            }
        }
    }

    /**
     * Returns the list of LSC solution filenames
     */
    @Override
    public String toString() {
        StringBuilder sb = new StringBuilder();
        for (String filename : set) {
            sb.append(filename+"\n");
        }
        return sb.toString();
    }

    /**
     * Writes the list of LSC solution filenames to a file.
     */
    public void writeToSolutionListFile() {
        System.out.println("Started writing tags into the output file.");
        try {
            BufferedWriter writer = new BufferedWriter(new FileWriter(new File(FilepathReader.OutputFolder, outputFileName)));
            writer.write(this.toString());
            writer.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        System.out.println("Done writing tags into the output file.");
    }

    /**
     * Returns a Set that includes LSC solution filenames.
     * @return the Set of LSC solution filenames
     */
    public Set<String> getSolutionSet() {
        return this.set;
    }
    
    public static void main(String[] args) {
        System.out.println("Started finding solution files from Topics file.");
        try {
            SolutionListGenerator slg = new SolutionListGenerator();
            slg.writeToSolutionListFile();
            System.out.println("Done.");
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
