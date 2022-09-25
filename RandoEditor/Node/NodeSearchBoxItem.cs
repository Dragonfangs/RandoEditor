using Common.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoEditor.Node
{
    public class NodeSearchBoxItem
    {
        public NodeSearchBoxItem()
        {
        }

        public NodeSearchBoxItem(string name, NodeBase node)
        {
            DisplayText = name;
            Node = node;
        }

        public string DisplayText { get; set; } = "";
        public NodeBase Node { get; set; } = null;
    }
}
