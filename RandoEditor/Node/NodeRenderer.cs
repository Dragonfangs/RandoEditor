using Common.Node;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RandoEditor.Node
{
	public class NodeRenderer
	{
		private NodeRenderOptions myRenderOptions;

		public static int nodeSize = 25;

		private NodeImageFactory myNodeImageFactory = new NodeImageFactory(nodeSize);

		private int TranslateX(float x)
		{
			return (int)((myRenderOptions.BasePos.x + x) * myRenderOptions.Zoom);
		}

		private int TranslateY(float y)
		{
			return (int)((myRenderOptions.BasePos.y + y) * myRenderOptions.Zoom);
		}

		public void RenderNodes(List<NodeBase> nodes, Graphics graphicsObj, NodeRenderOptions renderOptions)
		{
			myRenderOptions = renderOptions;

			NodeBase selectedNode = null;
			NodeBase highlightedNode = null;

			List<Connection> highlightedConnections = new List<Connection>();
			List<Connection> selectedConnections = new List<Connection>();

			foreach (var node in nodes)
			{
				foreach (var conn in node.myConnections)
				{
					var connection = new Connection(node, conn);

					if (connection.Equals(myRenderOptions.highlightedConnection))
					{
						highlightedConnections.Add(connection);
					}
					else if (connection.Equals(myRenderOptions.selectedConnection))
					{
						selectedConnections.Add(connection);
					}
					else if (node.id != myRenderOptions.selectedNodeId)
					{
						DrawConnection(connection, Color.HotPink, graphicsObj);
					}
				}

				//Draw selectednode and highlighted node last (on top)
				if (node.id == myRenderOptions.selectedNodeId)
				{
					selectedNode = node;
					continue;
				}

				if (node.id == myRenderOptions.highlightedNodeId)
				{
					highlightedNode = node;
					continue;
				}

				DrawNode(node, graphicsObj);
			}

			foreach (var connection in highlightedConnections)
			{
				DrawConnection(connection, Color.DodgerBlue, graphicsObj);
			}

			foreach (var connection in selectedConnections)
			{
				DrawConnection(connection, Color.DarkSeaGreen, graphicsObj);
			}

			if (selectedNode != null)
			{
				DrawNode(selectedNode, graphicsObj);

				foreach (var conn in selectedNode.myConnections)
				{
					var connection = new Connection(selectedNode, conn);
					if (myRenderOptions.highlightedConnection == null || !connection.Equals(myRenderOptions.highlightedConnection))
					{
						DrawConnection(connection, Color.Yellow, graphicsObj);
					}
				}
			}

			if (highlightedNode != null)
			{
				DrawNode(highlightedNode, graphicsObj);
			}
		}

		public void DrawCursorNode(Vector2 aPos, NodeType aType, Graphics aGraphicsObj)
		{
			var width = nodeSize;
			var height = nodeSize;

			var nodeRectangle = new Rectangle((int)aPos.x - width / 2, (int)aPos.y - height / 2, width, height);

			var panelRect = new Rectangle(new Point(0, 0), myRenderOptions.panelSize);
			var img = myNodeImageFactory.GetNodeImage(new NodeImageFactory.NodeInfo()
			{
				type = aType,

				selected = false,
				carried = true,
			});

			aGraphicsObj.DrawImage(img, nodeRectangle);
		}

		private void DrawNode(NodeBase aNode, Graphics aGraphicsObj)
		{
			var nodeInfo = new NodeImageFactory.NodeInfo()
			{
				type = aNode.myNodeType,

				selected = aNode.id == myRenderOptions.selectedNodeId,
				carried = aNode.id == myRenderOptions.carriedNodeId,
				highlighted = aNode.id == myRenderOptions.highlightedNodeId,
			};

			var renderPos = nodeInfo.carried ? myRenderOptions.carriedPos : aNode.myPos;

			var x = TranslateX(renderPos.x);
			var y = TranslateY(renderPos.y);
			var width = nodeSize;
			var height = nodeSize;

			var nodeRectangle = new Rectangle(x - width / 2, y - height / 2, width, height);

			var panelRect = new Rectangle(new Point(0, 0), myRenderOptions.panelSize);
			if (Utility.RectIntersect(nodeRectangle, panelRect))
			{
				if ((bool)Properties.Settings.Default["SimpleNodeGraphics"])
				{
					var color = myNodeImageFactory.GetNodeColor(nodeInfo);

					var brush = new SolidBrush(color);
					aGraphicsObj.FillRectangle(brush, nodeRectangle);
				}
				else
				{
					var img = myNodeImageFactory.GetNodeImage(nodeInfo);

					aGraphicsObj.DrawImage(img, nodeRectangle);
				}
			}
		}

		public void DrawCursorOneWayConnection(NodeBase startNode, Vector2 cursorPoint, Graphics aGraphicsObj)
		{
			DrawCursorConnection(startNode, cursorPoint, aGraphicsObj, false);
		}

		public void DrawCursorTwoWayConnection(NodeBase startNode, Vector2 cursorPoint, Graphics aGraphicsObj)
		{
			DrawCursorConnection(startNode, cursorPoint, aGraphicsObj, true);
		}

		private void DrawCursorConnection(NodeBase startNode, Vector2 cursorPoint, Graphics aGraphicsObj, bool twoWay)
		{
			var startPoint = new Vector2(TranslateX(startNode.myPos.x), TranslateY(startNode.myPos.y));

			var angle = Utility.AngleBetween(new Vector2(1, 0), startPoint - cursorPoint);

			// Adjust connection point to the sides of node graphic rather than center
			if (angle > -45 && angle < 45)
			{
				startPoint.x -= nodeSize / 2;
			}
			else if (angle > -135 && angle < -45)
			{
				startPoint.y += nodeSize / 2;
			}
			else if (angle > 45 && angle < 135)
			{
				startPoint.y -= nodeSize / 2;
			}
			else
			{
				startPoint.x += nodeSize / 2;
			}

			DrawConnection(startPoint, cursorPoint, Color.Yellow, aGraphicsObj);
			if (twoWay)
			{
				DrawConnection(cursorPoint, startPoint, Color.Yellow, aGraphicsObj);
			}
		}

		private void DrawConnection(Vector2 startPoint, Vector2 endPoint, Color aColor, Graphics aGraphicsObj)
		{
			var panelRect = new Rectangle(new Point(0, 0), myRenderOptions.panelSize);
			if (Utility.LineInRect(panelRect, startPoint.ToPoint(), endPoint.ToPoint()))
			{
				var myPen = new Pen(aColor, nodeSize / 16);
				aGraphicsObj.DrawLine(myPen, startPoint.ToPoint(), endPoint.ToPoint());

				//Draw arrow end
				var scale = 10f;
				var line = (endPoint - startPoint);
				if (line.Magnitude() > 50)
				{
					line = line.Normalized() * scale;
					var line1 = Utility.Rotate(line, (float)(Math.PI * (14f / 16f)));
					var line2 = Utility.Rotate(line, (float)(-Math.PI * (14f / 16f)));

					var arrowBrush = new SolidBrush(aColor);
					aGraphicsObj.FillPolygon(arrowBrush, new Point[3] { endPoint.ToPoint(),
					(endPoint+line1).ToPoint(),
					(endPoint+line2).ToPoint()});
				}
			}
		}

		private void DrawConnection(Connection aConnection, Color aColor, Graphics aGraphicsObj)
		{
			var startPos = aConnection.node1.id == myRenderOptions.carriedNodeId ? myRenderOptions.carriedPos : aConnection.node1.myPos;
			var endPos = aConnection.node2.id == myRenderOptions.carriedNodeId ? myRenderOptions.carriedPos : aConnection.node2.myPos;

			var startPoint = new Vector2(TranslateX(startPos.x), TranslateY(startPos.y));
			var endPoint = new Vector2(TranslateX(endPos.x), TranslateY(endPos.y));

			if ((startPoint - endPoint).Magnitude() < nodeSize)
				return;

			var angle = Utility.AngleBetween(new Vector2(1, 0), startPoint - endPoint);

			// Adjust connection point to the sides of node graphic rather than center
			if (angle > -45 && angle < 45)
			{
				startPoint.x -= nodeSize / 2;
				endPoint.x += nodeSize / 2;
			}
			else if (angle > -135 && angle < -45)
			{
				startPoint.y += nodeSize / 2;
				endPoint.y -= nodeSize / 2;
			}
			else if (angle > 45 && angle < 135)
			{
				startPoint.y -= nodeSize / 2;
				endPoint.y += nodeSize / 2;
			}
			else
			{
				startPoint.x += nodeSize / 2;
				endPoint.x -= nodeSize / 2;
			}

			DrawConnection(startPoint, endPoint, aColor, aGraphicsObj);
		}
	}
}
