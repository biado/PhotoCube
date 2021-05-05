using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleAppForInteractingWithDatabase
{
    class JsonHierarchyParser
    {
        private NameValueCollection sAll = ConfigurationManager.AppSettings;
        public JSNode root { get; set; }
        public Dictionary<string, HashSet<JSNode>> equalsCheck { get; set; }

        public JsonHierarchyParser()
        {
            buildRoot();
            setParentJSNodes();
            //equalsCheck = new Dictionary<string, HashSet<JSNode>>();
            //buildEqualsCheckMap();
        }

        private void setParentJSNodes()
        {
            // "root" has 3 children: Timezone, Day of week, Entity. These 3 becomes top nodes. We don't store "root" in the database.
            foreach (JSNode rootChild in root.children)
            {
                setParentJSNodes_Recursive(root, rootChild);
            }
        }

        private void setParentJSNodes_Recursive(JSNode parent, JSNode current)
        {
            current.parentJSNode = parent; // if parent == null, it is the top-most node. (rootChild)
            if (current.children != null) // if it has children (aka not a leaf)
            {
                foreach (JSNode child in current.children)
                {
                    setParentJSNodes_Recursive(current, child);
                }
            }
        }

        private void buildRoot()
        {
            string LscJsonHierarchyPath = sAll.Get("LscJsonHierarchyPath");
            using (StreamReader reader = new StreamReader(LscJsonHierarchyPath))
            {
                string json = reader.ReadToEnd();
                root = JsonConvert.DeserializeObject<JSNode>(json);
            }
        }

        private void buildEqualsCheckMap()
        {
            // "root" has 3 children: Timezone, Day of week, Entity. These 3 becomes top nodes. We don't store "root" in the database.
            foreach (JSNode rootChild in root.children)
            {
                buildEqualsCheckMap_Recursive(rootChild);
            }
        }

        private void buildEqualsCheckMap_Recursive(JSNode current)
        {
            putInEqualsCheckMap(current);
            if (current.children != null)
            {
                foreach (JSNode child in current.children)
                {
                    buildEqualsCheckMap_Recursive(child);
                }
            }
        }

        private void putInEqualsCheckMap(JSNode current)
        {
            string nodeName = current.name;
            HashSet<JSNode> nodeSet =
                (equalsCheck.ContainsKey(nodeName)) ? equalsCheck[nodeName] : new HashSet<JSNode>();
            nodeSet.Add(current);
            equalsCheck[nodeName] = nodeSet;
        }

        //static void Main(string[] args)
        //{
        //    JsonHierarchyParser parser = new JsonHierarchyParser();
        //    foreach (string nodeName in parser.equalsCheck.Keys)
        //    {
        //        Console.WriteLine(nodeName + ": " + parser.equalsCheck[nodeName].Count);
        //    }
        //}
    }
}
