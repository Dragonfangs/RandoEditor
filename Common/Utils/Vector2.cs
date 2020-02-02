using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
	[System.Serializable]
	public class Vector2
	{
		[JsonConstructor]
		public Vector2(float someX, float someY)
		{
			x = someX;
			y = someY;
		}

		public Vector2(Vector2 otherVector)
		{
			x = otherVector.x;
			y = otherVector.y;
		}

		public static Vector2 operator +(Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x + v2.x, v1.y + v2.y);
		}

		public static Vector2 operator -(Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x - v2.x, v1.y - v2.y);
		}

		public static Vector2 operator *(Vector2 v, float f)
		{
			return new Vector2(v.x * f, v.y * f);
		}

		public static Vector2 operator /(Vector2 v, float f)
		{
			return new Vector2(v.x / f, v.y / f);
		}

		public Vector2 Clone()
		{
			return new Vector2(x, y);
		}

		public System.Drawing.Point ToPoint()
		{
			return new System.Drawing.Point((int)x, (int)y);
		}

		public System.Drawing.Size ToSize()
		{
			return new System.Drawing.Size((int)x, (int)y);
		}

		public Vector2 Normalized()
		{
			return this / Magnitude();
		}

		public float Magnitude()
		{
			return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}

		public float x = 0;
		public float y = 0;
	}
}
