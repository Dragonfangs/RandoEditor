using RandoEditor.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoEditor.Node
{
	
	public class NodeImageFactory
	{
		public class NodeInfo
		{
			public NodeType type;
			public bool carried;
			public bool selected;
		}

		public Bitmap blankStandard;
		public Bitmap blankSelected;
		public Bitmap blankCarried;

		public Bitmap randomStandard;
		public Bitmap randomSelected;
		public Bitmap randomCarried;

		public Bitmap eventStandard;
		public Bitmap eventSelected;
		public Bitmap eventCarried;

		public Bitmap lockStandard;
		public Bitmap lockSelected;
		public Bitmap lockCarried;

		public NodeImageFactory(int nodeSize)
		{
			LoadNodeIcons(nodeSize);
		}

		public void LoadNodeIcons(int nodeSize)
		{
			blankStandard = Utility.ResizeImage(Properties.Resources.crossroads, nodeSize, nodeSize);
			blankSelected = Utility.ResizeImage(Properties.Resources.crossroads_selected, nodeSize, nodeSize);
			blankCarried = Utility.ResizeImage(Properties.Resources.crossroads_carried, nodeSize, nodeSize);

			randomStandard = Utility.ResizeImage(Properties.Resources.question, nodeSize, nodeSize);
			randomSelected = Utility.ResizeImage(Properties.Resources.question_selected, nodeSize, nodeSize);
			randomCarried = Utility.ResizeImage(Properties.Resources.question_carried, nodeSize, nodeSize);

			eventStandard = Utility.ResizeImage(Properties.Resources.exclamation, nodeSize, nodeSize);
			eventSelected = Utility.ResizeImage(Properties.Resources.exclamation_selected, nodeSize, nodeSize);
			eventCarried = Utility.ResizeImage(Properties.Resources.exclamation_carried, nodeSize, nodeSize);

			lockStandard = Utility.ResizeImage(Properties.Resources.keyhole, nodeSize, nodeSize);
			lockSelected = Utility.ResizeImage(Properties.Resources.keyhole_selected, nodeSize, nodeSize);
			lockCarried = Utility.ResizeImage(Properties.Resources.keyhole_carried, nodeSize, nodeSize);
		}

		public Bitmap GetNodeImage(NodeInfo info)
		{
			if (info.type == NodeType.Blank)
			{
				if (info.carried)
					return blankCarried;
				if (info.selected)
					return blankSelected;

				return blankStandard;
			}

			if (info.type == NodeType.RandomKey)
			{
				if (info.carried)
					return randomCarried;
				if (info.selected)
					return randomSelected;

				return randomStandard;
			}

			if (info.type == NodeType.EventKey)
			{
				if (info.carried)
					return eventCarried;
				if (info.selected)
					return eventSelected;

				return eventStandard;
			}

			if (info.type == NodeType.Lock)
			{
				if (info.carried)
					return lockCarried;
				if (info.selected)
					return lockSelected;

				return lockStandard;
			}

			return null;
		}
	}
}
