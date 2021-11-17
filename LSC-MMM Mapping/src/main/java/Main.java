import java.io.IOException;
import java.text.ParseException;

/**
 * The Main class of the program.
 * The program requires config.properties file where filepaths are specified. (See Readme)
 * Running the program results in 2 files: lscHierarchies.csv and lscImageTags.csv.
 */
public class Main {
    public static void main(String[] args) {
        ImageTagGenerator itg;
        try {
            itg = new ImageTagGenerator();
            itg.writeToImageTagFile();
        } catch (IOException e) {
            e.printStackTrace();
        } catch (ParseException e) {
            e.printStackTrace();
        }
        System.out.println("Done.");
    }
}
