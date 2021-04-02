import java.util.HashMap;
import java.util.Map;

/**
 * MetadataFormatter changes the metadata values to fit the data type of columns we agreed upon.
 * For example, a Heartbeat value would be formatted as Int, thus clear all the decimal digits.
 */
public class MetadataFormatter {
    private int numOfMetadataColumns;
    private String[] columnTypes;

    public MetadataFormatter(String[] metadataColumns) {
        Map<String, String> columnTypeMap = initializeColumnTypeMap();
        initializeColumnTypes(columnTypeMap, metadataColumns);
    }

    /**
     * Returns the String array with correctly formatted metadata values according to data type of columns
     * @param metadataLine a line from LSC metadata file, split by delimiter
     * @return String[] with correctly formatted metadata values
     */
    public String[] formatMetadataLine(String[] metadataLine) {
        String[] formatted = new String[numOfMetadataColumns];
        metadataLine = sanitizeInputForCommaUsedInSemanticName(metadataLine);
        for (int i = 0; i < numOfMetadataColumns; i++) {
            String columnValue = metadataLine[i];
            formatted[i] = formatColumnValue(columnValue, i);
        }
        return formatted;
    }

    private String formatColumnValue(String columnValue, int index) {
        String formattedValue = "";
        if (!columnValue.equals("NULL")) {
            switch (columnTypes[index]) {
                case "int":
                    long longValue = Math.round(Double.parseDouble(columnValue));
                    formattedValue = Long.toString(longValue);
                    break;
                case "double":
                    Double fiveDigitValue = Math.round(Double.parseDouble(columnValue) * 10000.0) / 10000.0;
                    formattedValue = Double.toString(fiveDigitValue);
                    break;
                case "string":
                    formattedValue = columnValue;
                    break;
            }
            return formattedValue;
        }
        return columnValue;
    }

    private void initializeColumnTypes(Map<String, String> columnTypeMap, String[] metadataColumns) {
        this.numOfMetadataColumns = metadataColumns.length;
        this.columnTypes = new String[numOfMetadataColumns];
        for (int i = 0; i < numOfMetadataColumns; i++) {
            String columnName = metadataColumns[i];
            String columnType = columnTypeMap.get(columnName);
            columnTypes[i] = columnType;
        }
    }

    private String[] sanitizeInputForCommaUsedInSemanticName(String[] input) {
        if (input.length != numOfMetadataColumns) { // comma(,) used in the 6th column(semantic_name). input[] length == 14
            String[] sanitized = new String[numOfMetadataColumns];
            for(int i = 0; i<6; i++) {
                sanitized[i] = input[i];
            }
            sanitized[6] = String.join(",", input[6], input[7]).replace(", ","-");
            for(int i=7; i<sanitized.length; i++) {
                sanitized[i] = input[i+1];
            }
            return sanitized;
        } else {
            return input;
        }
    }

    // The data type we used for LSC metadata columns.
    private Map<String, String> initializeColumnTypeMap() {
        Map<String, String> columnTypeMap = new HashMap<>();
        columnTypeMap.put("elevation", "int");
        columnTypeMap.put("minute_id", "string");
        columnTypeMap.put("timezone", "string");
        columnTypeMap.put("semantic_name", "string");
        columnTypeMap.put("lon", "double");
        columnTypeMap.put("calories", "int");
        columnTypeMap.put("steps", "int");
        columnTypeMap.put("speed", "int");
        columnTypeMap.put("heart", "int");
        columnTypeMap.put("local_time", "string");
        columnTypeMap.put("utc_time", "string");
        columnTypeMap.put("activity_type", "string");
        columnTypeMap.put("lat", "double");
        return columnTypeMap;
    }

    /**
     * Used to format the metadata column names to the names we agreed upon.
     * @param columnNamesFromCSV the first line of LSC metadata file, split by delimiter
     * @return the String[] with formatted column names
     */
    public String[] formatMetadataColumnNames(String[] columnNamesFromCSV) {
        Map<String, String> columnNameMap = initializeColumnNameMap();
        String[] formattedColumnNames = new String[numOfMetadataColumns];
        for (int i = 0; i < numOfMetadataColumns; i++) {
            String columnName = columnNamesFromCSV[i];
            String formattedColumnName = columnNameMap.containsKey(columnName) ? columnNameMap.get(columnName) : columnName;
            formattedColumnNames[i] = formattedColumnName;
        }
        return formattedColumnNames;
    }

    private Map<String, String> initializeColumnNameMap() {
        Map<String, String> columnNameMap = new HashMap<>();
        columnNameMap.put("elevation", "Elevation");
        columnNameMap.put("timezone", "Timezone");
        columnNameMap.put("semantic_name", "Semantic name");
        columnNameMap.put("lon", "Longitude");
        columnNameMap.put("calories", "Calories");
        columnNameMap.put("steps", "Steps");
        columnNameMap.put("speed", "Speed");
        columnNameMap.put("heart", "Heart");
        columnNameMap.put("local_time", "Local time");
        columnNameMap.put("utc_time", "UTC time");
        columnNameMap.put("activity_type", "Activity type");
        columnNameMap.put("lat", "Latitude");
        return columnNameMap;
    }

    public static void main(String[] args) {
        String[] firstLine = new String[] {"minute_id", "utc_time", "local_time", "timezone", "lat", "lon", "semantic_name", "elevation", "speed", "heart", "calories", "activity_type", "steps"};
        MetadataFormatter mf = new MetadataFormatter(firstLine);
    }
}
