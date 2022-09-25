using Common.Node;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandoEditor.Node
{
    public class NodeSearcher
    {
        public delegate void SearchDelegate();

        public event SearchDelegate SearchComplete;

        private NodeCollection myNodeCollection;

        private bool searchStringDirty = false;
        private string searchString;

        private bool cancelThread;

        public NodeSearcher(NodeCollection collection)
        {
            myNodeCollection = collection;
            
            Task.Run(() => Worker());
        }

        public List<NodeBase> SearchResult { get; set; } = new List<NodeBase>();
        public string ResultString { get; set; } = "";

        public void Search(string query)
        {
            searchString = query;
            searchStringDirty = true;
        }

        public void Stop()
        {
            cancelThread = true;
        }

        private async void Worker()
        {
            while (true)
            {
                if (cancelThread)
                    return;

                if (!searchStringDirty)
                {
                    await Task.Delay(200);
                    continue;
                }

                searchStringDirty = false;

                var localSearchString = searchString;

                List<NodeBase> result = myNodeCollection.myNodes.Where(node =>
                {
                    if (node is KeyNode keyNode)
                    {
                        return keyNode.Name().ToLowerInvariant().Contains(searchString.ToLowerInvariant());
                    }
                    else if (node is LockNode lockNode)
                    {
                        return lockNode.myRequirement.ContainsKeyWithString(searchString);
                    }

                    return false;
                }).ToList();

                ResultString = localSearchString;
                SearchResult = result;

                SearchComplete?.Invoke();
            }
        }
    }
}