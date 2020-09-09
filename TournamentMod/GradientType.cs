using UnityEngine;
using System;
namespace TournamentMod
{
	/// <summary>
	/// Available Gradients
	/// </summary>
	public enum GradientType : int
	{
		YouTubeBufferHealth,
		CustomBattleMode,
		CorrelationMap,
		TemperaturColorLinear,
		TemperaturColorLogarithmic,
		TemperaturColorOrdinal,
		TrafficLight,
		Wintergatan_MMX_PMMP,
		RoyalHourglass
	}
	public static class GradientTypeExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Gradient GetGradient(this GradientType type)
		{
			switch (type)
			{
				case GradientType.YouTubeBufferHealth:
					return PenaltyTimeGradient.YOUTUBE_BUFFER_HEALTH;
				case GradientType.CustomBattleMode:
					return PenaltyTimeGradient.CUSTOM_BATTLE_MODE;
				case GradientType.CorrelationMap:
					return PenaltyTimeGradient.CORRELATION_MAP;
				case GradientType.TemperaturColorLinear:
					return PenaltyTimeGradient.TEMPERATUR_COLOR_LINEAR;
				case GradientType.TemperaturColorLogarithmic:
					return PenaltyTimeGradient.TEMPERATUR_COLOR_LOGARITHMIC;
				case GradientType.TemperaturColorOrdinal:
					return PenaltyTimeGradient.TEMPERATUR_COLOR_ORDINAL;
				case GradientType.TrafficLight:
					return PenaltyTimeGradient.TRAFFIC_LIGHT;
				case GradientType.Wintergatan_MMX_PMMP:
					return PenaltyTimeGradient.WINTERGATAN_PROJECT_MASTER_PLAN;
				case GradientType.RoyalHourglass:
					return PenaltyTimeGradient.ROYAL_HOURGLASS;
				default:
					throw new ArgumentOutOfRangeException("type", type, "This Gradient is not yet defined!");
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetDescription(this GradientType type)
		{
			switch (type)
			{
				case GradientType.YouTubeBufferHealth:
					return "Based on the Health of the Buffer from YouTube-Videos. Critical accumulated time is at the 87.5%(7/8)-Mark.";
				case GradientType.CustomBattleMode:
					return "The same Gradient also found in the Custom Battle Mode.";
				case GradientType.CorrelationMap:
					return "Based on the mapping used for correlation matricies.";
				case GradientType.TemperaturColorLinear:
					return "Based on the color of a star relative to Vega. This one is based on a linear interpolation of the temperatur and has a long blue-phase. Critical accumulated time is near the 96%-Mark.";
				case GradientType.TemperaturColorLogarithmic:
					return "Based on the color of a star relative to Vega. This one is based on a logarithmic interpolation of the temperatur and has a long red-phase. Critical accumulated time is near the 38%-Mark.";
				case GradientType.TemperaturColorOrdinal:
					return "Based on the color of a star relative to Vega. This one is based on a ordinal interpolation of the spectral class. Critical accumulated time is near the 82%-Mark.";
				case GradientType.TrafficLight:
					return "Based on the colors of a traffic light. Simple yet informative. Critical accumulated time is at the 75%-Mark.";
				case GradientType.Wintergatan_MMX_PMMP:
					return "Based on the colors used in Wintergatan's Project Management Masterplan for the Marble Machine X.";
				case GradientType.RoyalHourglass:
					return "Based on the color-stages of purpur.";
				default:
					throw new ArgumentOutOfRangeException("type", type, "This Gradient is not yet defined!");
			}
		}
	}
}