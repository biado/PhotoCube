using ObjectCubeServer.Models.DomainClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectCubeServer.Services
{
    public class QueryGenerationService
    {
        private int numberOfAdditionalFilters;
        private int totalNumberOfFilters;  // cell definition filters + additional filters
        private int numberOfFilters;  // internal counter

        internal string generateSQLQueryForState(string xType, int xVertexId, string yType, int yVertexId, string zType, int zVertexId, IList<ParsedFilter>? filtersList)
        {
            numberOfAdditionalFilters = filtersList?.Count ?? 0;
            totalNumberOfFilters = numberOfAdditionalFilters + ((xType != "") ? 1 : 0) + ((yType != "") ? 1 : 0) + ((zType != "") ? 1 : 0);
            numberOfFilters = 0;

            if (totalNumberOfFilters == 0)
            {
                const string baseQuery = "select X.idx as x, X.idy as y, X.idz as z, X.object_id as id, O.file_uri as fileURI, X.cnt as count from(select 1 as idx, 1 as idy, 1 as idz, max(R1.id) as object_id, count(*) as cnt from cubeobjects R1 group by idx, idy, idz) X join cubeobjects O on X.object_id = O.id;";
                return baseQuery;
            }
            
            var queryFront = new StringBuilder("select X.idx as x, X.idy as y, X.idz as z, X.object_id as id, O.file_uri as fileURI, X.cnt as count from (select ");
            var queryMiddle =  new StringBuilder(" from (");
            var queryEnd =  new StringBuilder(" group by idx, idy, idz");

            numberOfFilters += (xType == "") ? 0 : 1;
            queryFront.Append((xType == "") ? "1 as idx, " : $"R{numberOfFilters}.id as idx, ");

            queryMiddle.Append(generateAxisQueryForState(xType, xVertexId));
            queryMiddle.Append((xType == "")
                ? ""         // There is no x-value
                : (numberOfFilters == totalNumberOfFilters)
                ? ""         // There is an x-value, but nothing more
                : " join ("); // There is an x-value, and something more

            numberOfFilters += (yType == "") ? 0 : 1;
            queryFront.Append((yType == "") ? "1 as idy, " : String.Format("R{0}.id as idy, ", numberOfFilters));

            queryMiddle.Append(generateAxisQueryForState(yType, yVertexId));
            queryMiddle.Append((yType == "")
                ? ""         // There is no y-value
                : ((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                ? ""  // This is the first entry, and there is nothing more
                : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                ? " join ("  // This is the first entry, and there is something more
                : (numberOfFilters == totalNumberOfFilters)
                ? $" on R1.object_id = R{numberOfFilters}.object_id " // Not first, but nothing more
                : $" on R1.object_id = R{numberOfFilters}.object_id join ("); // Not first, and something more

            numberOfFilters += (zType == "") ? 0 : 1;
            queryFront.Append((zType == "") ? "1 as idz, " : String.Format("R{0}.id as idz, ", numberOfFilters));
            //queryend += (zType == "") ? "" : String.Format("{0} idz", queryendsep);

            queryMiddle.Append(generateAxisQueryForState(zType, zVertexId));
            queryMiddle.Append((zType == "")
                ? ""         // There is no z-value
                : ((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                ? ""  // This is the first entry, and there is nothing more
                : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                ? " join ("  // This is the first entry, and there is something more
                : (numberOfFilters == totalNumberOfFilters)
                ? $" on R1.object_id = R{numberOfFilters}.object_id " // Not first, but nothing more
                : $" on R1.object_id = R{numberOfFilters}.object_id join ("); // Not first, and something more

            queryFront.Append("max(R1.object_id) as object_id, count(distinct R1.object_id) as cnt ");
            queryEnd.Append(") X join cubeobjects O on X.object_id = O.id;");

            if (filtersList != null)
            {
                foreach (var filter in filtersList)
                {
                    numberOfFilters++;
                    queryMiddle.Append(generateFilterQueryForState(filter));
                    queryMiddle.Append(((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                        ? ""  // This is the first entry, and there is nothing more
                        : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                        ? " join ("  // This is the first entry, and there is something more
                        : (numberOfFilters == totalNumberOfFilters)
                        ? $" on R1.object_id = R{numberOfFilters}.object_id " // Not first, but nothing more
                        : $" on R1.object_id = R{numberOfFilters}.object_id join ("); // Not first, and something more
                }
            }

            string SQLQuery = queryFront.Append(queryMiddle.Append(queryEnd)).ToString();
            return SQLQuery;
        }

        internal string generateSQLQueryForCell(string xType, int xVertexId, string yType, int yVertexId, string zType, int zVertexId, IList<ParsedFilter>? filtersList)
        {
            numberOfAdditionalFilters = filtersList == null ? 0 : filtersList.Count;
            totalNumberOfFilters = numberOfAdditionalFilters + ((xType != "") ? 1 : 0) + ((yType != "") ? 1 : 0) + ((zType != "") ? 1 : 0);
            numberOfFilters = 0;

            if (totalNumberOfFilters == 0)
            {
                // No ordering here, would be very expensive!
                string BaseQuery = "select O.id as Id, O.file_uri as fileURI from cubeobjects O;";
                return BaseQuery;
            }

            var queryFront = new StringBuilder("select distinct O.id as Id, O.file_uri as fileURI, TS.name as T from (select R1.object_id ");
            var queryMiddle = new StringBuilder(" from (");
            var queryEnd = new StringBuilder(") X join cubeobjects O on X.object_id = O.id join objecttagrelations R2 on O.id = R2.object_id join timestamp_tags TS on R2.tag_id = TS.id order by TS.name;");

            numberOfFilters += (xType == "") ? 0 : 1;

            queryMiddle.Append(generateAxisQueryForCell(xType, xVertexId));
            queryMiddle.Append((xType == "")
                ? ""         // There is no x-value
                : (numberOfFilters == totalNumberOfFilters)
                ? ""         // There is an x-value, but nothing more
                : " join ("); // There is an x-value, and something more

            numberOfFilters += (yType == "") ? 0 : 1;

            queryMiddle.Append(generateAxisQueryForCell(yType, yVertexId));
            queryMiddle.Append((yType == "")
                ? ""         // There is no y-value
                : ((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                ? ""  // This is the first entry, and there is nothing more
                : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                ? " join ("  // This is the first entry, and there is something more
                : (numberOfFilters == totalNumberOfFilters)
                ? $" on R1.object_id = R{numberOfFilters}.object_id " // Not first, but nothing more
                : $" on R1.object_id = R{numberOfFilters}.object_id join ("); // Not first, and something more

            numberOfFilters += (zType == "") ? 0 : 1;

            queryMiddle.Append(generateAxisQueryForCell(zType, zVertexId));
            queryMiddle.Append((zType == "")
                ? ""         // There is no z-value
                : ((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                ? ""  // This is the first entry, and there is nothing more
                : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                ? " join ("  // This is the first entry, and there is something more
                : (numberOfFilters == totalNumberOfFilters)
                ? $" on R1.object_id = R{numberOfFilters}.object_id " // Not first, but nothing more
                : $" on R1.object_id = R{numberOfFilters}.object_id join ("); // Not first, and something more

            if (filtersList != null)
            {
                foreach (var filter in filtersList)
                {
                    numberOfFilters++;
                    queryMiddle.Append(generateFilterQueryForCell(filter));
                    queryMiddle.Append(((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                        ? ""  // This is the first entry, and there is nothing more
                        : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                        ? " join ("  // This is the first entry, and there is something more
                        : (numberOfFilters == totalNumberOfFilters)
                        ? $" on R1.object_id = R{numberOfFilters}.object_id " // Not first, but nothing more
                        : $" on R1.object_id = R{numberOfFilters}.object_id join ("); // Not first, and something more
                }
            }

            string SQLQuery = queryFront.Append(queryMiddle.Append(queryEnd)).ToString();
            Console.WriteLine(SQLQuery);
            return SQLQuery;
        }

        internal string generateSQLQueryForTimeline(IList<ParsedFilter>? filtersList)
        {
            numberOfAdditionalFilters = filtersList == null ? 0 : filtersList.Count;
            totalNumberOfFilters = numberOfAdditionalFilters;
            numberOfFilters = 0;

            if (totalNumberOfFilters != 1)
            {
                // No ordering here, would be very expensive!
                string BaseQuery = "select O.id as Id, O.file_uri as fileURI from cubeobjects O;";
                return BaseQuery;
            }

            var queryFront = new StringBuilder("select O.id as Id, O.file_uri as fileURI, TS1.name as T ");
            var queryMiddle = new StringBuilder("from cubeobjects O join objecttagrelations R1 on O.id = R1.object_id join timestamp_tags TS1 on R1.tag_id = TS1.id join timestamp_tags TS2 on TS1.name between TS2.name - interval '30 minutes' and TS2.name + interval '30 minutes' join objecttagrelations R2 on TS2.id = R2.tag_id where R2.object_id = ");
            var queryEnd = new StringBuilder(" order by TS1.name;");

            if (filtersList != null)
            {
                foreach (var filter in filtersList)
                {
                    queryMiddle.Append($"{ filter.Ids[0] }");
                }
            }

            string SQLQuery = queryFront.Append(queryMiddle.Append(queryEnd)).ToString();
            Console.WriteLine(SQLQuery);
            return SQLQuery;
        }

        private string generateAxisQueryForState(string type, int vertexId)
        {
            string query = "";
            switch (type)
            {
                case "node":
                    query +=
                        $" select N.object_id, N.node_id as id from nodes_taggings N where N.parentnode_id = {vertexId}) R{numberOfFilters} ";
                    break;
                case "tagset":
                    query +=
                        $" select T.object_id, T.tag_id as id from tagsets_taggings T where T.tagset_id = {vertexId}) R{numberOfFilters} ";
                    break;
            }
            return query;
        }

        private string generateAxisQueryForCell(string type, int vertexId)
        {
            string query = "";
            switch (type)
            {
                case "node":
                    query +=
                        $" select N.object_id from nodes_taggings N where N.node_id = {vertexId}) R{numberOfFilters} ";
                    break;
                case "tag":
                    query +=
                        $" select R.object_id from objecttagrelations R where R.tag_id = {vertexId}) R{numberOfFilters} ";
                    break;
            }
            return query;
        }

        private string generateFilterQueryForState(ParsedFilter filter)
        {
            string query = "";
            switch (filter.type)
            {
                case "node":
                    if (filter.Ids.Count == 1)
                    {
                        query +=
                            $" select N.object_id from nodes_taggings N where N.node_id = {filter.Ids[0]}) R{numberOfFilters}";
                    }
                    else
                    {
                        query +=
                            $" select N.object_id from nodes_taggings N where N.node_id in {generateIdList(filter)}) R{numberOfFilters}";
                    }
                    break;

                case "tagset":
                    if (filter.Ids.Count == 1)
                    {
                        query +=
                            $" select T.object_id from tagsets_taggings T where T.tagset_id = {filter.Ids[0]}) R{numberOfFilters}";
                    }
                    else
                    {
                        query +=
                            $" select T.object_id from tagsets_taggings T where T.tagset_id in {generateIdList(filter)}) R{numberOfFilters}";
                    }
                    break;

                case "tag":
                    if (filter.Ids.Count == 1)
                    {
                        query +=
                            $" select R.object_id from objecttagrelations R where R.tag_id = {filter.Ids[0]}) R{numberOfFilters}";
                    }
                    else
                    {
                        query +=
                            $" select R.object_id from objecttagrelations R where R.tag_id in {generateIdList(filter)}) R{numberOfFilters}";
                    }
                    break;

                case "numrange":
                    query +=
                        $" select R.object_id from numerical_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "")}) R{numberOfFilters}";
                    break;

                case "alpharange":
                    query +=
                        $" select R.object_id from alphanumerical_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "'")}) R{numberOfFilters}";
                    break;

                case "daterange":
                    query +=
                        $" select R.object_id from date_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "'")}) R{numberOfFilters}";
                    break;

                case "timerange":
                    query +=
                        $" select R.object_id from time_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "'")}) R{numberOfFilters}";
                    break;
                case "timestamprange":
                    query +=
                        $" select R.object_id from timestamp_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "'")}) R{numberOfFilters}";
                    break;
            }

            return query;
        }

        private string generateFilterQueryForCell(ParsedFilter filter)
        {
            string query = "";
            switch (filter.type)
            {
                case "node":
                    if (filter.Ids.Count == 1)
                    {
                        query +=
                            $" select N.object_id from nodes_taggings N where N.node_id = {filter.Ids[0]}) R{numberOfFilters}";
                    }
                    else
                    {
                        query +=
                            $" select N.object_id from nodes_taggings N where N.node_id in {generateIdList(filter)}) R{numberOfFilters}";
                    }
                    break;

                case "tagset":
                    if (filter.Ids.Count == 1)
                    {
                        query +=
                            $" select T.object_id from tagsets_taggings T where T.tagset_id = {filter.Ids[0]}) R{numberOfFilters}";
                    }
                    else
                    {
                        query +=
                            $" select T.object_id from tagsets_taggings T where T.tagset_id in {generateIdList(filter)}) R{numberOfFilters}";
                    }
                    break;

                case "tag":
                    if (filter.Ids.Count == 1)
                    {
                        query +=
                            $" select R.object_id from objecttagrelations R where R.tag_id = {filter.Ids[0]}) R{numberOfFilters}";
                    }
                    else
                    {
                        query +=
                            $" select R.object_id from objecttagrelations R where R.tag_id in {generateIdList(filter)}) R{numberOfFilters}";
                    }
                    break;

                case "numrange":
                    query +=
                        $" select R.object_id from numerical_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "")}) R{numberOfFilters}";
                    break;

                case "alpharange":
                    query +=
                        $" select R.object_id from alphanumerical_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "'")}) R{numberOfFilters}";
                    break;

                case "daterange":
                    query +=
                        $" select R.object_id from date_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "'")}) R{numberOfFilters}";
                    break;

                case "timerange":
                    query +=
                        $" select R.object_id from time_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "'")}) R{numberOfFilters}";
                    break;
                case "timestamprange":
                    query +=
                        $" select R.object_id from timestamp_tags T join objecttagrelations R on T.id = R.tag_id where {generateRangeList(filter, "'")}) R{numberOfFilters}";
                    break;
            }

            return query;
        }

        private string generateIdList(ParsedFilter filter)
        {
            var idList = new StringBuilder("(");
            string separator = "";

            foreach (var id in filter.Ids)
            {
                idList.Append(separator + id);
                separator = ", ";
            }

            idList.Append(')');
            return idList.ToString();
        }

        private string generateRangeList(ParsedFilter filter, string quote)
        {
            var rangeList = new StringBuilder("(");
            string separator = "";

            for (var i = 0; i < filter.Ids.Count; i++)
            {
                rangeList.Append(separator +
                                 $"T.tagset_id = {filter.Ids[i]} and T.name between {quote}{filter.Ranges[i][0]}{quote} and {quote}{filter.Ranges[i][1]}{quote}");
                separator = ") or (";
            }

            rangeList.Append(')');
            return rangeList.ToString();
        }
    }
}
