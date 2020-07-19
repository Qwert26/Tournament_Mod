using UnityEngine;
using System;
namespace TournamentMod
{
	public enum GradientType : int
	{
		YouTubeBufferHealth,
		CustomBattleMode,
		CorrelationMap,
		TemperaturColorLinear,
		TemperaturColorLogarithmic,
		TemperaturColorOrdinal,
		TrafficLight
	}
	public static class GradientTypeExtensions
	{
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
				default:
					throw new ArgumentOutOfRangeException("type", type, "This Gradient has not yet defined!");
			}
		}
	}
}