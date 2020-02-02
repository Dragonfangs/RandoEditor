using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Common.Node;
using Common.Key.Requirement;
using Common.Key;

namespace RandoEditor
{
	public partial class LockPanelLogic : UserControl
	{
		public class DropDownTreeNode : TreeNode
		{
			public DropDownTreeNode(ComplexRequirement req)
				:base()
			{
				Tag = req;

				m_ComboBox.Tag = this;

				m_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

				m_ComboBox.Items.AddRange(((RequirementType[])Enum.GetValues(typeof(RequirementType))).Select(x => RequirementTypeConverter.Convert(x)).ToArray());

				if (req.myType == RequirementType.AND)
				{
					m_ComboBox.SelectedItem = RequirementTypeConverter.Convert(RequirementType.AND);
				}
				else if (req.myType == RequirementType.OR)
				{
					m_ComboBox.SelectedItem = RequirementTypeConverter.Convert(RequirementType.OR);
				}
			}

			private ComboBox m_ComboBox = new ComboBox();
			public ComboBox ComboBox
			{
				get
				{
					return m_ComboBox;
				}
			}
		}

		public class AddingTreeNode : TreeNode
		{
			public AddingTreeNode(Requirement req)
				: base()
			{
				btnAddComplex.Tag = this;
				btnAddComplex.BackgroundImage = Properties.Resources.ComplexRequirement;
				btnAddComplex.BackgroundImageLayout = ImageLayout.Zoom;
				myToolTip.SetToolTip(btnAddComplex, "Add Complex Requirement");
				btnAddSimple.Tag = this;
				btnAddSimple.BackgroundImage = Properties.Resources.SimpleRequirement;
				btnAddSimple.BackgroundImageLayout = ImageLayout.Zoom;
				myToolTip.SetToolTip(btnAddSimple, "Add Simple Requirement");

				Tag = req;
			}

			public Button btnAddComplex = new Button();
			public Button btnAddSimple = new Button();

			private ToolTip myToolTip = new ToolTip();
		}

		public class KeyTreeNode : TreeNode
		{
			public KeyTreeNode(SimpleRequirement req)
				: base(req.GetKey().Name)
			{
				Tag = req;

				if (req.GetKey().Repeatable == true)
				{
					Text = $"          {req.GetKey().Name}";

					m_NumericUpDown.Tag = this;

					m_NumericUpDown.Minimum = 1;
					m_NumericUpDown.Maximum = 9;
					m_NumericUpDown.Increment = 1;

					m_NumericUpDown.Value = req.myRepeatCount;
				}
			}

			public KeyTreeNode(ComplexRequirement req)
				: base(RequirementTypeConverter.Convert(req.myType))
			{
				Tag = req;
			}

			public bool Deletable { get; set; } = true;

			private NumericUpDown m_NumericUpDown = new NumericUpDown();
			public NumericUpDown NumericUpDown
			{
				get
				{
					return m_NumericUpDown;
				}
			}
		}

		public class SeparatorTreeNode : TreeNode
		{
			public SeparatorTreeNode(RequirementType type)
				: base(GetSeparatorForType(type))
			{

			}
		}

		public LockPanelLogic()
		{
			InitializeComponent();
		}

		public void SetNode(ComplexRequirement req)
		{
			myRequirement = req;

			RefreshNode();
		}

		public void RefreshNode()
		{
			HideControls(treeView1.Nodes);

			treeView1.Nodes.Clear();

			treeView1.Nodes.Add(GenerateTreeNode(myRequirement));

			foreach (TreeNode node in treeView1.Nodes)
			{
				ExpandTreeNode(node);
			}
		}

		private void ExpandTreeNode(TreeNode node)
		{
			if (node is KeyTreeNode keyNode)
				return;

			node.Expand();
			foreach (TreeNode child in node.Nodes)
			{
				ExpandTreeNode(child);
			}
		}

		private ComplexRequirement myRequirement = new ComplexRequirement();
		
		private static string GetSeparatorForType(RequirementType type)
		{
			switch (type)
			{
				case RequirementType.AND:
					return "- AND -";
				case RequirementType.OR:
					return "- OR -";
			}

			return string.Empty;
		}

		private KeyTreeNode GenerateLeafNode(Requirement aRequirement)
		{
			if (aRequirement is SimpleRequirement sReq)
			{
				var parent = new KeyTreeNode(sReq);

				if (sReq.GetKey() is ComplexKey cKey)
				{
					var childReqs = cKey.myRequirement.myRequirements.Select(req => GenerateLeafNode(req)).ToList();
					foreach (var child in childReqs)
					{
						child.Deletable = false;
						parent.Nodes.Add(child);
					}
				}

				GenerateSeparators(parent);
				return parent;
			}
			
			if (aRequirement is ComplexRequirement cReq)
			{
				var parent = new KeyTreeNode(cReq);

				var childReqs = cReq.myRequirements.Select(req => GenerateLeafNode(req)).ToList();
				foreach (var child in childReqs)
				{
					child.Deletable = false;
					parent.Nodes.Add(child);
				}

				GenerateSeparators(parent);
				return parent;
			}

			// Impossible
			return null;
		}

		private TreeNode GenerateTreeNode(Requirement aRequirement)
		{
			if(aRequirement is SimpleRequirement sReq)
			{
				return GenerateLeafNode(sReq);
			}

			if (aRequirement is ComplexRequirement cReq)
			{
				var parent = new DropDownTreeNode(cReq);

				var childReqs = cReq.myRequirements.Select(req => GenerateTreeNode(req)).ToList();
				foreach(var child in childReqs)
				{
					parent.Nodes.Add(child);
				}
				
				parent.Nodes.Add(new AddingTreeNode(cReq));
				GenerateSeparators(parent);
				return parent;
			}

			// Impossible
			return null;
		}

		private void GenerateSeparators(TreeNode aNode)
		{
			var nodesToRemove = aNode.Nodes.Cast<TreeNode>().Where(node => (node is SeparatorTreeNode)).ToList();
			
			foreach (var node in nodesToRemove)
			{
				node.Remove();
			}

			ComplexRequirement req = null;
			if (aNode.Tag is ComplexRequirement cReq)
			{
				req = cReq;
			}

			if (aNode.Tag is SimpleRequirement sReq && sReq.GetKey() is ComplexKey cKey)
			{
				req = cKey.myRequirement;
			}

			if (req == null)
				return;

			var nodesThatShouldHaveSeparator = aNode.Nodes.Cast<TreeNode>().Take(aNode.Nodes.Count - 2).ToList();
			foreach (TreeNode node in nodesThatShouldHaveSeparator)
			{
				aNode.Nodes.Insert(node.Index + 1, new SeparatorTreeNode(req.myType));
			}
		}

		private void ShowControls()
		{
			ShowControls(treeView1.Nodes);
		}

		private void ShowControls(TreeNodeCollection collection)
		{
			foreach (TreeNode node in collection)
			{
				if (node is KeyTreeNode keyTreeNode)
				{
					if (keyTreeNode.Tag is SimpleRequirement req && req.GetKey()?.Repeatable == true)
					{
						treeView1.Controls.Add(keyTreeNode.NumericUpDown);

						keyTreeNode.NumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);

						keyTreeNode.NumericUpDown.Show();
					}
				}

				if (node is DropDownTreeNode dropDownNode)
				{
					treeView1.Controls.Add(dropDownNode.ComboBox);

					dropDownNode.ComboBox.SelectedValueChanged += new EventHandler(ComboBox_SelectedValueChanged);
					dropDownNode.ComboBox.KeyDown += new KeyEventHandler(ComboBox_KeyDown);

					dropDownNode.ComboBox.Show();
				}

				if (node is AddingTreeNode addingNode)
				{
					treeView1.Controls.Add(addingNode.btnAddComplex);
					treeView1.Controls.Add(addingNode.btnAddSimple);

					addingNode.Text = "                  ";
					addingNode.btnAddComplex.Click += new EventHandler(btnAddComplex_Clicked);
					addingNode.btnAddSimple.Click += new EventHandler(btnAddSimple_Clicked);

					addingNode.btnAddComplex.Show();
					addingNode.btnAddSimple.Show();
				}

				if (node.IsExpanded)
					ShowControls(node.Nodes);
			}

			UpdateControls();
		}

		private void HideControls()
		{
			HideControls(treeView1.Nodes);
		}

		private void HideControls(TreeNodeCollection collection)
		{
			foreach (TreeNode node in collection)
			{
				if (node is KeyTreeNode keyTreeNode)
				{
					keyTreeNode.NumericUpDown.ValueChanged -= NumericUpDown_ValueChanged;
					
					keyTreeNode.NumericUpDown.Hide();

					treeView1.Controls.Remove(keyTreeNode.NumericUpDown);
				}

				if (node is DropDownTreeNode dropDownNode)
				{
					dropDownNode.ComboBox.SelectedValueChanged -= ComboBox_SelectedValueChanged;
					dropDownNode.ComboBox.KeyDown -= ComboBox_KeyDown;

					dropDownNode.ComboBox.Hide();
					dropDownNode.ComboBox.DroppedDown = false;
					
					treeView1.Controls.Remove(dropDownNode.ComboBox);
				}

				if (node is AddingTreeNode addingNode)
				{
					addingNode.btnAddComplex.Click -= btnAddComplex_Clicked;
					addingNode.btnAddSimple.Click -= btnAddSimple_Clicked;

					addingNode.btnAddComplex.Hide();
					addingNode.btnAddSimple.Hide();

					treeView1.Controls.Remove(addingNode.btnAddComplex);
					treeView1.Controls.Remove(addingNode.btnAddSimple);
				}

				if (node.IsExpanded)
					HideControls(node.Nodes);
			}
		}

		private void UpdateControls()
		{
			UpdateControls(treeView1.Nodes);
		}

		private void UpdateControls(TreeNodeCollection collection)
		{
			foreach (TreeNode node in collection)
			{
				if (node is KeyTreeNode keyTreeNode)
				{
					keyTreeNode.NumericUpDown.SetBounds(
						keyTreeNode.Bounds.X,
						keyTreeNode.Bounds.Y,
						30,
						keyTreeNode.Bounds.Height);
				}

				if (node is DropDownTreeNode dropDownNode)
				{
					dropDownNode.ComboBox.SetBounds(
						dropDownNode.Bounds.X,
						dropDownNode.Bounds.Y,
						dropDownNode.Bounds.Width + 50,
						dropDownNode.Bounds.Height);
				}

				if (node is AddingTreeNode addingNode)
				{
					addingNode.btnAddComplex.SetBounds(
						addingNode.Bounds.X,
						addingNode.Bounds.Y,
						22,
						addingNode.Bounds.Height);

					addingNode.btnAddSimple.SetBounds(
						addingNode.btnAddComplex.Bounds.X + addingNode.btnAddComplex.Bounds.Width + 5,
						addingNode.Bounds.Y,
						22,
						addingNode.Bounds.Height);
				}

				if (node.IsExpanded)
					UpdateControls(node.Nodes);
			}
		}

		private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			ShowControls();
		}

		private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
		{
			ShowControls();
		}

		private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			HideControls();
		}

		private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			HideControls();
		}

		private void treeView1_Scroll(object sender, ScrollEventArgs e)
		{
			UpdateControls();
		}

		private void treeView1_MouseWheel(object sender, MouseEventArgs e)
		{
			UpdateControls();
		}

		void NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (sender is NumericUpDown num &&
				num.Tag is TreeNode parentNode &&
				parentNode.Tag is SimpleRequirement req && 
				req.GetKey()?.Repeatable == true)
			{
				req.myRepeatCount = (uint)num.Value;
			}
		}

		void ComboBox_SelectedValueChanged(object sender, EventArgs e)
		{
			if (sender is ComboBox box && box.Tag is TreeNode parentNode && parentNode.Tag is ComplexRequirement req)
			{
				req.myType = RequirementTypeConverter.Convert(box.SelectedItem.ToString());

				GenerateSeparators(parentNode);
			}
		}

		void ComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (sender is ComboBox box && box.Tag is TreeNode node && e.KeyCode == Keys.Delete)
			{
				if (node.Parent != null && (node is KeyTreeNode || node is DropDownTreeNode))
				{
					HideControls();
					var parentNode = node.Parent;
					(parentNode.Tag as ComplexRequirement).myRequirements.Remove(node.Tag as Requirement);
					node.Remove();
					GenerateSeparators(parentNode);
					ShowControls();
				}
			}
		}

		void btnAddComplex_Clicked(object sender, EventArgs e)
		{
			if (sender is Button btn && btn.Tag is TreeNode parentNode && parentNode.Tag is ComplexRequirement requirement)
			{
				var newReq = new ComplexRequirement();
				requirement.myRequirements.Add(newReq);

				var newNode = new DropDownTreeNode(newReq);
				newNode.Nodes.Add(new AddingTreeNode(newReq));
				
				HideControls();
				parentNode.Parent.Nodes.Insert(parentNode.Parent.Nodes.Count - 1, newNode);
				GenerateSeparators(parentNode.Parent);
				newNode.Expand();
			}
		}

		void btnAddSimple_Clicked(object sender, EventArgs e)
		{
			if (sender is Button btn && btn.Tag is TreeNode parentNode && parentNode.Tag is ComplexRequirement requirement)
			{
				var keySelector = new KeySelector();
				keySelector.ShowDialog();
				
				foreach (var key in keySelector.SelectedKeys.Distinct())
				{
					if (!requirement.myRequirements.Any(req => req is SimpleRequirement sReq && sReq.GetKey() == key))
					{
						var newReq = new SimpleRequirement(key);
						requirement.myRequirements.Add(newReq);
						
						parentNode.Parent.Nodes.Insert(parentNode.Parent.Nodes.Count - 1, GenerateLeafNode(newReq));
					}
				}

				HideControls();
				GenerateSeparators(parentNode.Parent);
				ShowControls();
			}
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				var nodeToRemove = treeView1.SelectedNode;
				if (nodeToRemove.Parent != null && ((nodeToRemove is KeyTreeNode keyNode && keyNode.Deletable) || nodeToRemove is DropDownTreeNode))
				{
					HideControls();
					var parentNode = nodeToRemove.Parent;
					(parentNode.Tag as ComplexRequirement).myRequirements.Remove(nodeToRemove.Tag as Requirement);
					nodeToRemove.Remove();
					GenerateSeparators(parentNode);
					ShowControls();
				}
			}
		}
	}
}
