﻿using System;
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
using System.IO;
using System.Threading.Tasks;

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
        private NodeSearcher myNodeSearcher = null;
        private List<NodeMemento> myMementos = new List<NodeMemento>();
		private NodeBase carriedNode = null;
		private NodeBase selectedNode = null;
		private Connection selectedConnection = null;

		private Vector2 imageBasePos = new Vector2(0, 0);
		private Vector2 mapPickedUpPos = new Vector2(0, 0);
		private bool carriedMap = false;

		private Vector2 myMousePos = new Vector2(0, 0);
		private Vector2 myNodeCarriedPos = new Vector2(0, 0);

		private Vector2 selectedOffset = null;
		private DateTime mouseDownTimeStamp = DateTime.MinValue;
		private static TimeSpan clickTreshold = TimeSpan.FromMilliseconds(150);

		private float baseZoomScale = 0.1f;
		private float ZoomScale { get { return baseZoomScale * (Utility.CalcDiag(panel1.Width, panel1.Height) / 1000f); } set { baseZoomScale = value; } }

        private bool traveling = false;
        private Vector2 travelOrigPos = new Vector2(0, 0);
        private Vector2 travelTargetPos = new Vector2(0, 0);
        private float travelOrigZoom = 0f;
        private float travelTargetZoom = 0f;
        private DateTime travelStartTime = DateTime.MinValue;
        private TimeSpan travelTime = TimeSpan.MinValue;

        private PointerState myPointerState = PointerState.None;

		public Form1()
		{
			if (!SaveManager.Open((string)Properties.Settings.Default["LatestFilePath"]))
				SaveManager.New();

            myNodeSearcher = new NodeSearcher(myNodeCollection);
            myNodeSearcher.SearchComplete += NodeSearchComplete;

            InitializeComponent();

			LoadData();

			(panel1 as Control).KeyDown += new KeyEventHandler(panel1_KeyDown);
			(panel1 as Control).KeyUp += new KeyEventHandler(panel1_KeyUp);

			SaveManager.DirtyChanged += new EventHandler(saveManager_DirtyChanged);

			myMap.GenerateAllLODs();
		}

		void LoadData()
		{
			KeyManager.Initialize(SaveManager.Data);
			myNodeCollection.InitializeNodes(SaveManager.Data);

			myMementos.Clear();
			lockPanelLogic1.ClearMementos();

			UpdateFileDirty();

			lockPanelLogic1.SetKeys();

			carriedNode = null;
			selectedNode = null;
			selectedConnection = null;

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

            comboBoxOrigItem.DataSource = KeyManager.GetRandomKeys().ToList();
            comboBoxOrigItem.DisplayMember = "Name";
            comboBoxOrigItem.Enabled = false;
            comboBoxOrigItem.Visible = false;

            lockPanelLogic1.Enabled = false;
			lockPanelLogic1.Visible = false;
		}

		private void UpdateFileDirty()
		{
			this.Text = (SaveManager.Dirty ? "*" : string.Empty) + SaveManager.CurrentFile() + " - Logic Editor";
		}

		private void saveManager_DirtyChanged(object sender, EventArgs e)
		{
			UpdateFileDirty();
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
                {
                    txtRandomId.Text = randomNode.myRandomKeyIdentifier;
                    comboBoxOrigItem.SelectedItem = randomNode.GetOriginalKey();
                }

				txtRandomId.Enabled = (selectedNode.myNodeType == NodeType.RandomKey);
				txtRandomId.Visible = (selectedNode.myNodeType == NodeType.RandomKey);
                comboBoxOrigItem.Enabled = (selectedNode.myNodeType == NodeType.RandomKey);
                comboBoxOrigItem.Visible = (selectedNode.myNodeType == NodeType.RandomKey);
            }
			else
			{
				comboBoxEvent.Visible = false;
				comboBoxEvent.Enabled = false;

				txtRandomId.Enabled = false;
				txtRandomId.Visible = false;
                comboBoxOrigItem.Enabled = false;
                comboBoxOrigItem.Visible = false;

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

			SaveManager.Dirty = true;

			panel1.Invalidate();
		}

		private void UpdateNodeDeleted()
		{
			if (!myNodeCollection.myNodes.Contains(selectedNode))
			{
				selectedNode = null;
				selectedConnection = null;
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

			var highlightedNode = FindClickedNode(myMousePos);
			var highlightedConnection = highlightedNode == null ? FindClickedConnection(myMousePos) : null;

			var renderOptions = new NodeRenderOptions();
			renderOptions.BasePos = imageBasePos;
			renderOptions.Zoom = ZoomScale;
			renderOptions.carriedPos = myNodeCarriedPos;
			renderOptions.carriedNodeId = carriedNode?.id ?? Guid.Empty;
			renderOptions.selectedNodeId = selectedNode?.id ?? Guid.Empty;
			renderOptions.highlightedNodeId = highlightedNode?.id ?? Guid.Empty;
			renderOptions.selectedConnection = selectedConnection;
			renderOptions.highlightedConnection = highlightedConnection;
			renderOptions.panelSize = panel1.Size;
			
			myNodeRenderer.RenderNodes(myNodeCollection.myNodes, graphicsObj, renderOptions);

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

			// var mopusepos = (myMousePos / ZoomScale) - imageBasePos;
			// DrawDebugMessage($"{mopusepos.x}, {mopusepos.y}", graphicsObj);
		}

        private void Travel()
        {
            try
            {
                traveling = true;
                while (DateTime.Now < travelStartTime + travelTime)
                {
                    var currentTime = DateTime.Now - travelStartTime;
                    var timeFraction = (float)(currentTime.TotalMilliseconds / travelTime.TotalMilliseconds);

                    baseZoomScale = travelOrigZoom + (travelTargetZoom - travelOrigZoom) * timeFraction;
                    var targetPos = travelOrigPos + (travelTargetPos - travelOrigPos) * timeFraction;

                    imageBasePos = new Vector2(panel1.Size.Width / 2, panel1.Size.Height / 2) / ZoomScale - targetPos;
                    panel1.Invalidate();
                }
            }
            finally
            {
                traveling = false;
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

		private Connection FindClickedConnection(Vector2 someMousePos)
		{
			foreach (var node in myNodeCollection.myNodes)
			{
				var screenSpaceNodePos = TranslateVector(node.myPos);

				foreach (var conn in node.myConnections)
				{
					var screenSpaceConnectionPos = TranslateVector(conn.myPos);

					if (Utility.FindDistanceToSegment(someMousePos.ToPoint(), screenSpaceNodePos.ToPoint(), screenSpaceConnectionPos.ToPoint()) < 7)
					{
						return new Connection(node, conn);
					}
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

				SaveManager.Dirty = true;

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

				SaveManager.Dirty = true;

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

			SaveManager.Dirty = true;

			selectedNode = newNode;

			UpdateNodeSettings();

			panel1.Invalidate();

			return newNode;
		}

		private void PickUpNode(NodeBase nodeToCarry, Vector2 offset)
		{
			carriedNode = nodeToCarry;
			myNodeCarriedPos = carriedNode.myPos;

			selectedOffset = offset;
			selectedNode = nodeToCarry;
			selectedConnection = null;

			UpdateNodeSettings();
		}

		private void DropCarriedNode()
		{
			if (carriedNode != null)
			{
				if(DateTime.UtcNow - mouseDownTimeStamp > clickTreshold)
				{
					myMementos.Add(carriedNode.CreateMemento());

					SaveManager.Dirty = true;

					carriedNode.myPos = myNodeCarriedPos;
				}
				
				carriedNode = null;
			}
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			var mousePos = new Vector2(e.Location);

			if (e.Button != MouseButtons.Left)
				return;

            if (traveling)
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

						var clickedConnection = FindClickedConnection(mousePos);

						if (clickedConnection != null)
						{
							selectedConnection = clickedConnection;
						}
						else
						{
							selectedConnection = null;
						}

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
				Connection clickedConnection = FindClickedConnection(mousePos);

				contextMenuStrip.Tag = mousePos;

				contextMenuStrip.Items.Clear();
				if (clickedNode != null && selectedNode != null && selectedNode != clickedNode)
				{
					var connectionThere = selectedNode.myConnections.Contains(clickedNode);
					var connectionFrom = clickedNode.myConnections.Contains(selectedNode);

					if(connectionThere != connectionFrom)
					{
						var item = new ToolStripMenuItem("Complete Two-Way Connection");
						item.Click += completeConnectionContextMenuItem_Click;
						item.Tag = new Connection(selectedNode, clickedNode);
						contextMenuStrip.Items.Add(item);
					}

					if (!connectionThere && !connectionFrom)
					{
						var item = new ToolStripMenuItem("Create New One-Way Connection");
						item.Click += oneWayConnectionContextMenuItem_Click;
						item.Tag = new Connection(selectedNode, clickedNode);
						contextMenuStrip.Items.Add(item);

						var item2 = new ToolStripMenuItem("Create New Two-Way Connection");
						item2.Click += completeConnectionContextMenuItem_Click;
						item.Tag = new Connection(selectedNode, clickedNode);
						contextMenuStrip.Items.Add(item2);
					}

					if (connectionThere || connectionFrom)
					{
						var item = new ToolStripMenuItem("Remove Connection");
						item.Click += removeConnectionContextMenuItem_Click;
						item.Tag = new Connection(selectedNode, clickedNode);
						contextMenuStrip.Items.Add(item);
					}
				}
				else if(clickedConnection != null)
				{
					var connectionThere = clickedConnection.node1.myConnections.Contains(clickedConnection.node2);
					var connectionFrom = clickedConnection.node2.myConnections.Contains(clickedConnection.node1);

					if (connectionThere != connectionFrom)
					{
						var item = new ToolStripMenuItem("Complete Two-Way Connection");
						item.Click += completeConnectionContextMenuItem_Click;
						item.Tag = clickedConnection;
						contextMenuStrip.Items.Add(item);
					}

					if (connectionThere || connectionFrom)
					{
						var item = new ToolStripMenuItem("Remove Connection");
						item.Click += removeConnectionContextMenuItem_Click;
						item.Tag = clickedConnection;
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
				myNodeCarriedPos = (myMousePos / ZoomScale) - (selectedOffset + imageBasePos);
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

				SaveManager.Dirty = true;
			}
		}
		
		private void txtRandomId_TextChanged(object sender, EventArgs e)
		{
			if (selectedNode is RandomKeyNode randomNode)
			{
				randomNode.myRandomKeyIdentifier = txtRandomId.Text;

				SaveManager.Dirty = true;
			}
		}

        private void comboBoxOrigItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedNode is RandomKeyNode randomNode)
            {
                randomNode.SetOriginalKey((BaseKey)comboBoxOrigItem.SelectedItem);

                SaveManager.Dirty = true;
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
				DeleteConnection(selectedConnection);
			}

			if (ModifierKeys == Keys.Control && e.KeyCode == Keys.Z)
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
			var keyEditor = new KeyEditor();
			keyEditor.StartPosition = FormStartPosition.CenterParent;
			keyEditor.ShowDialog();

			if(lockPanelLogic1.Visible)
				lockPanelLogic1.RefreshNode();

			comboBoxEvent.DataSource = KeyManager.GetEventKeys().ToList();
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
			var menuItem = (ToolStripMenuItem)sender;
			var connection = (Connection)menuItem.Tag;

			if (!connection.node1.myConnections.Contains(connection.node2) || !connection.node2.myConnections.Contains(connection.node1))
			{
				myMementos.Add(myNodeCollection.CreateConnectionMemento(new List<NodeBase> { connection.node1, connection.node2 }));

				SaveManager.Dirty = true;

				connection.node1.CreateConnection(connection.node2);
				connection.node2.CreateConnection(connection.node1);
			}
		}

		private void oneWayConnectionContextMenuItem_Click(object sender, EventArgs e)
		{
			var menuItem = (ToolStripMenuItem)sender;
			var connection = (Connection)menuItem.Tag;

			if (!connection.node1.myConnections.Contains(connection.node2))
			{
				myMementos.Add(myNodeCollection.CreateConnectionMemento(connection.node1));

				SaveManager.Dirty = true;

				connection.node1.CreateConnection(connection.node2);
			}
		}

		private void removeConnectionContextMenuItem_Click(object sender, EventArgs e)
		{
			var menuItem = (ToolStripMenuItem)sender;
			var connection = (Connection)menuItem.Tag;

			DeleteConnection(connection);
		}

		private void DeleteConnection(Connection connection)
		{
			if (connection == null)
				return;

			if (connection.node1.myConnections.Contains(connection.node2) ||
				connection.node2.myConnections.Contains(connection.node1))
			{
				myMementos.Add(myNodeCollection.CreateConnectionMemento(new List<NodeBase> { connection.node1, connection.node2 }));

				SaveManager.Dirty = true;

				connection.node1.RemoveConnection(connection.node2);
				connection.node2.RemoveConnection(connection.node1);
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

			LoadData();	
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

		private bool Save()
		{
			if (!SaveManager.Save())
				return SaveAs();

			return true;
		}

		private bool SaveAs()
		{
			var dialog = new SaveFileDialog();

			dialog.Filter = "logic files (*.lgc)|*.lgc";

			if(dialog.ShowDialog() == DialogResult.OK)
			{
				if (SaveManager.Save(dialog.FileName))
				{
					return true;
				}
				else
				{
					MessageBox.Show($"Saving to {dialog.FileName} failed", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			return false;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (SaveManager.Dirty)
			{
				DialogResult dialogResult = MessageBox.Show($"Do you want to save changes to {SaveManager.CurrentFile()}", "Changes not saved", MessageBoxButtons.YesNoCancel);
				if (dialogResult == DialogResult.Yes)
				{
					if(!Save())
					{
						e.Cancel = true;
					}
				}
				else if (dialogResult == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}

        private void txtNodeSearch_TextChanged(object sender, EventArgs e)
        {
            myNodeSearcher.Search(txtNodeSearch.Text);
        }

        private delegate void NodeSearchCompleteDelegate();
        private void NodeSearchComplete()
        {
            if (lstNodeSearchResult.InvokeRequired)
            {
                Invoke(new NodeSearchCompleteDelegate(NodeSearchComplete));
                return;
            }

            lstNodeSearchResult.Items.Clear();

            foreach (var item in myNodeSearcher.SearchResult)
            {
                if (item is BlankNode)
                {
                    lstNodeSearchResult.Items.Add(new NodeSearchBoxItem($"Blank {item.myPos.x} : {item.myPos.y}", item));
                }

                if (item is LockNode)
                {
                    lstNodeSearchResult.Items.Add(new NodeSearchBoxItem($"Lock {item.myPos.x} : {item.myPos.y}", item));
                }

                if (item is KeyNode key)
                {
                    lstNodeSearchResult.Items.Add(new NodeSearchBoxItem(key.Name(), key));
                }
            }
        }

        private void lstNodeSearchResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstNodeSearchResult.SelectedItem != null)
            {
                var selectedItem = (NodeSearchBoxItem)lstNodeSearchResult.SelectedItem;
                var targetNode = selectedItem?.Node;
                if (targetNode != null)
                {
                    selectedNode = targetNode;
                    UpdateNodeSettings();

                    travelOrigPos = new Vector2(panel1.Size.Width / 2, panel1.Size.Height / 2) / ZoomScale - imageBasePos;
                    travelOrigZoom = baseZoomScale;
                    travelStartTime = DateTime.Now;
                    travelTargetPos = targetNode.myPos;
                    travelTargetZoom = 0.3f;
                    travelTime = TimeSpan.FromMilliseconds(400);

                    Task.Run(() => Travel());
                }
            }
        }
    }
}
