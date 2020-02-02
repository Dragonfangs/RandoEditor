using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Utils;
using RandoEditor.Map;
using Common.Node;
using Newtonsoft.Json;
using Common.Key;
using RandoEditor.SaveData;
using RandoEditor.Node;

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
        private List<NodeBase> myNodes = new List<NodeBase>();
        private NodeBase carriedNode = null;
		private NodeBase selectedNode = null;
		
		private Vector2 imageBasePos = new Vector2(0,0);
		private Vector2 mapPickedUpPos = new Vector2(0,0);
		private bool carriedMap = false;

		private Vector2 mousePos = new Vector2(0, 0);

		private Vector2 selectedOffset = null;
				
		private float baseZoomScale = 0.1f;
		private float ZoomScale { get { return baseZoomScale * (Utility.CalcDiag(panel1.Width, panel1.Height) / 1000f); } set { baseZoomScale = value; } }

		private PointerState myPointerState = PointerState.None;

        public Form1()
        {
            InitializeComponent();

			(panel1 as Control).KeyDown += new KeyEventHandler(panel1_KeyDown);
			(panel1 as Control).KeyUp += new KeyEventHandler(panel1_KeyUp);

			comboBoxEvent.DataSource = KeyManager.GetEventKeys().ToList();
			comboBoxEvent.DisplayMember = "Name";
			comboBoxEvent.Enabled = false;
			comboBoxEvent.Visible = false;

			txtRandomId.Enabled = false;
			txtRandomId.Visible = false;

			lockPanelLogic1.Enabled = false;
			lockPanelLogic1.Visible = false;

			myMap.GenerateAllLODs();

			InitializeNodes();
		}

		private void SaveNodes()
		{
			var text = JsonConvert.SerializeObject(myNodes);
			System.IO.File.WriteAllText("nodes.json", JsonConvert.SerializeObject(myNodes));
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

				if(selectedNode is EventKeyNode eventNode)
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

		private Vector2 TranslateVector(Vector2 v)
		{
			return new Vector2(TranslateX(v.x), TranslateY(v.y));
		}

		private int TranslateX(float x)
        {
            return (int) ((imageBasePos.x + x) * ZoomScale);
        }

        private int TranslateY(float y)
        {
            return (int) ((imageBasePos.y + y) * ZoomScale);
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

			var panelRect = new Rectangle(new Point(0,0), panel1.Size);
			myMap.Draw(imageBasePos, ZoomScale, graphicsObj, panelRect);

			//DrawDebugMessage($"{myPointerState}", graphicsObj);

			myNodeRenderer.BasePos = imageBasePos;
			myNodeRenderer.Zoom = ZoomScale;
			myNodeRenderer.carriedNodeId = carriedNode?.id ?? Guid.Empty;
			myNodeRenderer.selectedNodeId = selectedNode?.id ?? Guid.Empty;
			myNodeRenderer.panelSize = panel1.Size;

			myNodeRenderer.RenderNodes(myNodes, graphicsObj);

			//Draw cursor
			if (myPointerState == PointerState.PlaceBlank)
			{
				myNodeRenderer.DrawCursorNode(mousePos, NodeType.Blank, graphicsObj);
			}
			else if (myPointerState == PointerState.PlaceLock)
			{
				myNodeRenderer.DrawCursorNode(mousePos, NodeType.Lock, graphicsObj);
			}
			else if (myPointerState == PointerState.PlaceRandom)
			{
				myNodeRenderer.DrawCursorNode(mousePos, NodeType.RandomKey, graphicsObj);
			}
			else if (myPointerState == PointerState.PlaceEvent)
			{
				myNodeRenderer.DrawCursorNode(mousePos, NodeType.EventKey, graphicsObj);
			}
			else if (myPointerState == PointerState.OneWay && selectedNode != null)
			{
				myNodeRenderer.DrawCursorOneWayConnection(selectedNode, mousePos, graphicsObj);
			}
			else if (myPointerState == PointerState.TwoWay && selectedNode != null)
			{
				myNodeRenderer.DrawCursorTwoWayConnection(selectedNode, mousePos, graphicsObj);
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

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			var mousePos = new Vector2(e.X, e.Y);
			NodeBase newNode = null;
			if (myPointerState == PointerState.PlaceBlank ||
				myPointerState == PointerState.PlaceLock ||
				myPointerState == PointerState.PlaceRandom ||
				myPointerState == PointerState.PlaceEvent)
			{
				switch(myPointerState)
				{
					case PointerState.PlaceBlank:
						newNode = new BlankNode();
						break;
					case PointerState.PlaceLock:
						newNode = new LockNode();
						break;
					case PointerState.PlaceRandom:
						newNode = new RandomKeyNode();
						break;
					case PointerState.PlaceEvent:
						newNode = new EventKeyNode();
						break;
				}
				
				newNode.myPos = (mousePos / ZoomScale) - imageBasePos;

				myNodes.Add(newNode);

				selectedNode = newNode;
				carriedNode = newNode;
				selectedOffset = new Vector2(0, 0);

				UpdateNodeSettings();
				UncheckStateBoxes();
			}
			else
			{
				var width = NodeRenderer.nodeSize;
				var height = NodeRenderer.nodeSize;
				
				foreach (var node in myNodes)
				{
					var screenSpaceNodePos = TranslateVector(node.myPos);

					if (mousePos.x > (screenSpaceNodePos.x - width / 2) && mousePos.y > (screenSpaceNodePos.y - height / 2) &&
						mousePos.x < (screenSpaceNodePos.x + width / 2) && mousePos.y < (screenSpaceNodePos.y + height / 2))
					{
						newNode = node;
						break;
					}
				}

				if (newNode != null)
				{
					if (myPointerState == PointerState.TwoWay && selectedNode != null)
					{
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
					else if (myPointerState == PointerState.OneWay && selectedNode != null)
					{
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
						carriedNode = newNode;
						selectedNode = newNode;
						
						selectedOffset = (mousePos - TranslateVector(newNode.myPos)) / ZoomScale;

						UpdateNodeSettings();
					}
				}
			}

			// No node was hit, activate map dragging
			if (newNode == null)
			{
				carriedMap = true;

				mapPickedUpPos = imageBasePos.Clone();
				selectedOffset = (mousePos / ZoomScale) - imageBasePos;
			}

			panel1.Refresh();
		}

		private void panel1_MouseUp(object sender, MouseEventArgs e)
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

			carriedNode = null;
			carriedMap = false;

			panel1.Refresh();
		}

		private void panel1_MouseMove(object sender, MouseEventArgs e)
		{
			mousePos = new Vector2(e.X, e.Y);

			if (carriedNode != null)
			{
				carriedNode.myPos = (mousePos / ZoomScale) - (selectedOffset + imageBasePos);
			}

			if(carriedMap)
			{
				imageBasePos = (mousePos / ZoomScale) - selectedOffset;
			}

			UpdatePointerState();

			panel1.Refresh();
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
				panel1.Refresh();
			}
		}

		private void panel1_MouseHover(object sender, EventArgs e)
		{
			panel1.Focus();

			UpdatePointerState();

			panel1.Refresh();
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
			panel1.Refresh();
		}

		private void panel1_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Delete)
			{
				if(selectedNode != null)
				{
					foreach(var node in myNodes)
					{
						node.myConnections.Remove(selectedNode);
					}
					myNodes.Remove(selectedNode);

					selectedNode = null;
					carriedNode = null;

					UpdateNodeSettings();

					panel1.Refresh();
				}
			}

			UpdatePointerState();

			panel1.Refresh();
		}

		private void panel1_KeyUp(object sender, KeyEventArgs e)
		{
			UpdatePointerState();

			panel1.Refresh();
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
			panel1.Refresh();
		}

		private void InitializeNodes()
		{
			myNodes = SaveManager.Data.Nodes;
			foreach (var node in myNodes)
			{
				node.FormConnections(myNodes);
			}
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveManager.New();

			KeyManager.Initialize(SaveManager.Data);
			InitializeNodes();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();

			dialog.Filter = "logic files (*.lgc)|*.lgc";

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				if (SaveManager.Open(dialog.FileName))
				{
					KeyManager.Initialize(SaveManager.Data);
					InitializeNodes();
				}
				else
				{
					MessageBox.Show($"Opening {dialog.FileName} failed", "Load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveNodes();
			KeyManager.SaveKeys(SaveManager.Data);

			Save();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveNodes();
			KeyManager.SaveKeys(SaveManager.Data);

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
