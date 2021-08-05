using Common.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verifier.Key;

namespace Verifier
{
	public class FillSearcher
	{
		private Queue<NodeBase> searchQueue = new Queue<NodeBase>();
		private List<NodeBase> visitedNodes = new List<NodeBase>();
		private List<NodeBase> lockedNodes = new List<NodeBase>();

		private bool isNew = true;

		public FillSearcher()
		{
		}

		public FillSearcher(FillSearcher other)
		{
			searchQueue = new Queue<NodeBase>(other.searchQueue);
			visitedNodes = new List<NodeBase>(other.visitedNodes);
			lockedNodes = new List<NodeBase>(other.lockedNodes);

			isNew = other.isNew;
		}

        public List<NodeBase> NewSearch(NodeBase startNode, Inventory anInventory)
        {
            return NewSearch(startNode, anInventory, node => true);
        }

        public List<NodeBase> NewSearch(NodeBase startNode, Inventory anInventory, Func<NodeBase, bool> predicate)
		{
            if (startNode == null)
                return new List<NodeBase>();

			searchQueue.Clear();
			visitedNodes.Clear();
			lockedNodes.Clear();
			
			searchQueue.Enqueue(startNode);

			isNew = false;

			return Search(anInventory, predicate);
		}

        public List<NodeBase> ContinueSearch(NodeBase startNode, Inventory anInventory)
        {
            return ContinueSearch(startNode, anInventory, node => true);
        }

        public List<NodeBase> ContinueSearch(NodeBase startNode, Inventory anInventory, Func<NodeBase, bool> predicate)
        {
            if (isNew)
                return NewSearch(startNode, anInventory, predicate);

            return ContinueSearch(anInventory, predicate);
        }

        public List<NodeBase> ContinueSearch(Inventory anInventory)
        {
            return ContinueSearch(anInventory, node => true);
        }

        public List<NodeBase> ContinueSearch(Inventory anInventory, Func<NodeBase, bool> predicate)
        {
            searchQueue.Clear();

            foreach (var node in lockedNodes)
            {
                visitedNodes.Remove(node);
                searchQueue.Enqueue(node);
            }

            lockedNodes.Clear();

            return Search(anInventory, predicate);
        }

		private List<NodeBase> Search(Inventory anInventory, Func<NodeBase, bool> predicate)
		{
			var searchResult = new List<NodeBase>();
			
			while (searchQueue.Count() > 0)
			{
				var currentNode = searchQueue.Dequeue();

				visitedNodes.Add(currentNode);
				
				if (predicate(currentNode))
					searchResult.Add(currentNode);

				if (currentNode is LockNode lockNode && !anInventory.Unlocks(lockNode.myRequirement))
				{
					lockedNodes.Add(currentNode);
					continue;
				}

				foreach (var connection in currentNode.myConnections)
				{
					if (!visitedNodes.Contains(connection) && !searchQueue.Contains(connection))
					{
						searchQueue.Enqueue(connection);
					}
				}
			}

			return searchResult;
		}
	}
}
