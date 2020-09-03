using UnityEngine;
namespace TournamentMod {
	/// <summary>
	/// Holds all available Gradients for the penalty time indicator.
	/// </summary>
	public static class PenaltyTimeGradient {
		/// <summary>
		/// The Gradient used by YouTube for visualizing Buffer-Health.
		/// </summary>
		public static readonly Gradient YOUTUBE_BUFFER_HEALTH;
		/// <summary>
		/// The Gradient used by the Custom Battle Mode.
		/// </summary>
		public static readonly Gradient CUSTOM_BATTLE_MODE;
		/// <summary>
		/// The Gradient used by Correlation-Matricies.
		/// </summary>
		public static readonly Gradient CORRELATION_MAP;
		/// <summary>
		/// The Gradient extracted from Spectral classes of stars with linear interploation. Long Blue-Phase.
		/// </summary>
		public static readonly Gradient TEMPERATUR_COLOR_LINEAR;
		/// <summary>
		/// The Gradient extracted from Spectral classes of stars with logarithmic interpolation. Long Red-Phase.
		/// </summary>
		public static readonly Gradient TEMPERATUR_COLOR_LOGARITHMIC;
		/// <summary>
		/// The Gradient extracted from Spectral classes of stars with ordinal interpolation.
		/// </summary>
		public static readonly Gradient TEMPERATUR_COLOR_ORDINAL;
		/// <summary>
		/// A Traffic light inspired gradient.
		/// </summary>
		public static readonly Gradient TRAFFIC_LIGHT;
		/// <summary>
		/// The Gradient constructed by the colors used in Wintergatan's Project Management Masterplan for the MMX.
		/// </summary>
		public static readonly Gradient WINTERGATAN_PROJECT_MASTER_PLAN;
		/// <summary>
		/// The Gradient constructed by the royal colors of purpur. Later stages correspond with higher penalty time.
		/// </summary>
		public static readonly Gradient ROYAL_HOURGLASS;
		/// <summary>
		/// Static Initializer: Gets called when the class gets loaded.
		/// </summary>
		static PenaltyTimeGradient() {
			YOUTUBE_BUFFER_HEALTH = new Gradient
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(Color.white, 0f),
					new GradientColorKey(Color.blue, 0.25f),
					new GradientColorKey(Color.green, 0.5f),
					new GradientColorKey(new Color(1f, 1f, 0f), 0.75f), //Yellow
					new GradientColorKey(new Color(1f, 0.5f, 0f), 0.875f), //Orange
					new GradientColorKey(Color.red, 1f)
				},
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(1f, 1f)
				},
				mode = GradientMode.Blend
			};
			CUSTOM_BATTLE_MODE = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(Color.white, 0f),
					new GradientColorKey(Color.red, 1f)
				},
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(1f, 1f)
				},
				mode = GradientMode.Blend
			};
			CORRELATION_MAP = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(Color.blue, 0f),
					new GradientColorKey(Color.white, 0.5f),
					new GradientColorKey(Color.red, 1f)
				},
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(1f, 1f)
				},
				mode = GradientMode.Blend
			};
			TEMPERATUR_COLOR_LINEAR = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(Color.blue, 0f), //40000K
					new GradientColorKey(new Color(0.5f, 0.5f, 1f), 0.358126721763f), //27000K: blue-white
					new GradientColorKey(Color.white, 0.826446280992f), //10000K
					new GradientColorKey(new Color(1f, 1f, 0.5f), 0.903581267218f), //7200K: yellow-white
					new GradientColorKey(new Color(1f, 1f, 0f), 0.936639118457f), //6000K: Yellow
					new GradientColorKey(new Color(1f, 0.5f, 0f), 0.961432506887f), //5100K: Orange
					new GradientColorKey(Color.red, 1f) //3700K
				},
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(1f, 1f)
				},
				mode = GradientMode.Blend
			};
			TEMPERATUR_COLOR_LOGARITHMIC = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(Color.blue, 0f), //40000K
					new GradientColorKey(new Color(0.5f, 0.5f, 1f), 0.0717047055393f), //27000K: blue-white
					new GradientColorKey(Color.white, 0.252908544677f), //10000K
					new GradientColorKey(new Color(1f, 1f, 0.5f), 0.312839168236f), //7200K: yellow-white
					new GradientColorKey(new Color(1f, 1f, 0f), 0.346100992626f), //6000K: Yellow
					new GradientColorKey(new Color(1f, 0.5f, 0f), 0.375750125663f), //5100K: Orange
					new GradientColorKey(Color.red, 1f) //3700K
				},
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(1f, 1f)
				},
				mode = GradientMode.Blend
			};
			TEMPERATUR_COLOR_ORDINAL = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(Color.blue, 0 / 55f), //40000K
					new GradientColorKey(new Color(0.5f, 0.5f, 1f), 5 / 55f), //27000K: blue-white
					new GradientColorKey(Color.white, 15 / 55f), //10000K
					new GradientColorKey(new Color(1f, 1f, 0.5f), 25 / 55f), //7200K: yellow-white
					new GradientColorKey(new Color(1f, 1f, 0f), 35 / 55f), //6000K: Yellow
					new GradientColorKey(new Color(1f, 0.5f, 0f), 45 / 55f), //5100K: Orange
					new GradientColorKey(Color.red, 55 / 55f) //3700K
				},
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(1f, 1f)
				},
				mode = GradientMode.Blend
			};
			TRAFFIC_LIGHT = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(Color.green, 0f),
					new GradientColorKey(new Color(1f, 1f, 0f), 0.5f), //Yellow
					new GradientColorKey(Color.red, 1f)
				},
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(1f, 1f)
				},
				mode = GradientMode.Blend
			};
			WINTERGATAN_PROJECT_MASTER_PLAN = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(Color.white, 0f),
					new GradientColorKey(Color.cyan, 0.2f),
					new GradientColorKey(Color.green, 0.4f),
					new GradientColorKey(new Color(1f, 1f, 0f), 0.6f),//Yellow
					new GradientColorKey(Color.red, 0.8f),
					new GradientColorKey(Color.magenta, 1f)
				},
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(1f, 1f)
				},
				mode = GradientMode.Blend
			};
			ROYAL_HOURGLASS = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(0f, 0.5f, 0f), 0f),//Darkgreen
					new GradientColorKey(new Color(1f, 0.5f, 0.5f), 0.2f),//Hotpink
					new GradientColorKey(new Color(0.5f, 0f, 0f), 0.4f),//Darkred
					new GradientColorKey(Color.blue, 0.6f),
					new GradientColorKey(new Color(0.93f, 0.51f, 0.93f), 0.8f),//Violett
					new GradientColorKey(new Color(0.5f, 0f, 0.5f), 1f)//Purple
				},
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f),
					new GradientAlphaKey(1f, 1f)
				},
				mode = GradientMode.Blend
			};
		}
	}
}