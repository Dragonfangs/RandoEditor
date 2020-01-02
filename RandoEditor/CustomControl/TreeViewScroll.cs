using System;
using System.Windows.Forms;

namespace RandoEditor.CustomControl
{
	public class TreeViewScroll : TreeView
	{
		private const int WM_VSCROLL = 0x0115;
		private const int WM_HSCROLL = 0x0114;
		private const int SB_HORZ = 0;
		private const int SB_VERT = 1;

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern int GetScrollPos(int hWnd, int nBar);
		public int HScrollPos
		{
			get { return GetScrollPos((int)Handle, SB_VERT); }
		}
		public int VScrollPos
		{
			get { return GetScrollPos((int)Handle, SB_HORZ); }
		}

		public event ScrollEventHandler Scroll;
		protected override void WndProc(ref Message m)
		{
			if (Scroll != null)
			{
				switch (m.Msg)
				{
					case WM_VSCROLL:
					case WM_HSCROLL:
						{
							ScrollEventType
							t = (ScrollEventType)Enum.Parse(typeof(ScrollEventType), (m.WParam.ToInt32() & 65535).ToString());
							Scroll(m.HWnd, new ScrollEventArgs(t, ((int)(m.WParam.ToInt64() >> 16)) & 255));
							break;
						}
				}
			}

			base.WndProc(ref m);
		}
	}
}
