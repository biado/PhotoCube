import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

/**
 * FeatureFinder finds the semantic tags associated to the given LSC filename.
 * Currently it finds max. 5 tags with top probability, duplicates removed.
 */
public class FeatureFinder {
    private static final String LSCFilename = FilepathReader.LSCFilename;
    private static final String ImageFeatureTop5 = FilepathReader.ImageFeatureTop5;
    private static final String FeatureTags = FilepathReader.FeatureTags;

    private Map<String, Integer> filename_row_map; // row: line index for filename (0 based)
    private Map<Integer, List<Integer>> row_featureIndex_map; // row: line index for filename (0 based), featureIndex: index for feature (0 based)
    private Map<Integer, String> featureIndex_tagname_map; // featureIndex: index for feature (0 based), tagname: name of the tag (only the first word in the lines of FeatureTags file)

    private Set<String> homonyms;

    public FeatureFinder(Set<String> homonyms) throws IOException {
        filename_row_map = new HashMap<>();
        buildFilenameRowMap();
        row_featureIndex_map = new HashMap<>();
        buildRowFeatureIndexMap();
        featureIndex_tagname_map = new HashMap<>();
        buildFeatureIndexTagnameMap();
        this.homonyms = homonyms;
    }

    /**
     * Finds the semantic tags associated to the given LSC filename.
     * @param filename
     * @return List of the semantic tags
     */
    public List<String> findFeatures(String filename) {
        List<String> tagnames = new ArrayList<>();
        int row = -1;
        if (filename_row_map.containsKey(filename)) {
            row = filename_row_map.get(filename); // Find the row of filename
        }
        if (row != -1) {
            List<Integer> featureIndex = (row_featureIndex_map.containsKey(row)) ? row_featureIndex_map.get(row) : new ArrayList<>(); // get the list of featureIndex
            for (int index : featureIndex) { // for each index number, find the tagname
                String tagname = featureIndex_tagname_map.get(index); // all lowercase
                if (tagname != null) { // we found the tagname for the index number
                    String beautifiedName = StringBeautifier.toPrettyFeatureName(tagname); // change to consistent naming strategy (First letter capital, no underscore in the name)
                    if (homonyms.contains(beautifiedName)) { // There are multiple tagsets of different meanings for this tagname
                        beautifiedName = beautifiedName + "(" + index + ")"; // Handle duplicates by concatenating id(=feature index) to the tagname
                    }
                    tagnames.add(beautifiedName);
                }
            }
        }
        return tagnames;
    }

    private void buildFeatureIndexTagnameMap() throws IOException {
        BufferedReader br = new BufferedReader(new FileReader(new File(FeatureTags)));
        String line;
        int index = 0;
        while ((line = br.readLine()) != null && !line.equals("")) {
            String[] words_description = line.split(":");
            String[] words = words_description[0].split(",");
            String tagname = words[0].strip();
            this.featureIndex_tagname_map.put(index, tagname);
            index++;
        }
        br.close();
    }

    private void buildRowFeatureIndexMap() throws IOException {
        BufferedReader br = new BufferedReader(new FileReader(new File(ImageFeatureTop5)));
        String line;
        int row = 0;
        while ((line = br.readLine()) != null && !line.equals("")) {
            List<Integer> featureIndexes = makeFeatureIndexList(line);
            this.row_featureIndex_map.put(row, featureIndexes);
            row++;
        }
        br.close();
    }

    private List<Integer> makeFeatureIndexList(String line) {
        List<Integer> featureIndexes = new ArrayList<>();
        // It will be much better with regex, but focused on making it work.
        String replacedLine = line.replaceAll("[\\[\\ \\(\\]]", "");
        String[] splitTuples = replacedLine.split("\\)\\,");
        for (String tuple : splitTuples) {
            String[] index_prob = tuple.split("\\,");
            int index = Integer.parseInt(index_prob[0]);
            featureIndexes.add(index);
        }
        return featureIndexes;
    }

    private void buildFilenameRowMap() throws IOException {
        BufferedReader br = new BufferedReader(new FileReader(new File(LSCFilename)));
        String line;
        int row = 0;
        while ((line = br.readLine()) != null && !line.equals("")) {
            String filename = makeImagePathFromLSCFilenames(line);
            this.filename_row_map.put(filename, row);
            row++;
        }
        br.close();
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

    public static void main(String[] args) throws IOException {
        JsonHierarchyGenerator jshg = new JsonHierarchyGenerator();
        FeatureFinder ff = new FeatureFinder(jshg.getHomonyms());
        String path = Paths.get("2016-08-11/20160811_154301_000.jpg").toString();
        System.out.println(ff.findFeatures(path));
    }
}