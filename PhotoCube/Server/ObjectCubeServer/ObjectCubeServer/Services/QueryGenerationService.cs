using ObjectCubeServer.Models.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectCubeServer.Services
{
    public class QueryGenerationService
    {
        private int numberOfAdditionalFilters;
        private int numberOfCellDefinitionFilters;
        internal string generateFilterQuery(List<ParsedFilter> filtersList)
        {
            string query = "";
            string separator = "";
            foreach (var filter in filtersList)
            {
                query += separator + generateFilterQueryPerType(filter);
                numberOfAdditionalFilters++;

                separator = "\n natural join \n";
            }

            return query;
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

        internal void resetNumberOfAdditionalFilters()
        {
            numberOfAdditionalFilters = 0;
        }
    }
}
