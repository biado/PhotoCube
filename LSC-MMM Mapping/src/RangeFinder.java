import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;
import java.util.TreeSet;

public class RangeFinder {
    private static final String LSCmetadata = "C:\\lsc2020\\lsc2020-metadata\\lsc2020-metadata.csv";
    private static final String outputPath = "C:\\lsc2020\\tags-and-hierarchies\\lsc2019-metadata-range-1.csv";

    private Map<String, String> columnTypes;
    private Map<String, TreeSet<String>> stringColumnValues;
    private Map<String, TreeSet<Long>> intColumnValues;
    private Map<String, TreeSet<Double>> doubleColumnValues;

    public RangeFinder() throws IOException {
        initializeColumnTypes();
        initializeColumnValueMaps();
        buildSet();
    }

    public void buildSet() throws IOException {
        BufferedReader br = new BufferedReader(new FileReader(new File(LSCmetadata)));
        String[] columns = br.readLine().split(","); // first line
        String line;
        while ((line = br.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            input = HierarchyGenerator.sanitizeInput(input);
            for (int i=0; i<input.length; i++) {
                String column = columns[i];
                String value = input[i];
                switch (columnTypes.get(column)) {
                    case "string":
                        putStringValues(column, value);
                        break;
                    case "int":
                        putIntValues(column, value);
                        break;
                    case "double":
                        putDoubleValues(column, value);
                        break;
                    default:
                        break;
                }

            }
        }
    }

    private void putStringValues(String column, String value) {
        if (!value.equals("NULL")) {
            if (stringColumnValues.containsKey(column)) {
                TreeSet<String> values = stringColumnValues.get(column);
                values.add(value);
                stringColumnValues.put(column, values);
            } else {
                TreeSet<String> values = new TreeSet<>();
                values.add(value);
                stringColumnValues.put(column, values);
            }
        }
    }
    
    private void putIntValues(String column, String value) {
        if (!value.equals("NULL")) {
            if (intColumnValues.containsKey(column)) {
                TreeSet<Long> values = intColumnValues.get(column);
                long oneDigitValue = Math.round(Double.parseDouble(value));
                values.add(oneDigitValue);
                intColumnValues.put(column, values);
            } else {
                TreeSet<Long> values = new TreeSet<>();
                long oneDigitValue = Math.round(Double.parseDouble(value));
                values.add(oneDigitValue);
                intColumnValues.put(column, values);
            }
        }
    }

    private void putDoubleValues(String column, String value) {
        if (!value.equals("NULL")) {
            if (doubleColumnValues.containsKey(column)) {
                TreeSet<Double> values = doubleColumnValues.get(column);
                Double fiveDigitValue = Math.round(Double.parseDouble(value) * 10000.0) / 10000.0;
                values.add(fiveDigitValue);
                doubleColumnValues.put(column, values);
            } else {
                TreeSet<Double> values = new TreeSet<>();
                Double fiveDigitValue = Math.round(Double.parseDouble(value) * 10000.0) / 10000.0;
                values.add(fiveDigitValue);
                doubleColumnValues.put(column, values);
            }
        }
    }

    public String buildString() {
        StringBuilder sb = new StringBuilder();
        sb.append("Column,Minimum,Maximum,Size\n");
        for (String column : stringColumnValues.keySet()) {
            TreeSet<String> values = stringColumnValues.get(column);
            sb.append(column + "," + values.first() + "," + values.last() + "," + values.size() + "\n");
        }
        for (String column : intColumnValues.keySet()) {
            TreeSet<Long> values = intColumnValues.get(column);
            sb.append(column + "," + values.first() + "," + values.last() + "," + values.size() + "\n");
        }
        for (String column : doubleColumnValues.keySet()) {
            TreeSet<Double> values = doubleColumnValues.get(column);
            sb.append(column + "," + values.first() + "," + values.last() + "," + values.size() + "\n");
        }
        return sb.toString();
    }

    public void writeToFile() {
        System.out.println("Started writing tags into the file.");
            try {
                BufferedWriter writer = new BufferedWriter(new FileWriter(outputPath));
                writer.write(this.buildString());
                writer.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
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

    private void initializeColumnValueMaps() {
        this.stringColumnValues = new HashMap<>();
        this.intColumnValues = new HashMap<>();
        this.doubleColumnValues = new HashMap<>();
    }

    public static void main(String[] args) throws IOException {
        new RangeFinder().writeToFile();
        System.out.println("Done.");
    }
}
