import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

/**
 * Reads in config.properties file and store the filepath information in the static fields.
 * It is the way we deal with different filepaths in different OS.
 */
public class FilepathReader {
    public static String LSCVisualConcept;
    public static String LSCMetadata;
    public static String LSCTopic;
    public static String manualTagSetsMD;
    public static String JsonHierarchy;
    public static String LSCFilename;
    public static String ImageFeatureTop5;
    public static String FeatureTags;
    public static String LSCHierarchiesOutput;
    public static String LSCImageTagsOutput;
    public static String UniqueTagHierarchy;
    public static String OutputFolder;

    static { // initialize static fields
        Properties prop = new Properties();
        String configFile = "config.properties";
        InputStream is = null;
        try {
            is = ClassLoader.getSystemResourceAsStream(configFile);
            prop.load(is);
        } catch (IOException e) {
            System.out.println(e.getMessage());
            System.out.println(e.getStackTrace());
        }
        LSCVisualConcept = prop.getProperty("LSCVisualConcept");
        LSCMetadata = prop.getProperty("LSCMetadata");
        LSCTopic = prop.getProperty("LSCTopic");
        manualTagSetsMD = prop.getProperty("manualTagSetsMD");
        JsonHierarchy = prop.getProperty("JsonHierarchy");
        LSCFilename = prop.getProperty("LSCFilename");
        ImageFeatureTop5 = prop.getProperty("ImageFeatureTop5");
        FeatureTags = prop.getProperty("FeatureTags");
        LSCHierarchiesOutput = prop.getProperty("LSCHierarchiesOutput");
        LSCImageTagsOutput = prop.getProperty("LSCImageTagsOutput");
        UniqueTagHierarchy = prop.getProperty("UniqueTagHierarchy");
        OutputFolder = prop.getProperty("OutputFolder");
    }
}
