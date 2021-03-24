import java.util.HashMap;
import java.util.Map;

public class MetadataFormatter {
    private static final String LSCmetadata = "C:\\lsc2020\\lsc2020-metadata\\lsc2020-metadata.csv";
    private Map<String, String> columnTypes;

    public MetadataFormatter() {
        initializeColumnTypes();
    }

    private void initializeColumnTypes() {
        this.columnTypes = new HashMap<>();
        columnTypes.put("elevation", "int");
        columnTypes.put("minute_id", "string");
        columnTypes.put("timezone", "string");
        columnTypes.put("semantic_name", "string");
        columnTypes.put("lon", "double");
        columnTypes.put("calories", "int");
        columnTypes.put("steps", "int");
        columnTypes.put("speed", "int");
        columnTypes.put("heart", "int");
        columnTypes.put("local_time", "string");
        columnTypes.put("utc_time", "string");
        columnTypes.put("activity_type", "string");
        columnTypes.put("lat", "double");
    }
}
