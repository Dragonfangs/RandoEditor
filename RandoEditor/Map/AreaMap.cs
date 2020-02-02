using Common.Utils;
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
		private class LODLevel
		{
			public float Diagonal;

			public Bitmap Image;
		}

		private class Area
		{
			public string Name;

			public Vector2 Offset;

			public Vector2 Size;

			public List<LODLevel> LODs;
		}

		Dictionary<string, Area> myAreas = new Dictionary<string, Area>();

		public void GenerateAllLODs()
		{
			myAreas.Clear();
			GenerateAreaLODs("Brinstar",	new Vector2(0,0), Properties.Resources.brinstar_real);

			GenerateAreaLODs("Kraid",		new Vector2(-2000, 3100), Properties.Resources.kraid_real);
			GenerateAreaLODs("Norfair",		new Vector2(2800, 3250), Properties.Resources.norfair_real);			
			GenerateAreaLODs("Ridley",		new Vector2(3050, 5700), Properties.Resources.ridley_real);

			GenerateAreaLODs("Tourian",		new Vector2(-3500, 1000), Properties.Resources.tourian_real);
			GenerateAreaLODs("Crateria",	new Vector2(-2000, -2600), Properties.Resources.crateria_real);
			GenerateAreaLODs("Chozodia",	new Vector2(3500, -5000), Properties.Resources.chozodia_real);
		}

		private void GenerateAreaLODs(string areaName, Vector2 offset, Bitmap originalImage)
		{
			var newArea = new Area()
			{
				Name = areaName,
				Offset = offset,
				Size = new Vector2(originalImage.Width, originalImage.Height),
				LODs = new List<LODLevel>(),
			};

			var scaledImage = originalImage;
			newArea.LODs.Add(new LODLevel()
			{
				Diagonal = Utility.CalcDiag(scaledImage.Width, scaledImage.Height),
				Image = scaledImage,
			});

			while (newArea.LODs.Last().Diagonal > 50)
			{
				scaledImage = Utility.ResizeImage(scaledImage, (int)(scaledImage.Size.Width * 0.6f), (int)(scaledImage.Size.Height * 0.6f));
				newArea.LODs.Add(new LODLevel()
				{
					Diagonal = Utility.CalcDiag(scaledImage.Width, scaledImage.Height),
					Image = scaledImage,
				});
			}

			myAreas.Add(areaName, newArea);
		}

		// Will generate more LODs at a higher resolution
		// Zoom scaling should be more smooth closer to the max resolution but has memory issues...
		private void GenerateAreaLODsLogarithmic(string areaName, Vector2 offset, Bitmap originalImage)
		{
			var newArea = new Area()
			{
				Name = areaName,
				Offset = offset,
				Size = new Vector2(originalImage.Width, originalImage.Height),
				LODs = new List<LODLevel>(),
			};

			int steps = 4;
			for(int i = 1; i <= steps; i++)
			{
				var scale = Math.Log((100f / steps) * i) / Math.Log(100);
				var scaledImage = Utility.ResizeImage(originalImage, (int)(originalImage.Size.Width * scale), (int)(originalImage.Size.Height * scale));
				newArea.LODs.Add(new LODLevel()
				{
					Diagonal = Utility.CalcDiag(scaledImage.Width, scaledImage.Height),
					Image = scaledImage,
				});
			}

			myAreas.Add(areaName, newArea);
		}

		private Bitmap GetAppropriateLOD(Area anArea, Size aSize)
		{
			var rectDiag = Utility.CalcDiag(aSize.Width, aSize.Height);

			//10 is Max quality, maybe save this as not a magic number somewhere?
			var maxQuality = 10f;
			var maxScale = 7f;
			var scale = maxScale / maxQuality;
			float invertedQualitySetting = maxQuality - (int)Properties.Settings.Default["MapQuality"];

			var compressionScale = 1 + (invertedQualitySetting * scale);

			var applicableLODs = anArea.LODs.Where(lod => lod.Diagonal > (rectDiag / compressionScale));
			if (!applicableLODs.Any())
			{
				return anArea.LODs.First().Image;
			}

			return applicableLODs.OrderBy(lod => lod.Diagonal).First().Image;
		}

		public void Draw(Vector2 pos, float zoom, Graphics graphicsObj, Rectangle canvasRect)
		{
			foreach (var area in myAreas.Values)
			{
				DrawArea(area, pos, zoom, graphicsObj, canvasRect);
			}
		}

		private void DrawArea(Area anArea, Vector2 pos, float zoom, Graphics graphicsObj, Rectangle canvasRect)
		{
			var imagerect = anArea.LODs.OrderBy(lod => lod.Diagonal).Last().Image;
			var scaledPos = ((pos + anArea.Offset)* zoom).ToPoint();
			var scaledSize = new Size((int)(imagerect.Size.Width * zoom), (int)(imagerect.Size.Height * zoom));
			var scaledRect = new Rectangle(scaledPos, scaledSize);

			if (Utility.RectIntersect(scaledRect, canvasRect))
			{
				graphicsObj.DrawImage(GetAppropriateLOD(anArea, scaledSize), scaledRect);
			}
		}
	}
}
