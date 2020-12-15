package Script;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.HashSet;
import java.util.Set;

public class SolutionListGenerator {
    private static final String topicXmlPath = "C:\\lsc2020\\tags-and-hierarchies\\lsc2019-topics.xml";
    private static final String outputPath = "C:\\lsc2020\\tags-and-hierarchies\\lsc2019-solution-list.txt";
    
    private Set<String> set = new HashSet<>();

    public SolutionListGenerator() throws IOException {
        this.set = new HashSet<>();
        BufferedReader br = new BufferedReader(new FileReader(new File(topicXmlPath)));
        buildSet(br);
    }

    public void buildSet(BufferedReader br) throws IOException {
        String line;
        while((line = br.readLine()) != null && !line.equals("")) {
            if (line.contains(".jpg")) {
                String[] splitedTexts = line.split("(?=<)|(?<=>)");
                // line is splited into 4 parts.
                //                  , <ImageID> , 20160822_061808_000.jpg , </ImageID>
                
                String filename = splitedTexts[2];
                // String folder = filename.substring(0,8);
                // String foldername = folder.substring(0,4) + "-" + folder.substring(4,6) + "-" + folder.substring(6,8) + "\\";
                // String filepath = foldername + filename;
                if (set.contains(filename)) {
                    System.out.println("Duplicate file in the solution: " + filename);
                } else {
                    set.add(filename);
                }
            }
        }
    }

    @Override
    public String toString() {
        StringBuilder sb = new StringBuilder();
        for (String filename : set) {
            sb.append(filename+"\n");
        }
        return sb.toString();
    }

    public void writeToSolutionListFile() {
        System.out.println("Started writing tags into the output file.");
        try {
            BufferedWriter writer = new BufferedWriter(new FileWriter(outputPath));
            writer.write(this.toString());
            writer.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        System.out.println("Done writing tags into the output file.");
    }

    public Set<String> getSolutionSet() {
        return this.set;
    }
    
    public static void main(String[] args) {
        System.out.println("Started finding solution files from Topics file.");
        try {
            SolutionListGenerator slg = new SolutionListGenerator();
            slg.writeToSolutionListFile();
            
            // System.out.println("number of files: " + count);
            // System.out.println("number of distinct solution files: " + set.size());
            // System.out.println("total line in the file: " + totalline);

            System.out.println("Done.");
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
