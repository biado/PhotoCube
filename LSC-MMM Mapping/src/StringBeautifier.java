/**
 * StringBeautifier helps cleaning the tag names in a consistent way.
 * We clean a name of JSTagset by 1) replacing underscore to space and 2) make only the first letter capitalized.
 */
public class StringBeautifier {
    public static String toPrettyTagsetName(JSTagset current) {
        String underscoreRemoved = current.getName().replaceAll("_", " ");
        String firstLetterCapitalized = underscoreRemoved.substring(0, 1).toUpperCase() + underscoreRemoved.substring(1).toLowerCase();
        return firstLetterCapitalized;
    }

    public static String toPrettyFeatureName(String name) {
        String underscoreRemoved = name.replaceAll("_", " ");
        String firstLetterCapitalized = underscoreRemoved.substring(0, 1).toUpperCase() + underscoreRemoved.substring(1).toLowerCase();
        return firstLetterCapitalized;
    }
}
