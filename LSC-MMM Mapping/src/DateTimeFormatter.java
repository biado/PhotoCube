import java.time.LocalDateTime;
import java.time.format.TextStyle;
import java.util.Locale;

public class DateTimeFormatter {
    private LocalDateTime localDateTime;

    public DateTimeFormatter(String timestamp) {
        // parameter format: 2015-02-23_00:00
        this.localDateTime = LocalDateTime.parse(timestamp.replace("_", "T"));
    }

    public String getDayOfWeekString() {
        // Monday...Sunday
        return localDateTime.getDayOfWeek().getDisplayName(TextStyle.FULL, Locale.ENGLISH);
    }

    public String getDayOfWeekNumber() {
        // 1 (Monday)...7 (Sunday)
        return "" + localDateTime.getDayOfWeek().getValue();
    }

    public String getMonthString() {
        // January...December
        return localDateTime.getMonth().getDisplayName(TextStyle.FULL, Locale.ENGLISH);
    }

    public String getMonthNumber() {
        // 1...12
        return "" + localDateTime.getMonth().getValue();
    }

    public String getDayWithinMonth() {
        // 1...31
        return "" + localDateTime.getDayOfMonth();
    }

    public String getDayWithinYear() {
        // 1...366
        return "" + localDateTime.getDayOfYear();
    }

    public String getYear() {
        // fx. 2021
        return "" + localDateTime.getYear();
    }

    public String getHour() {
        // 0...23
        return "" + localDateTime.getHour();
    }

    public String getMinute() {
        // 0...59
        return "" + localDateTime.getMinute();
    }

    public static void main(String[] args) {
        DateTimeFormatter dtf = new DateTimeFormatter("2015-05-17_23:20");
        System.out.println(dtf.getDayOfWeekString()); 
    }
}
