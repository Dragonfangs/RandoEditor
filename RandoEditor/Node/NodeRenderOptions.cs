using Common.Utils;
using System;
using System.Drawing;

namespace RandoEditor.Node
{
	public struct NodeRenderOptions
	{
		public Vector2 BasePos;
		public float Zoom;
		public Guid carriedNodeId;
		public Vector2 carriedPos;
		public Guid selectedNodeId;
		public Guid highlightedNodeId;
		public Connection selectedConnection;
		public Connection highlightedConnection;
		public Size panelSize;
	}
}
