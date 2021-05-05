using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppForInteractingWithDatabase
{
    class JSNode
    {
        public string name { get; set; }
        public int? id { get; set; }
        public JSNode[] children { get; set; }
        public JSNode parentJSNode { get; set; }

        protected bool Equals(JSNode other)
        {
            return name == other.name && id == other.id && Equals(children, other.children) && Equals(parentJSNode, other.parentJSNode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JSNode)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, id, children, parentJSNode);
        }
    }
}
