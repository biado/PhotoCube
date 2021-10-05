using System.Collections.Generic;

namespace ObjectCubeServer.Models.DomainClasses
{
    public class ParsedFilter
    {
        public string type { get; set; }
            //possible types:
            //"tag" : Tag filter, including year, month, day filters from Client's Left Dock 
            //"tagset" : Tagset filter. Tagged with at least 1 tag in a tagset 
            //"hierarchy" : Hierarchy filter. Tagged with at least 1 tag in a hierarchy
            //"time": Time range filter. Tagged with tags between startTime tag and endTime tag
            //"date": Applied same as tag filter. Request with tags from Year, Month (number), Day within month may have this type.
            //"day of week": Tag filter with OR search
        public List<int> Ids { get; set; }
        public List<List<string>> Ranges { get; set; }

        //internal void initializeIds()
        //{
        //    List<int> IdList = new List<int>();
        //    IEnumerable<Tag> tagsInTagset;
        //    switch (type)
        //    {
        //        case "tagset":
        //            tagsInTagset = extractTagsFromTagsetFilter(); // Could not query Tags table using raw sql - seems like it doesn't support TPT hierarchies.

        //            foreach (var tag in tagsInTagset)
        //            {
        //                IdList.Add(tag.Id);
        //            }

        //            Ids = IdList;
        //            break;

        //        case "hierarchy":
        //        case "tag":
        //        case "date":
        //            IdList.Add(Id);
        //            Ids = IdList;
        //            break;

        //        case "day of week":
        //            if (Ids == null) // Case: There is only 1 DoW filter in the request. (If more than 1, already merged and created Ids List in CellController.)
        //            {
        //                IdList.Add(Id);
        //                Ids = IdList;
        //            }
        //            break;

        //        case "time":
        //            tagsInTagset = extractTagsFromTimeFilter();

        //            foreach (var tag in tagsInTagset)
        //            {
        //                IdList.Add(tag.Id);
        //            }
        //            Ids = IdList;
        //            break;
        //    }
        //}

        //private List<Tag> extractTagsFromTagsetFilter()
        //{
        //    using var context = new ObjectContext();
        //    var Tagset = context.Tagsets
        //        .Include(ts => ts.Tags)
        //        .FirstOrDefault(ts => ts.Id == Id);
        //    return Tagset.Tags;
        //}

        //private List<Tag> extractTagsFromTimeFilter()
        //{
        //    var times = name.Split("-");
        //    TimeSpan start = parseToTimeSpan(times[0]);
        //    TimeSpan end = parseToTimeSpan(times[1]);

        //    using (var context = new ObjectContext())
        //    {
        //        var Tagset = context.Tagsets
        //            .Include(ts => ts.Tags)
        //            .FirstOrDefault(ts => ts.Name == "Time");
        //        if (start <= end)
        //        {
        //            return Tagset.Tags.Where(t =>
        //                ((TimeTag)t).Name >= start &&
        //                ((TimeTag)t).Name <= end).ToList();
        //        }
        //        else
        //        {
        //            // Case: Going over midnight. For example, 20:00-02:00
        //            return Tagset.Tags.Where(t =>
        //                (((TimeTag)t).Name >= start &&
        //                 ((TimeTag)t).Name < new TimeSpan(24, 0, 0))
        //                || (((TimeTag)t).Name >= new TimeSpan(0, 0, 0) &&
        //                    ((TimeTag)t).Name <= end)).ToList();
        //        }
        //    }
        //}

        //private TimeSpan parseToTimeSpan(string timeString)
        //{
        //    string[] hourMinute = timeString.Split(":");
        //    int hour = int.Parse(hourMinute[0]);
        //    int minute = int.Parse(hourMinute[1]);
        //    TimeSpan time = new TimeSpan(hour, minute, 0);
        //    return time;
        //}
    }
}
