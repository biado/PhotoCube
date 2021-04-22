import java.io.BufferedReader;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;

import com.google.gson.*;

public class JsonToTagsetParser {
    public JSTagset root;
    public Gson g;

    public JsonToTagsetParser() {
        this.g = new Gson();
    }

    public void buildTagsets(String jsonString) {
        this.root = g.fromJson(jsonString, JSTagset.class);
    }

    public static void main(String[] args) throws FileNotFoundException {
        String draftjson = FilepathReader.DraftJson;
        BufferedReader br = new BufferedReader(new FileReader(new File(draftjson)));
        Gson g = new Gson();
        JSTagset root = g.fromJson(br, JSTagset.class);

        System.out.println(root.name);
        for (JSTagset childtagset : root.children) {
            System.out.println(childtagset.name);
        }
        for (JSTagset childtagset : root.children[0].children) {
            System.out.println(childtagset.name);
        }
        System.out.println(root.children[0].children[0].children);
    }
}
