﻿using Common.Key;
using Common.Node;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common.Utils
{
	public class Utility
	{
		public static string GetNodeName(NodeBase node)
		{
			if (node is RandomKeyNode randomNode)
			{
				return randomNode.myRandomKeyIdentifier;
			}

			if (node is EventKeyNode eventNode && eventNode.myKeyId.HasValue)
			{
				return KeyManager.GetKey(eventNode.myKeyId.Value)?.Name;
			}

			return string.Empty;
		}
		public static List<ListViewItem> GenerateKeyList(ListView listView)
		{
			var returnList = new List<ListViewItem>();

			foreach (var key in KeyManager.GetRandomKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Group = GetGroup(listView, "listViewGroupRandom");
				item.Tag = key;
				returnList.Add(item);
			}

			foreach (var key in KeyManager.GetEventKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Group = GetGroup(listView, "listViewGroupEvents");
				item.Tag = key;
				returnList.Add(item);
			}

			foreach (var key in KeyManager.GetSettingKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Group = GetGroup(listView, "listViewGroupSettings");
				item.Tag = key;
				returnList.Add(item);
			}

			foreach (var key in KeyManager.GetCustomKeys())
			{
				var item = new ListViewItem(key.Name);
				item.Group = GetGroup(listView, "listViewGroupCustom");
				item.Tag = key;
				returnList.Add(item);
			}

			return returnList;
		}

		private static ListViewGroup GetGroup(ListView listView, string name)
		{
			foreach (ListViewGroup group in listView.Groups)
			{
				if (group.Name == name)
				{
					return group;
				}
			}

			return null;
		}

		/// <summary>
		/// Resize the image to the specified width and height.
		/// </summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}

		public static double AngleBetween(Vector2 vector1, Vector2 vector2)
		{
			double sin = vector1.x * vector2.y - vector2.x * vector1.y;
			double cos = vector1.x * vector2.x + vector1.y * vector2.y;

			return Math.Atan2(sin, cos) * (180 / Math.PI);
		}

		public static bool RectContains(Rectangle aRect, Point aPoint)
		{
			// If point is outside horizontally
			if (aPoint.X > aRect.Right || aPoint.X < aRect.Left)
				return false;

			// If point is outside vertically
			if (aPoint.Y > aRect.Bottom || aPoint.Y < aRect.Top)
				return false;

			return true;
		}

		// Calculate the distance between
		// point pt and the segment p1 --> p2.
		public static double FindDistanceToSegment(
			PointF pt, PointF p1, PointF p2)
		{
			float dx = p2.X - p1.X;
			float dy = p2.Y - p1.Y;
			if ((dx == 0) && (dy == 0))
			{
				// It's a point not a line segment.
				dx = pt.X - p1.X;
				dy = pt.Y - p1.Y;
				return Math.Sqrt(dx * dx + dy * dy);
			}

			// Calculate the t that minimizes the distance.
			float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
				(dx * dx + dy * dy);

			// See if this represents one of the segment's
			// end points or a point in the middle.
			if (t < 0)
			{
				dx = pt.X - p1.X;
				dy = pt.Y - p1.Y;
			}
			else if (t > 1)
			{
				dx = pt.X - p2.X;
				dy = pt.Y - p2.Y;
			}
			else
			{
				var closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
				dx = pt.X - closest.X;
				dy = pt.Y - closest.Y;
			}

			return Math.Sqrt(dx * dx + dy * dy);
		}

		// Find the point of intersection between
		// the lines p1 --> p2 and p3 --> p4.
		private static bool FindIntersection(
			PointF p1, PointF p2, PointF p3, PointF p4)
		{
			// Get the segments' parameters.
			float dx12 = p2.X - p1.X;
			float dy12 = p2.Y - p1.Y;
			float dx34 = p4.X - p3.X;
			float dy34 = p4.Y - p3.Y;

			// Solve for t1 and t2
			float denominator = (dy12 * dx34 - dx12 * dy34);

			float t1 =
				((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
					/ denominator;
			if (float.IsInfinity(t1))
			{
				// The lines are parallel (or close enough to it).
				return false;
			}

			float t2 =
				((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
					/ -denominator;

			// The segments intersect if t1 and t2 are between 0 and 1.
			return ((t1 >= 0) && (t1 <= 1) &&
				 (t2 >= 0) && (t2 <= 1));
		}

		public static bool LineInRect(Rectangle aRect, Point point1, Point point2)
		{
			var rectPoint1 = new Point(aRect.Left, aRect.Top);
			var rectPoint2 = new Point(aRect.Right, aRect.Top);
			var rectPoint3 = new Point(aRect.Left, aRect.Bottom);
			var rectPoint4 = new Point(aRect.Right, aRect.Bottom);

			if(FindIntersection(rectPoint1, rectPoint2, point1, point2) ||
				FindIntersection(rectPoint1, rectPoint3, point1, point2) ||
				FindIntersection(rectPoint2, rectPoint4, point1, point2) ||
				FindIntersection(rectPoint3, rectPoint4, point1, point2))
			{
				return true;
			}
			
			return RectContains(aRect, point1) || RectContains(aRect, point2);
		}

		public static bool RectIntersect(Rectangle aRect, Rectangle anotherRect)
		{
			// If one rectangle is on left side of other
			if (aRect.Left > anotherRect.Right || anotherRect.Left > aRect.Right)
				return false;

			// If one rectangle is above other 
			if (aRect.Top > anotherRect.Bottom || anotherRect.Top > aRect.Bottom)
				return false;

			return true;
		}

		public static float CalcDiag(int width, int height)
		{
			return (float)(Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2)));
		}

		public static Vector2 Rotate(Vector2 v, float rads)
		{
			var retVec = new Vector2(v);
			var sin = (float)Math.Sin(rads);
			var cos = (float)Math.Cos(rads);

			var tx = retVec.x;
			var ty = retVec.y;
			retVec.x = (cos * tx) - (sin * ty);
			retVec.y = (sin * tx) + (cos * ty);
			return retVec;
		}
	}
}
