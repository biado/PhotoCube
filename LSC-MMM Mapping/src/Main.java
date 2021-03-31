import java.io.IOException;
import java.text.ParseException;

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
