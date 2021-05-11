using Common.Utils;
using Common.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Key;

namespace Verifier
{
	public class WaveLog
	{
		List<List<NodeBase>> liveLog = new List<List<NodeBase>>();
		List<WaveLog> deadLogs = new List<WaveLog>();

		public void Clear()
		{
			liveLog.Clear();
			deadLogs.Clear();
		}

		public void AddLive(NodeBase node)
		{
			liveLog.Add(new List<NodeBase> { node });
		}

		public void AddLive(List<NodeBase> wave)
		{
			liveLog.Add(wave);
		}

		public void AddLive(WaveLog waves)
		{
			liveLog.AddRange(waves.liveLog);
		}

		public void ClearDead()
		{
			deadLogs.Clear();
		}

		public void AddDead(WaveLog wave)
		{
			deadLogs.Add(wave);
		}

		public string Print(int depth = 0, string prefix = "")
		{
			// Filter out any empty nodes
			var filledLog = liveLog.Select(nodelist => nodelist.Where(node => !(node is RandomKeyNode keyNode) || (keyNode.GetKey() is BaseKey))).Where(nodeList => nodeList.Any());

			// Change all waves to a list of names of nodes visited
			// (Still prints "empty" as name for missing keys as a redundant precaution)
			var livePrints = filledLog.Select(nodeList => nodeList.Select(node => Utility.GetNodeName(node) + (node is RandomKeyNode keyNode ? $"({(keyNode.GetKey() is BaseKey key ? key.Name : "empty")})" : string.Empty)).Aggregate((i, j) => i + ", " + j));

			// Add "Wave" titles
			var formattedPrints = livePrints.Select((print, i) => $"{new string('\t', depth)}Wave {prefix}{i + 1}: {print}");

			// Aggregate all live waves into one string
			var fullPrint = formattedPrints.Any() ? formattedPrints.Aggregate((i, j) => i + Environment.NewLine + j) : "";

			// Recursively print out paths that led to a dead end
			var deadPrints = deadLogs.Select((log, i) => AddDeadEnd(log.Print(depth + 1,$"{prefix}{formattedPrints.Count() + i}.")));

			// If any dead ends, add to printed string
			if (deadPrints.Any())
			{
				fullPrint = $"{fullPrint} (No more guaranteed){Environment.NewLine}{deadPrints.Aggregate((i, j) => i + Environment.NewLine + j)}";
			}

			return fullPrint;
		}

		private string AddDeadEnd(string text)
		{
			return text.EndsWith("(Dead End)") ? text : text + " (Dead End)";
		}
	}
}
