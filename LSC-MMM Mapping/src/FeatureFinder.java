import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

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

    public FeatureFinder() throws IOException {
        filename_row_map = new HashMap<>();
        buildFilenameRowMap();
        row_featureIndex_map = new HashMap<>();
        buildRowFeatureIndexMap();
        featureIndex_tagname_map = new HashMap<>();
        buildFeatureIndexTagnameMap();
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
            row = filename_row_map.get(filename);
        }
        if (row != -1) {
            List<Integer> featureIndex = (row_featureIndex_map.containsKey(row)) ? row_featureIndex_map.get(row) : new ArrayList<>();
            for (int index : featureIndex) {
                String tagname = featureIndex_tagname_map.get(index);
                if (tagname != null) {
                    tagnames.add(tagname);
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
        FeatureFinder ff = new FeatureFinder();
        String path = Paths.get("2018-05-14\\B00001004_21I6X0_20180514_091713E.JPG").toString();
        System.out.println(ff.findFeatures(path));
    }
}