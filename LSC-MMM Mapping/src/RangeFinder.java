package Script;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;
import java.util.TreeSet;

public class RangeFinder {
    private static final String LSCmetadata = "C:\\lsc2020\\lsc2020-metadata\\lsc2020-metadata.csv";
    private static final String outputPath = "C:\\lsc2020\\tags-and-hierarchies\\lsc2019-metadata-range.csv";

    private Map<String, TreeSet<String>> columnValues;

    public RangeFinder() throws IOException {
        buildSet();
    }

    public void buildSet() throws IOException {
        BufferedReader br = new BufferedReader(new FileReader(new File(LSCmetadata)));
        String[] columns = br.readLine().split(","); // first line
        columnValues = new HashMap<>();
        String line;
        while ((line = br.readLine()) != null && !line.equals("")) {
            String[] input = line.split(",");
            input = HierarchyGenerator.sanitizeInput(input);
            for (int i=0; i<input.length; i++) {
                String column = columns[i];
                String value = input[i];
                if (!value.equals("NULL")) {
                    if (columnValues.containsKey(column)) {
                        TreeSet<String> values = columnValues.get(column);
                        values.add(value);
                        columnValues.put(column, values);
                    } else {
                        TreeSet<String> values = new TreeSet<>();
                        values.add(value);
                        columnValues.put(column, values);
                    }
                }
            }
        }
    }

    public String buildString() {
        StringBuilder sb = new StringBuilder();
        sb.append("Column,Minimum,Maximum,Size\n");
        for (String column : columnValues.keySet()) {
            TreeSet<String> values = columnValues.get(column);
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

    public static void main(String[] args) throws IOException {
        new RangeFinder().writeToFile();
        System.out.println("Done.");
    }
}
