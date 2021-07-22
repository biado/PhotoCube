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
        private int numberOfFilters; // cell definition filters + additional filters
        private string filterQuery;

        internal string generateSQLQueryForCell(string xType, int xVertexId, string yType, int yVertexId, string zType,
            int zVertexId)
        {
            numberOfFilters = numberOfAdditionalFilters;
            string SQLQuery = "select distinct(O.*) from cubeobjects O join (\n";
            string cellQuery = generateCellQuery(xType, xVertexId, yType, yVertexId, zType, zVertexId);
            SQLQuery += filterQuery + cellQuery;
            if (numberOfFilters == 1)
            {
                SQLQuery = Regex.Replace(SQLQuery, @"R\d", "");
            }
            SQLQuery += "\n) X on O.id = X.object_id limit 6;";
            return SQLQuery;
        }
        private string generateCellQuery(string xType, int xVertexId, string yType, int yVertexId, string zType,
            int zVertexId)
        {
            // If axis == null, parameters will be: type == "", id == -1
            string cellQuery = "";
            if (xType != "") cellQuery += generateVertexQuery(xType, xVertexId);
            if (yType != "") cellQuery += generateVertexQuery(yType, yVertexId);
            if (zType != "") cellQuery += generateVertexQuery(zType, zVertexId);
            return cellQuery;
        }

        private string generateVertexQuery(string type, int vertexId)
        {
            string query = (numberOfFilters == 0) ? "" : "\n natural join \n";
            switch (type)
            {
                case "Hierarchy":
                    query += String.Format("(select R.object_id from nodes_taggings R where R.node_id = {0}) R{1}", vertexId, numberOfFilters);
                    break;
                case "Tagset":
                    query += String.Format(
                        "(select R.object_id from objecttagrelations R where R.tag_id = {0}) R{1}", vertexId, numberOfFilters);
                    break;
            }
            numberOfFilters++;
            return query;
        }

        internal void generateFilterQuery(List<ParsedFilter> filtersList)
        {
            string query = "";
            string separator = "";
            foreach (var filter in filtersList)
            {
                query += separator + generateFilterQueryPerType(filter);
                numberOfAdditionalFilters++;

                separator = "\n natural join \n";
            }

            filterQuery = query;
        }

        private string generateFilterQueryPerType(ParsedFilter filter)
        {
            string query = "";
            switch (filter.type)
            {
                case "hierarchy":
                    query += String.Format(
                        "(select R.object_id from nodes_taggings R where R.node_id = {0}) R{1}", filter.Ids[0], numberOfAdditionalFilters);
                    break;

                case "tag":
                case "date":
                    query += String.Format(
                        "(select R.object_id from objecttagrelations R where R.tag_id = {0}) R{1}", filter.Ids[0], numberOfAdditionalFilters);
                    break;

                case "tagset":
                case "day of week":
                case "time":
                    query += String.Format(
                        "(select R.object_id from objecttagrelations R where R.tag_id in {0}) R{1}", generateIdList(filter), numberOfAdditionalFilters);
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

        internal void reset()
        {
            numberOfAdditionalFilters = 0;
            numberOfFilters = 0;
            filterQuery = "";
        }
    }
}
