﻿using Common.Node;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RandoEditor.Node
{
	public class NodeRenderer
	{
		// This ain't pretty but better than passing a bunch of values around right now
		// Fix when it becomes a problem lol
		public Vector2 BasePos;
		public Vector2 CarriedPos;
		public float Zoom;
		public Guid carriedNodeId;
		public Guid selectedNodeId;
		public Size panelSize;
		
		public static int nodeSize = 25;

		private NodeImageFactory myNodeImageFactory = new NodeImageFactory(nodeSize);

		private int TranslateX(float x)
		{
			return (int)((BasePos.x + x) * Zoom);
		}

		private int TranslateY(float y)
		{
			return (int)((BasePos.y + y) * Zoom);
		}

		public void RenderNodes(List<NodeBase> nodes, Graphics graphicsObj)
		{
			var connections = new List<Connection>();

			NodeBase selected = null;

			foreach (var node in nodes)
			{
				//Draw selectednode last (on top)
				if (node.id == selectedNodeId)
				{
					selected = node;
					continue;
				}

				foreach (var conn in node.myConnections)
				{
					connections.Add(new Connection(node, conn));
				}

				DrawNode(node, graphicsObj);
			}

			foreach (var connection in connections)
			{
				DrawConnection(connection, Color.HotPink, graphicsObj);
			}

			if (selected != null)
			{
				DrawNode(selected, graphicsObj);

				foreach (var conn in selected.myConnections)
				{
					DrawConnection(new Connection(selected, conn), Color.Yellow, graphicsObj);
				}
			}
		}

		public void DrawCursorNode(Vector2 aPos, NodeType aType, Graphics aGraphicsObj)
		{
			var width = nodeSize;
			var height = nodeSize;

			var nodeRectangle = new Rectangle((int)aPos.x - width / 2, (int)aPos.y - height / 2, width, height);

			var panelRect = new Rectangle(new Point(0, 0), panelSize);
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

				selected = aNode.id == selectedNodeId,
				carried = aNode.id == carriedNodeId,
			};

			var renderPos = nodeInfo.carried ? CarriedPos : aNode.myPos;

			var x = TranslateX(renderPos.x);
			var y = TranslateY(renderPos.y);
			var width = nodeSize;
			var height = nodeSize;

			var nodeRectangle = new Rectangle(x - width / 2, y - height / 2, width, height);

			var panelRect = new Rectangle(new Point(0, 0), panelSize);
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
			var panelRect = new Rectangle(new Point(0, 0), panelSize);
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
			var startPos = aConnection.node1.id == carriedNodeId ? CarriedPos : aConnection.node1.myPos;
			var endPos = aConnection.node2.id == carriedNodeId ? CarriedPos : aConnection.node2.myPos;

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
