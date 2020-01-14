using RandoEditor.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoEditor.Map
{
	public class AreaMap
	{
		private float imageCompression = 1.5f;

		//For debug stuff
		private int myDrawnPixelCount;

		private class LODLevel
		{
			public float Diagonal;

			public Bitmap Image;
		}

		private class AreaSplit
		{
			public Vector2 Offset;

			public List<LODLevel> LODs;
		}

		private class Area
		{
			public string Name;

			public Vector2 Offset;

			public Vector2 Size;

			public List<AreaSplit> Splits;
		}

		Dictionary<string, Area> myAreas = new Dictionary<string, Area>();

		public void GenerateAllLODs()
		{
			myAreas.Clear();
			GenerateAreaSplits("Brinstar",	new Vector2(0,0), Properties.Resources.brinstar_real);

			GenerateAreaSplits("Kraid",		new Vector2(-2000, 3100), Properties.Resources.kraid_real);
			GenerateAreaSplits("Norfair",	new Vector2(2800, 3250), Properties.Resources.norfair_real);			
			GenerateAreaSplits("Ridley",	new Vector2(3050, 5700), Properties.Resources.ridley_real);

			GenerateAreaSplits("Tourian",	new Vector2(-3500, 1000), Properties.Resources.tourian_real);
			GenerateAreaSplits("Crateria",	new Vector2(-2000, -2600), Properties.Resources.crateria_real);
			GenerateAreaSplits("Chozodia",	new Vector2(3500, -5000), Properties.Resources.chozodia_real);
		}

		private void GenerateAreaSplits(string areaName, Vector2 offset, Bitmap originalImage)
		{
			var newArea = new Area()
			{
				Name = areaName,
				Offset = offset,
				Size = new Vector2(originalImage.Width, originalImage.Height),
				Splits = new List<AreaSplit>(),
			};

			int tileWidth = 2048;
			int tileHeight = 2048;
			int tileCountX = originalImage.Width / tileWidth;
			int tileCountY = originalImage.Height / tileHeight;

			for (int x = 0; x <= tileCountX; x++)
			{
				var splitOffsetX = x * tileWidth;
				var splitWidth = Math.Min(originalImage.Width - splitOffsetX, tileWidth);
				if (splitWidth > 0)
				{
					for (int y = 0; y <= tileCountY; y++)
					{
						var splitOffsetY = y * tileHeight;
						var splitHeight = Math.Min(originalImage.Height - splitOffsetY, tileHeight);

						if (splitHeight > 0)
						{
							Rectangle tileBounds = new Rectangle(splitOffsetX, splitOffsetY, splitWidth, splitHeight);

							var splitImage = originalImage.Clone(tileBounds, PixelFormat.Format32bppArgb);
							newArea.Splits.Add(GenerateAreaLODs(new Vector2(splitOffsetX, splitOffsetY), splitImage));
						}
					}
				}
			}

			//newArea.Splits.Add(GenerateAreaLODs(new Vector2(0, 0), originalImage));

			myAreas.Add(areaName, newArea);
		}

		private AreaSplit GenerateAreaLODs(Vector2 offset, Bitmap originalImage)
		{
			var newAreaSplit = new AreaSplit()
			{
				Offset = offset,
				LODs = new List<LODLevel>(),
			};

			var scaledImage = originalImage;
			newAreaSplit.LODs.Add(new LODLevel()
			{
				Diagonal = Utility.CalcDiag(scaledImage.Width, scaledImage.Height),
				Image = scaledImage,
			});

			while (newAreaSplit.LODs.Last().Diagonal > 50)
			{
				scaledImage = Utility.ResizeImage(scaledImage, (int)(scaledImage.Size.Width * 0.6f), (int)(scaledImage.Size.Height * 0.6f));

				if (scaledImage == null)
					break;

				newAreaSplit.LODs.Add(new LODLevel()
				{
					Diagonal = Utility.CalcDiag(scaledImage.Width, scaledImage.Height),
					Image = scaledImage,
				});
			}

			return newAreaSplit;
		}

		// Will generate more LODs at a higher resolution
		// Zoom scaling should be more smooth closer to the max resolution but has memory issues...
		private AreaSplit GenerateAreaLODsLogarithmic(string areaName, Vector2 offset, Bitmap originalImage)
		{
			var newAreaSplit = new AreaSplit()
			{
				Offset = offset,
				LODs = new List<LODLevel>(),
			};

			int steps = 4;
			for(int i = 1; i <= steps; i++)
			{
				var scale = Math.Log((100f / steps) * i) / Math.Log(100);
				var scaledImage = Utility.ResizeImage(originalImage, (int)(originalImage.Size.Width * scale), (int)(originalImage.Size.Height * scale));

				if (scaledImage == null)
					break;

				newAreaSplit.LODs.Add(new LODLevel()
				{
					Diagonal = Utility.CalcDiag(scaledImage.Width, scaledImage.Height),
					Image = scaledImage,
				});
			}

			return newAreaSplit;
		}

		private Bitmap GetAppropriateLOD(AreaSplit anAreaSplit, Size aSize)
		{
			var rectDiag = Utility.CalcDiag(aSize.Width, aSize.Height);

			var applicableLODs = anAreaSplit.LODs.Where(lod => lod.Diagonal > (rectDiag / imageCompression));
			if (!applicableLODs.Any())
			{
				return anAreaSplit.LODs.First().Image;
			}

			return applicableLODs.OrderBy(lod => lod.Diagonal).First().Image;
		}

		public void Draw(Vector2 pos, float zoom, Graphics graphicsObj, Rectangle canvasRect)
		{
			myDrawnPixelCount = 0;
			foreach (var area in myAreas.Values)
			{
				DrawArea(area, pos, zoom, graphicsObj, canvasRect);
			}

			Font drawFont = new Font("Arial", 16);
			SolidBrush drawBrush = new SolidBrush(Color.White);
			graphicsObj.DrawString(myDrawnPixelCount.ToString(), drawFont, drawBrush, new Point(50, 50));
		}

		private void DrawArea(Area anArea, Vector2 pos, float zoom, Graphics graphicsObj, Rectangle canvasRect)
		{
			foreach (var areaSplit in anArea.Splits)
			{
				var imagerect = areaSplit.LODs.OrderBy(lod => lod.Diagonal).Last().Image;
				var scaledPos = ((pos + anArea.Offset + areaSplit.Offset) * zoom).ToPoint();
				var scaledSize = new Size((int)(imagerect.Size.Width * zoom), (int)(imagerect.Size.Height * zoom));
				var scaledRect = new Rectangle(scaledPos, scaledSize);

				if (Utility.RectIntersect(scaledRect, canvasRect))
				{
					var img = GetAppropriateLOD(areaSplit, scaledSize);
					graphicsObj.DrawImage(img, scaledRect);
					myDrawnPixelCount += img.Size.Width * img.Size.Height;
				}
			}
		}
	}
}
