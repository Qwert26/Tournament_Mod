using System;
using UnityEngine;
namespace TournamentMod.Datatypes
{
	[Serializable]
	public struct Vector4Int
	{
		public int x, y, z, w;
		public Vector4Int(int x = 0, int y = 0, int z = 0, int w = 0) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public int this[int i]
		{
			get
			{
				switch (i)
				{
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					case 3:
						return w;
					default:
						throw new IndexOutOfRangeException($"Index {i} was outside [0;3]!");
				}
			}
			set
			{
				switch (i)
				{
					case 0:
						x = value;
						return;
					case 1:
						y = value;
						return;
					case 2:
						z = value;
						return;
					case 3:
						w = value;
						return;
					default:
						throw new IndexOutOfRangeException($"Index {i} was outside [0;3]!");
				}
			}
		}
		public int SqrMagnitude { get => x * x + y * y + z * z + w * w; }
		public static Vector4Int operator +(Vector4Int a) => a;
		public static Vector4Int operator -(Vector4Int a) => new Vector4Int(-a.x, -a.y, -a.z, -a.w);
		public static Vector4Int operator ~(Vector4Int a) => new Vector4Int(~a.x, ~a.y, ~a.z, ~a.w);
		public static Vector4Int operator +(Vector4Int a, Vector4Int b) => new Vector4Int(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		public static Vector4Int operator -(Vector4Int a, Vector4Int b) => new Vector4Int(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		public static Vector4Int operator *(Vector4Int a, int b) => new Vector4Int(a.x * b, a.y * b, a.z * b, a.w * b);
		public static Vector4Int operator *(int a, Vector4Int b) => b * a;
		public static implicit operator Vector4(Vector4Int a) => new Vector4(a.x, a.y, a.z, a.w);
		public static explicit operator Vector4Int(Vector4 a) => new Vector4Int((int) a.x, (int) a.y, (int) a.z, (int) a.w);
		public static implicit operator Vector3Int(Vector4Int a) => new Vector3Int(a.x, a.y, a.z);
		public static explicit operator Vector4Int(Vector3Int a) => new Vector4Int(a.x, a.y, a.z, 0);
		public static implicit operator Vector2Int(Vector4Int a) => new Vector2Int(a.x, a.y);
		public static explicit operator Vector4Int(Vector2Int a) => new Vector4Int(a.x, a.y, 0, 0);
		public override string ToString()
		{
			return $"({x},{y},{z},{w})";
		}
	}
}
