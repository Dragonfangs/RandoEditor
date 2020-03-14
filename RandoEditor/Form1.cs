using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Utils;
using RandoEditor.Map;
using Common.Node;
using Common.Key;
using RandoEditor.SaveData;
using RandoEditor.Node;
using Common.Memento;

namespace RandoEditor
{
	public partial class Form1 : Form
	{
		private enum PointerState
		{
			None = 0,
			PlaceBlank,
			PlaceLock,
			PlaceRandom,
			PlaceEvent,
			OneWay,
			TwoWay,
		}

		private AreaMap myMap = new AreaMap();
		private NodeRenderer myNodeRenderer = new NodeRenderer();
		private NodeCollection myNodeCollection = new NodeCollection();
		private List<NodeMemento> myMementos = new List<NodeMemento>();
		private NodeBase carriedNode = null;
		private NodeBase selectedNode = null;

		private Vector2 imageBasePos = new Vector2(0, 0);
		private Vector2 mapPickedUpPos = new Vector2(0, 0);
		private bool carriedMap = false;

		private Vector2 myMousePos = new Vector2(0, 0);

		private Vector2 selectedOffset = null;
		private DateTime mouseDownTimeStamp = DateTime.MinValue;
		private static TimeSpan clickTreshold = TimeSpan.FromMilliseconds(150);

		private float baseZoomScale = 0.1f;
		private float ZoomScale { get { return baseZoomScale * (Utility.CalcDiag(panel1.Width, panel1.Height) / 1000f); } set { baseZoomScale = value; } }

		private PointerState myPointerState = PointerState.None;

		public Form1()
		{
			InitializeComponent();

			if (!SaveManager.Open((string)Properties.Settings.Default["LatestFilePath"]))
				SaveManager.New();

			LoadData();

			(panel1 as Control).KeyDown += new KeyEventHandler(panel1_KeyDown);
			(panel1 as Control).KeyUp += new KeyEventHandler(panel1_KeyUp);

			myMap.GenerateAllLODs();
		}

		void LoadData()
		{
			KeyManager.Initialize(SaveManager.Data);
			myNodeCollection.InitializeNodes(SaveManager.Data);

			carriedNode = null;
			selectedNode = null;

			imageBasePos = new Vector2(0, 0);
			mapPickedUpPos = new Vector2(0, 0);
			carriedMap = false;

			myMousePos = new Vector2(0, 0);

			selectedOffset = null;
			mouseDownTimeStamp = DateTime.MinValue;

			baseZoomScale = 0.1f;

			comboBoxEvent.DataSource = KeyManager.GetEventKeys().ToList();
			comboBoxEvent.DisplayMember = "Name";
			comboBoxEvent.Enabled = false;
			comboBoxEvent.Visible = false;

			txtRandomId.Enabled = false;
			txtRandomId.Visible = false;

			lockPanelLogic1.Enabled = false;
			lockPanelLogic1.Visible = false;
		}

		private void UpdateNodeSettings()
		{
			if (selectedNode != null)
			{
				if (selectedNode is LockNode lockNode)
				{
					lockPanelLogic1.SetNode(lockNode.myRequirement);
				}

				lockPanelLogic1.Visible = (selectedNode.myNodeType == NodeType.Lock);
				lockPanelLogic1.Enabled = (selectedNode.myNodeType == NodeType.Lock);

				if (selectedNode is EventKeyNode eventNode)
					comboBoxEvent.SelectedItem = eventNode.GetKey();

				comboBoxEvent.Enabled = (selectedNode.myNodeType == NodeType.EventKey);
				comboBoxEvent.Visible = (selectedNode.myNodeType == NodeType.EventKey);

				if (selectedNode is RandomKeyNode randomNode)
					txtRandomId.Text = randomNode.myRandomKeyIdentifier;

				txtRandomId.Enabled = (selectedNode.myNodeType == NodeType.RandomKey);
				txtRandomId.Visible = (selectedNode.myNodeType == NodeType.RandomKey);
			}
			else
			{
				comboBoxEvent.Visible = false;
				comboBoxEvent.Enabled = false;

				txtRandomId.Enabled = false;
				txtRandomId.Visible = false;

				lockPanelLogic1.Visible = false;
				lockPanelLogic1.Enabled = false;
			}
		}

		private void DeleteNode(NodeBase nodeToDelete)
		{
			if (nodeToDelete == null)
				return;

			myMementos.Add(myNodeCollection.RemoveNode(nodeToDelete));

			UpdateNodeDeleted();

			panel1.Invalidate();
		}

		private void UpdateNodeDeleted()
		{
			if (!myNodeCollection.myNodes.Contains(selectedNode))
			{
				selectedNode = null;
				carriedNode = null;

				UpdateNodeSettings();
			}
		}

		private Vector2 TranslateVector(Vector2 v)
		{
			return new Vector2(TranslateX(v.x), TranslateY(v.y));
		}

		private int TranslateX(float x)
		{
			return (int)((imageBasePos.x + x) * ZoomScale);
		}

		private int TranslateY(float y)
		{
			return (int)((imageBasePos.y + y) * ZoomScale);
		}

		private void DrawDebugMessage(string message, Graphics graphicsObj)
		{
			Font drawFont = new Font("Arial", 16);
			SolidBrush drawBrush = new SolidBrush(Color.White);
			graphicsObj.DrawString(message, drawFont, drawBrush, new Point(50, 50));
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			var graphicsObj = e.Graphics;

			var panelRect = new Rectangle(new Point(0, 0), panel1.Size);
			myMap.Draw(imageBasePos, ZoomScale, graphicsObj, panelRect);

			//DrawDebugMessage($"{myPointerState}", graphicsObj);

			myNodeRenderer.BasePos = imageBasePos;
			myNodeRenderer.Zoom = ZoomScale;
			myNodeRenderer.carriedNodeId = carriedNode?.id ?? Guid.Empty;
			myNodeRenderer.selectedNodeId = selectedNode?.id ?? Guid.Empty;
			myNodeRenderer.panelSize = panel1.Size;

			myNodeRenderer.RenderNodes(myNodeCollection.myNodes, graphicsObj);

			//Draw cursor
			if (myPointerState == PointerState.PlaceBlank)
			{
				myNodeRenderer.DrawCursorNode(myMousePos, NodeType.Blank, graphicsObj);
			}
			else if (myPointerState == PointerState.PlaceLock)
			{
				myNodeRenderer.DrawCursorNode(myMousePos, NodeType.Lock, graphicsObj);
			}
			else if (myPointerState == PointerState.PlaceRandom)
			{
				myNodeRenderer.DrawCursorNode(myMousePos, NodeType.RandomKey, graphicsObj);
			}
			else if (myPointerState == PointerState.PlaceEvent)
			{
				myNodeRenderer.DrawCursorNode(myMousePos, NodeType.EventKey, graphicsObj);
			}
			else if (myPointerState == PointerState.OneWay && selectedNode != null)
			{
				myNodeRenderer.DrawCursorOneWayConnection(selectedNode, myMousePos, graphicsObj);
			}
			else if (myPointerState == PointerState.TwoWay && selectedNode != null)
			{
				myNodeRenderer.DrawCursorTwoWayConnection(selectedNode, myMousePos, graphicsObj);
			}
		}

		private void UpdatePointerState()
		{
			if (chkNewBlankNode.Checked)
			{
				myPointerState = PointerState.PlaceBlank;
			}
			else if (chkNewLockNode.Checked)
			{
				myPointerState = PointerState.PlaceLock;
			}
			else if (chkNewRandomNode.Checked)
			{
				myPointerState = PointerState.PlaceRandom;
			}
			else if (chkNewEventNode.Checked)
			{
				myPointerState = PointerState.PlaceEvent;
			}
			else if (chkTwoWayConnection.Checked)
			{
				myPointerState = PointerState.TwoWay;
			}
			else if (chkOneWayConnection.Checked)
			{
				myPointerState = PointerState.OneWay;
			}
			else if (ModifierKeys == Keys.Shift)
			{
				myPointerState = PointerState.TwoWay;
			}
			else if (ModifierKeys == Keys.Alt)
			{
				myPointerState = PointerState.OneWay;
			}
			else
			{
				myPointerState = PointerState.None;
			}
		}

		private NodeBase FindClickedNode(Vector2 someMousePos)
		{
			var width = NodeRenderer.nodeSize;
			var height = NodeRenderer.nodeSize;

			foreach (var node in myNodeCollection.myNodes)
			{
				var screenSpaceNodePos = TranslateVector(node.myPos);

				if (someMousePos.x > (screenSpaceNodePos.x - width / 2) && someMousePos.y > (screenSpaceNodePos.y - height / 2) &&
					someMousePos.x < (screenSpaceNodePos.x + width / 2) && someMousePos.y < (screenSpaceNodePos.y + height / 2))
				{
					return node;
				}
			}

			return null;
		}

		private void UpdateConnections(Vector2 mousePos, NodeBase newNode)
		{
			if (newNode == null)
				return;

			if (myPointerState == PointerState.TwoWay && selectedNode != null && selectedNode != newNode)
			{
				myMementos.Add(myNodeCollection.CreateConnectionMemento(new List<NodeBase> { selectedNode, newNode }));
				if (selectedNode.myConnections.Contains(newNode) && newNode.myConnections.Contains(selectedNode))
				{
					selectedNode.RemoveConnection(newNode);
					newNode.RemoveConnection(selectedNode);
				}
				else
				{
					if (!selectedNode.myConnections.Contains(newNode))
					{
						selectedNode.CreateConnection(newNode);
					}
					if (!newNode.myConnections.Contains(selectedNode))
					{
						newNode.CreateConnection(selectedNode);
					}
				}

				UncheckStateBoxes();
			}
			else if (myPointerState == PointerState.OneWay && selectedNode != null && selectedNode != newNode)
			{
				myMementos.Add(myNodeCollection.CreateConnectionMemento(selectedNode));
				if (!selectedNode.myConnections.Contains(newNode))
				{
					selectedNode.CreateConnection(newNode);
				}
				else
				{
					selectedNode.RemoveConnection(newNode);
				}

				UncheckStateBoxes();
			}
			else
			{
				PickUpNode(newNode, (mousePos - TranslateVector(newNode.myPos)) / ZoomScale);
			}
		}

		private NodeBase PlaceNewNode(NodeType type, Vector2 mousePos)
		{
			NodeBase newNode = null;
			switch (type)
			{
				case NodeType.Blank:
					newNode = new BlankNode();
					break;
				case NodeType.Lock:
					newNode = new LockNode();
					break;
				case NodeType.RandomKey:
					newNode = new RandomKeyNode();
					break;
				case NodeType.EventKey:
					newNode = new EventKeyNode();
					break;
				default:
					return null;
			}

			newNode.myPos = (mousePos / ZoomScale) - imageBasePos;

			myMementos.Add(myNodeCollection.AddNode(newNode));

			selectedNode = newNode;

			UpdateNodeSettings();

			panel1.Invalidate();

			return newNode;
		}

		private void PickUpNode(NodeBase nodeToCarry, Vector2 offset)
		{
			carriedNode = nodeToCarry;
			myNodeRenderer.CarriedPos = carriedNode.myPos;

			selectedOffset = offset;
			selectedNode = nodeToCarry;

			UpdateNodeSettings();
		}

		private void DropCarriedNode()
		{
			if (carriedNode != null)
			{
				if(DateTime.UtcNow - mouseDownTimeStamp > clickTreshold)
				{
					myMementos.Add(carriedNode.CreateMemento());
					carriedNode.myPos = myNodeRenderer.CarriedPos;
				}
				
				carriedNode = null;
			}
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			var mousePos = new Vector2(e.Location);

			if (e.Button != MouseButtons.Left)
				return;

			NodeBase newNode = null;

			switch (myPointerState)
			{
				case PointerState.PlaceBlank:
					newNode = PlaceNewNode(NodeType.Blank, mousePos);
					break;
				case PointerState.PlaceLock:
					newNode = PlaceNewNode(NodeType.Lock, mousePos);
					break;
				case PointerState.PlaceRandom:
					newNode = PlaceNewNode(NodeType.RandomKey, mousePos);
					break;
				case PointerState.PlaceEvent:
					newNode = PlaceNewNode(NodeType.EventKey, mousePos);
					break;
			}

			if(newNode != null)
			{
				UncheckStateBoxes();

				PickUpNode(newNode, new Vector2(0, 0));
			}
			else
			{
				newNode = FindClickedNode(mousePos);

				UpdateConnections(mousePos, newNode);
			}

			// No node was hit, activate map dragging
			if (newNode == null)
			{
				carriedMap = true;

				mapPickedUpPos = imageBasePos.Clone();
				selectedOffset = (mousePos / ZoomScale) - imageBasePos;
			}

			panel1.Invalidate();

			mouseDownTimeStamp = DateTime.UtcNow;
		}

		private void panel1_MouseUp(object sender, MouseEventArgs e)
		{
			var mousePos = new Vector2(e.Location);
			if (e.Button == MouseButtons.Left)
			{
				if (carriedMap)
				{
					//If picked up map but didn't move it, consider it as just clicking the map
					if ((mapPickedUpPos - imageBasePos).Magnitude() < 1)
					{
						selectedNode = null;
						UpdateNodeSettings();
					}
				}

				DropCarriedNode();

				carriedMap = false;

				panel1.Invalidate();
			}
			else if(e.Button == MouseButtons.Right && myPointerState == PointerState.None)
			{
				NodeBase clickedNode = FindClickedNode(mousePos);

				contextMenuStrip.Tag = mousePos;

				contextMenuStrip.Items.Clear();
				if (clickedNode != null && selectedNode != null && selectedNode != clickedNode)
				{
					contextMenuStrip.Tag = new Tuple<NodeBase, NodeBase>(selectedNode, clickedNode);

					var connectionThere = selectedNode.myConnections.Contains(clickedNode);
					var connectionFrom = clickedNode.myConnections.Contains(selectedNode);

					if(connectionThere != connectionFrom)
					{
						var item = new ToolStripMenuItem("Complete Two-Way Connection");
						item.Click += completeConnectionContextMenuItem_Click;
						item.Tag = new Tuple<NodeBase, NodeBase>(selectedNode, clickedNode);
						contextMenuStrip.Items.Add(item);
					}

					if (!connectionThere && !connectionFrom)
					{
						var item = new ToolStripMenuItem("Create New One-Way Connection");
						item.Click += oneWayConnectionContextMenuItem_Click;
						item.Tag = new Tuple<NodeBase, NodeBase>(selectedNode, clickedNode);
						contextMenuStrip.Items.Add(item);

						var item2 = new ToolStripMenuItem("Create New Two-Way Connection");
						item2.Click += completeConnectionContextMenuItem_Click;
						item.Tag = new Tuple<NodeBase, NodeBase>(selectedNode, clickedNode);
						contextMenuStrip.Items.Add(item2);
					}

					if (connectionThere || connectionFrom)
					{
						var item = new ToolStripMenuItem("Remove Connection");
						item.Click += removeConnectionContextMenuItem_Click;
						item.Tag = new Tuple<NodeBase, NodeBase>(selectedNode, clickedNode);
						contextMenuStrip.Items.Add(item);
					}
				}
				
				var blankItem = new ToolStripMenuItem("Create New Blank Node");
				blankItem.Click += placeNewContextMenuItem_Click;
				blankItem.Tag = NodeType.Blank;
				contextMenuStrip.Items.Add(blankItem);

				var lockItem = new ToolStripMenuItem("Create New Lock Node");
				lockItem.Click += placeNewContextMenuItem_Click;
				lockItem.Tag = NodeType.Lock;
				contextMenuStrip.Items.Add(lockItem);

				var randomItem = new ToolStripMenuItem("Create New Randomized Key Node");
				randomItem.Click += placeNewContextMenuItem_Click;
				randomItem.Tag = NodeType.RandomKey;
				contextMenuStrip.Items.Add(randomItem);

				var eventItem = new ToolStripMenuItem("Create New Event Key Node");
				eventItem.Click += placeNewContextMenuItem_Click;
				eventItem.Tag = NodeType.EventKey;
				contextMenuStrip.Items.Add(eventItem);
				
				if(clickedNode != null)
				{
					var deleteItem = new ToolStripMenuItem("Delete Node");
					deleteItem.Click += deleteContextMenuItem_Click;
					deleteItem.Tag = clickedNode;
					contextMenuStrip.Items.Add(deleteItem);
				}

				contextMenuStrip.Show(panel1.PointToScreen(mousePos.ToPoint()));
			}
		}

		private void panel1_MouseMove(object sender, MouseEventArgs e)
		{
			myMousePos = new Vector2(e.X, e.Y);

			if (carriedNode != null)
			{
				myNodeRenderer.CarriedPos = (myMousePos / ZoomScale) - (selectedOffset + imageBasePos);
			}

			if(carriedMap)
			{
				imageBasePos = (myMousePos / ZoomScale) - selectedOffset;
			}

			UpdatePointerState();

			panel1.Invalidate();
		}

		private void panel1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (carriedNode == null && carriedMap == false)
			{
				var mousePos = new Vector2(e.X, e.Y);

				var oldZoomScale = ZoomScale;
				baseZoomScale = (float)Math.Exp(Math.Log(baseZoomScale) + ((float)(e.Delta) / 1000));
				var newZoomScale = ZoomScale;

				imageBasePos = imageBasePos + ((mousePos / newZoomScale) - (mousePos / oldZoomScale));
				panel1.Invalidate();
			}
		}

		private void panel1_MouseHover(object sender, EventArgs e)
		{
			panel1.Focus();

			UpdatePointerState();

			panel1.Invalidate();
		}

		private void comboBoxEvent_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (selectedNode is EventKeyNode eventNode)
			{
				eventNode.SetKey((BaseKey)comboBoxEvent.SelectedItem);
			}
		}
		
		private void txtRandomId_TextChanged(object sender, EventArgs e)
		{
			if (selectedNode is RandomKeyNode randomNode)
			{
				randomNode.myRandomKeyIdentifier = txtRandomId.Text;
			}
		}

		private void panel1_SizeChanged(object sender, EventArgs e)
		{
			panel1.Invalidate();
		}

		private void panel1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				UncheckStateBoxes();
			}

			if (e.KeyCode == Keys.Delete)
			{
				DeleteNode(selectedNode);
			}

			if(ModifierKeys == Keys.Control && e.KeyCode == Keys.Z)
			{
				if (myMementos.Any())
				{
					var memento = myMementos.Last();
					myMementos.Remove(memento);
					myNodeCollection.RestoreMemento(memento);

					UpdateNodeDeleted();
				}
			}

			UpdatePointerState();

			panel1.Invalidate();
		}

		private void panel1_KeyUp(object sender, KeyEventArgs e)
		{
			UpdatePointerState();

			panel1.Invalidate();
		}

		private void customKeysToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new KeyEditor().ShowDialog();

			if(lockPanelLogic1.Visible)
				lockPanelLogic1.RefreshNode();
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new SettingsForm().ShowDialog();

			Refresh();
		}		

		private void UncheckAllExcept(CheckBox chkBox)
		{
			if(chkBox != chkNewBlankNode)
				chkNewBlankNode.Checked = false;
			if (chkBox != chkNewLockNode)
				chkNewLockNode.Checked = false;
			if (chkBox != chkNewRandomNode)
				chkNewRandomNode.Checked = false;
			if (chkBox != chkNewEventNode)
				chkNewEventNode.Checked = false;
			if (chkBox != chkTwoWayConnection)
				chkTwoWayConnection.Checked = false;
			if (chkBox != chkOneWayConnection)
				chkOneWayConnection.Checked = false;
		}

		private void UncheckStateBoxes()
		{
			UncheckAllExcept(null);
		}

		private void CheckBoxChecked(object sender, EventArgs e)
		{
			UncheckAllExcept(sender as CheckBox);

			UpdatePointerState();
			panel1.Invalidate();
		}

		private void completeConnectionContextMenuItem_Click(object sender, EventArgs e)
		{			
			var nodes = (Tuple<NodeBase, NodeBase>)contextMenuStrip.Tag;

			if (!nodes.Item1.myConnections.Contains(nodes.Item2) || !nodes.Item2.myConnections.Contains(nodes.Item1))
			{
				myMementos.Add(myNodeCollection.CreateConnectionMemento(new List<NodeBase> { nodes.Item1, nodes.Item2 }));

				nodes.Item1.CreateConnection(nodes.Item2);
				nodes.Item2.CreateConnection(nodes.Item1);
			}
		}

		private void oneWayConnectionContextMenuItem_Click(object sender, EventArgs e)
		{
			var nodes = (Tuple<NodeBase, NodeBase>)contextMenuStrip.Tag;

			if (!nodes.Item1.myConnections.Contains(nodes.Item2))
			{
				myMementos.Add(myNodeCollection.CreateConnectionMemento(nodes.Item1));

				nodes.Item1.CreateConnection(nodes.Item2);
			}
		}

		private void removeConnectionContextMenuItem_Click(object sender, EventArgs e)
		{
			var nodes = (Tuple<NodeBase, NodeBase>)contextMenuStrip.Tag;

			if (nodes.Item1.myConnections.Contains(nodes.Item2) ||
				nodes.Item2.myConnections.Contains(nodes.Item1))
			{
				myMementos.Add(myNodeCollection.CreateConnectionMemento(new List<NodeBase> { nodes.Item1, nodes.Item2 }));

				nodes.Item1.RemoveConnection(nodes.Item2);
				nodes.Item2.RemoveConnection(nodes.Item1);
			}
		}

		private void placeNewContextMenuItem_Click(object sender, EventArgs e)
		{
			var type = (NodeType)((ToolStripMenuItem)sender).Tag;
			var pos = (Vector2)contextMenuStrip.Tag;

			PlaceNewNode(type, pos);
		}

		private void deleteContextMenuItem_Click(object sender, EventArgs e)
		{
			var nodeToDelete = (NodeBase)((ToolStripMenuItem)sender).Tag;

			DeleteNode(nodeToDelete);
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveManager.New();

			KeyManager.Initialize(SaveManager.Data);
			myNodeCollection.InitializeNodes(SaveManager.Data);

			myMementos.Clear();
			lockPanelLogic1.ClearMementos();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();

			dialog.Filter = "logic files (*.lgc)|*.lgc";

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				if (SaveManager.Open(dialog.FileName))
				{
					LoadData();

					myMementos.Clear();
					lockPanelLogic1.ClearMementos();
				}
				else
				{
					MessageBox.Show($"Opening {dialog.FileName} failed", "Load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Save();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs();
		}

		private void Save()
		{
			if (!SaveManager.Save((string)Properties.Settings.Default["LatestFilePath"]))
				SaveAs();
		}

		private void SaveAs()
		{
			var dialog = new SaveFileDialog();

			dialog.Filter = "logic files (*.lgc)|*.lgc";

			if(dialog.ShowDialog() == DialogResult.OK)
			{
				if(!SaveManager.Save(dialog.FileName))
					MessageBox.Show($"Saving to {dialog.FileName} failed", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
