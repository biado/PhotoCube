package Script;

import java.time.LocalDate;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.text.ParseException;
import java.time.*; 
import java.time.DayOfWeek; 

public class Tagset {
    private String tagsetName; // The top level tagset name.
    private Map<String, Set<String>> tagset_tags_map; // fx. People - [Adult, Family, Student]
    private Map<String, String> tag_tagset_map; // fx. Alex - People, cat - animal. Note the value is top tagset name.

    public Tagset(String line) {
        this.tagset_tags_map = new HashMap<>();
        this.tag_tagset_map = new HashMap<>();
        putLineInMaps(line);
    }

    // TODO: Maybe make a method for Timezone hierarchy? But that's not now.

    public void extendHierarchy(String columnValue, String columnName) {
        if(!columnValue.trim().equals("NULL")) {
            addTagToHierarchy(columnName, columnValue.trim());
        }
    }
    
    public void extendTimeHierarchy(String columnValue, String columnName, String timezone) throws ParseException{
        // Format of parameter: 2015-02-23_00:00
        // columnName for example utc_time
        String[] input = columnValue.split("_");
        String[] ymd = input[0].split("-");
        
        addTagToHierarchy("Year", ymd[0]);
        addTagToHierarchy("Month", ymd[1]);
        addTagToHierarchy("Date", ymd[2]);
        addTagToHierarchy("Timestamp", input[1].replace(":", "."));
        addTagToHierarchy("Day", getDay(columnValue, timezone));
    }

    // TODO: refactor to a class for transforming Day and Month
    public static String getDay(String ymdhm, String timezone) throws ParseException {
        // parameter: 2015-02-23_00:00
        // getDay("2018-05-25T00:48", "Asia/Shanghai"));
        LocalDateTime localDateTime = LocalDateTime.parse(ymdhm.replace("_", "T"));
        ZoneId zoneId = ZoneId.of(timezone);
        ZonedDateTime zdt = ZonedDateTime.of(localDateTime, zoneId);
        DayOfWeek day = DayOfWeek.from(zdt);
        return day.name();
    }

    public static String getMonth(String ymdhm, String timezone) throws ParseException {
        // parameter: 2015-02-23_00:00
        // getDay("2018-05-25T00:48", "Asia/Shanghai"));
        LocalDateTime localDateTime = LocalDateTime.parse(ymdhm.replace("_", "T"));
        ZoneId zoneId = ZoneId.of(timezone);
        ZonedDateTime zdt = ZonedDateTime.of(localDateTime, zoneId);
        Month month = zdt.getMonth();
        return month.name();
    }

    private void addTagToHierarchy(String parentTag, String tag) {
        if (tagset_tags_map.containsKey(parentTag)) {
            Set<String> tags = tagset_tags_map.get(parentTag);
            tags.add(tag);
            tagset_tags_map.put(parentTag, tags);
        } else {
            Set<String> tags = new HashSet<>();
            tags.add(tag);
            tagset_tags_map.put(parentTag, tags);
        }
    }

    public void putLineInMaps(String line) {
        String[] input = line.split(",");
        int key;
        int value;

        if (tagsetName == null)
            this.tagsetName = input[input.length - 1];

        if (!input[0].equals("")) {
            // fill in tag_tagset_map
            tag_tagset_map.put(input[0], tagsetName);

            // fill in tagset_tags_map

            // assigning initial pair
            key = input.length - 1;
            value = assignColumn(input, key - 1);

            // putting tagset-tag list into the map
            while ((value >= 0) && (key > value)) {
                String tagset = input[key];
                String tag = input[value];
                if (!tagset_tags_map.containsKey(tagset)) {
                    Set<String> tags = new HashSet<>();
                    tags.add(tag);
                    tagset_tags_map.put(tagset, tags);
                } else {
                    Set<String> tags = tagset_tags_map.get(tagset);
                    if (!tags.contains(tag)) {
                        tags.add(tag);
                        tagset_tags_map.put(tagset, tags);
                    }
                }

                key = assignColumn(input, key - 1);
                value = assignColumn(input, value - 1);
            }
        }

    }

    private int assignColumn(String[] input, int index) {
        while (index >= 0 && input[index].equals("")) {
            index = index-1;
        }
        return index;
    }

    public String getTagsetName() {
        return this.tagsetName;
    }

    public Map<String,Set<String>> getTagset_Tags_Map() {
        return this.tagset_tags_map;
    }

    public Map<String,String> getTag_Tagset_Map() {
        return this.tag_tagset_map;
    }

    @Override
    public String toString() {
        StringBuilder sb;;
        Set<String> hierarchyLines = new HashSet<>();
        for (String tagset : tagset_tags_map.keySet()) { // <- this gives duplicate lines.
            Set<String> tags = tagset_tags_map.get(tagset);

            // Note: maximum height of the tree is 2.
            // depth: 1
            sb = new StringBuilder();
            sb.append(tagsetName + ":" + tagsetName + ":" + tagset);
            for (String tag : tags) {
                sb.append(":" + tag);
            }
            sb.append("\n");
            hierarchyLines.add(sb.toString());
            

            // depth: 2
            for (String tag : tags) {
                sb = new StringBuilder();
                
                if (tagset_tags_map.containsKey(tag) && !tag.equals(tagsetName)) {
                    sb.append(tagsetName + ":" + tagsetName + ":" + tag);
                    Set<String> subtags = tagset_tags_map.get(tag);
                    for (String subtag: subtags) {
                        sb.append(":" + subtag);
                    }
                    sb.append("\n");
                    hierarchyLines.add(sb.toString());
                }
                
            }
        }

        StringBuilder sb1 = new StringBuilder();
        for (String line : hierarchyLines) {
            sb1.append(line);
        }
        return sb1.toString();
    }

    public static void main(String[] args) throws ParseException{
        System.out.println(Tagset.getMonth("2015-02-23_00:00", "Asia/Shanghai")); 
    }
}
