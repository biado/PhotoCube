using ObjectCubeServer.Models.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObjectCubeServer.Services
{
    public class QueryGenerationService
    {
        private int numberOfAdditionalFilters;
        private int totalNumberOfFilters;  // cell definition filters + additional filters
        private int numberOfFilters;  // internal counter
        //private string filterQuery;

        internal string generateSQLQueryForCells(string xType, int xVertexId, string yType, int yVertexId, string zType, int zVertexId, List<ParsedFilter> filtersList)
        {
            numberOfAdditionalFilters = filtersList?.Count ?? 0;
            totalNumberOfFilters = numberOfAdditionalFilters + ((xType != "") ? 1 : 0) + ((yType != "") ? 1 : 0) + ((zType != "") ? 1 : 0);
            numberOfFilters = 0;

            if (totalNumberOfFilters == 0)
            {
                const string baseQuery = "select X.idx as x, X.idy as y, X.idz as z, X.object_id as id, O.file_uri as fileURI, X.cnt as count from(select 1 as idx, 1 as idy, 1 as idz, max(R1.id) as object_id, count(*) as cnt from cubeobjects R1 group by idx, idy, idz) X join cubeobjects O on X.object_id = O.id;";
                return baseQuery;
            }

            string queryfront = "select X.idx as x, X.idy as y, X.idz as z, X.object_id as id, O.file_uri as fileURI, X.cnt as count from (select ";
            string querymiddle = " from (";
            string queryend = " group by idx, idy, idz";
            //string queryendsep = "";

            numberOfFilters += (xType == "") ? 0 : 1;
            queryfront += (xType == "") ? "1 as idx, " : String.Format("R{0}.id as idx, ", numberOfFilters);
            //queryend += (xType == "") ? "" : " idx";
            //queryendsep = (xType == "") ? queryendsep : ", ";

            querymiddle += generateVertexQuery(xType, xVertexId);
            querymiddle += (xType == "")
                ? ""         // There is no x-value
                : (numberOfFilters == totalNumberOfFilters)
                ? ""         // There is an x-value, but nothing more
                : " join ("; // There is an x-value, and something more

            numberOfFilters += (yType == "") ? 0 : 1;
            queryfront += (yType == "") ? "1 as idy, " : String.Format("R{0}.id as idy, ", numberOfFilters);
            //queryend += (yType == "") ? "" : String.Format("{0} idy", queryendsep);
            //queryendsep = (yType == "") ? queryendsep : ", ";

            querymiddle += generateVertexQuery(yType, yVertexId);
            querymiddle += (yType == "")
                ? ""         // There is no y-value
                : ((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                ? ""  // This is the first entry, and there is nothing more
                : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                ? " join ("  // This is the first entry, and there is something more
                : (numberOfFilters == totalNumberOfFilters)
                ? String.Format(" on R1.object_id = R{0}.object_id ", numberOfFilters)        // Not first, but nothing more
                : String.Format(" on R1.object_id = R{0}.object_id join (", numberOfFilters); // Not first, and something more

            numberOfFilters += (zType == "") ? 0 : 1;
            queryfront += (zType == "") ? "1 as idz, " : String.Format("R{0}.id as idz, ", numberOfFilters);
            //queryend += (zType == "") ? "" : String.Format("{0} idz", queryendsep);

            querymiddle += generateVertexQuery(zType, zVertexId);
            querymiddle += (zType == "")
                ? ""         // There is no z-value
                : ((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                ? ""  // This is the first entry, and there is nothing more
                : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                ? " join ("  // This is the first entry, and there is something more
                : (numberOfFilters == totalNumberOfFilters)
                ? String.Format(" on R1.object_id = R{0}.object_id ", numberOfFilters)        // Not first, but nothing more
                : String.Format(" on R1.object_id = R{0}.object_id join (", numberOfFilters); // Not first, and something more

            queryfront += "max(R1.object_id) as object_id, count(distinct R1.object_id) as cnt ";
            queryend += ") X join cubeobjects O on X.object_id = O.id;";

            if (filtersList != null)
            {
                foreach (var filter in filtersList)
                {
                    numberOfFilters++;
                    querymiddle += generateFilterQueryPerType(filter);
                    querymiddle += ((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                        ? ""  // This is the first entry, and there is nothing more
                        : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                        ? " join ("  // This is the first entry, and there is something more
                        : (numberOfFilters == totalNumberOfFilters)
                        ? String.Format(" on R1.object_id = R{0}.object_id ", numberOfFilters)        // Not first, but nothing more
                        : String.Format(" on R1.object_id = R{0}.object_id join (", numberOfFilters); // Not first, and something more
                }
            }

            string SQLQuery = queryfront + querymiddle + queryend;
            Console.WriteLine(SQLQuery);
            return SQLQuery;
        }

        internal string generateSQLQueryForObjects(List<ParsedFilter> filtersList)
        {
            numberOfAdditionalFilters = (filtersList == null) ? 0 : filtersList.Count;
            totalNumberOfFilters = numberOfAdditionalFilters;
            numberOfFilters = 0;

            if (totalNumberOfFilters == 0)
            {
                string BaseQuery = "select O.id as Id, O.file_uri as fileURI from cubeobjects O;";
                Console.Write(BaseQuery);
                return BaseQuery;
            }

            string queryfront = "select distinct O.id as Id, O.file_uri as fileURI from (select R1.object_id ";
            string querymiddle = " from (";
            string queryend = ") X join cubeobjects O on X.object_id = O.id;";

            if (filtersList != null)
            {
                foreach (var filter in filtersList)
                {
                    numberOfFilters++;
                    querymiddle += generateFilterQueryPerTypeObjects(filter);
                    querymiddle += ((numberOfFilters == 1) && (numberOfFilters == totalNumberOfFilters))
                        ? ""  // This is the first entry, and there is nothing more
                        : ((numberOfFilters == 1) && (numberOfFilters < totalNumberOfFilters))
                        ? " join ("  // This is the first entry, and there is something more
                        : (numberOfFilters == totalNumberOfFilters)
                        ? String.Format(" on R1.object_id = R{0}.object_id ", numberOfFilters)        // Not first, but nothing more
                        : String.Format(" on R1.object_id = R{0}.object_id join (", numberOfFilters); // Not first, and something more
                }
            }

            string SQLQuery = queryfront + querymiddle + queryend;
            Console.WriteLine(SQLQuery);
            return SQLQuery;
        }

        private string generateVertexQuery(string type, int vertexId)
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

        private string generateFilterQueryPerType(ParsedFilter filter)
        {
            string query = "";
            switch (filter.type)
            {
                case "node":
                    if (filter.Ids.Count == 1)
                    {
                        query +=
                            $" select N.object_id from nodes_taggings N where N.parentnode_id = {filter.Ids[0]}) R{numberOfFilters}";
                    }
                    else
                    {
                        query +=
                            $" select N.object_id from nodes_taggings N where N.parentnode_id in {generateIdList(filter)}) R{numberOfFilters}";
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
            }

            return query;
        }

        private string generateFilterQueryPerTypeObjects(ParsedFilter filter)
        {
            string query = "";
            switch (filter.type)
            {
                case "node":
                    if (filter.Ids.Count == 1)
                    {
                        query += String.Format(" select N.object_id from nodes_taggings N where N.node_id = {0}) R{1}", filter.Ids[0], numberOfFilters);
                    }
                    else
                    {
                        query += String.Format(" select N.object_id from nodes_taggings N where N.node_id in {0}) R{1}", generateIdList(filter), numberOfFilters);
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
            }

            return query;
        }

        private string generateIdList(ParsedFilter filter)
        {
            string idList = "(";
            string separator = "";

            foreach (var id in filter.Ids)
            {
                idList += separator + id.ToString();
                separator = ", ";
            }

            idList += ")";
            return idList;
        }

        private string generateRangeList(ParsedFilter filter, string quote)
        {
            string rangeList = "(";
            string separator = "";

            for (var i = 0; i < filter.Ids.Count(); i++)
            {
                rangeList += separator + String.Format("T.tagset_id = {0} and T.name between {3}{1}{3} and {3}{2}{3}", filter.Ids[i], filter.Ranges[i][0], filter.Ranges[i][1], quote);
                separator = ") or (";
            }

            rangeList += ")";
            return rangeList;
        }
    }
}
