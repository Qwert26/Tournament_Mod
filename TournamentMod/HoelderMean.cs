using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace TournamentMod
{
	/// <summary>
	/// Implements the Hölder-Mean for p=-Inf, -1, 0, 1, 2, 3 and +Inf.
	/// </summary>
	public class HoelderMean
	{
		private WeightCombinerType wct;
		private readonly List<float> values = new List<float>();
		public HoelderMean(WeightCombinerType wct)
		{
			this.wct = wct;
		}
		public void AddValue(float f)
		{
			values.Add(f);
		}
		public float GetMean(bool clear = true)
		{
			float ret;
			int count = values.Count;
			switch (count)
			{
				case 0:
					throw new InvalidOperationException("No values mean no mean can be calculated!");
				case 1://Skip any calculation.
					ret = values[0];
					if (clear)
					{
						values.Clear();
					}
					return ret;
				default:
					ret = 0;
					switch (wct)
					{
						case WeightCombinerType.MINIMUM:
							ret = values.Min();
							break;
						case WeightCombinerType.HARMONIC:
							foreach (float v in values)
							{
								ret += 1f / v;
							}
							ret = count / ret;
							break;
						case WeightCombinerType.GEOMETRIC:
							ret = 1;
							foreach (float v in values)
							{
								ret *= v;
							}
							ret = Mathf.Pow(ret, 1f / count);
							break;
						case WeightCombinerType.ARITHMETIC:
							ret = values.Average();
							break;
						case WeightCombinerType.SQUARED:
							foreach (float v in values)
							{
								ret += v * v;
							}
							ret = Mathf.Sqrt(ret / count);
							break;
						case WeightCombinerType.CUBIC:
							foreach (float v in values)
							{
								ret += v * v * v;
							}
							ret = Mathf.Pow(ret / count, 1f / 3f);
							break;
						case WeightCombinerType.MAXIMUM:
							ret = values.Max();
							break;
						default:
							ret = float.NaN;
							break;
					}
					if (clear)
					{
						values.Clear();
					}
					return ret;
			}
		}
	}
}