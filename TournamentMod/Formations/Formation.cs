﻿using UnityEngine;
namespace TournamentMod.Formations
{
	/// <summary>
	/// Contains all required information to build a formation.
	/// </summary>
	public struct Formation
	{
		/// <summary>
		/// A function which uses the provided information to determine a position, rotated appropiated for a team.
		/// </summary>
		/// <param name="factionRotation">The final rotation for a team to apply</param>
		/// <param name="gapLeftRight">The distance along the local x-axis</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis</param>
		/// <param name="count">the amount of entries inside this formation</param>
		/// <param name="index">the current entry</param>
		/// <param name="distance">The distance ffrom the center</param>
		/// <param name="height">The set height of the entry</param>
		/// <returns>The Position of the entry of a team in respect to the center of the battlefield.</returns>
		public delegate Vector3 LocalPosition(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height);
		/// <summary>
		/// A function which uses the provided information to descripe a position inside a formation.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis</param>
		/// <param name="count">the amount of entries inside this formation</param>
		/// <param name="index">the current entry</param>
		/// <returns>The description of the current entry inside a particular formation.</returns>
		public delegate string PositionDescription(float gapLeftRight, float gapForwardBackward, int count, int index);
		/// <summary>
		/// A function which uses the provided information to calculate the size of the bounding box
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis</param>
		/// <param name="count">The amount of entries inside this formation</param>
		/// <returns>The bounding-box of this formation, used for combining formations.</returns>
		public delegate Vector2 Size(float gapLeftRight, float gapForwardBackward, int count);
		/// <summary>
		/// A function which uses the provided information to give the index of the flagship the indexed construct should go to.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis. Most Formations will ignore this.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis. Most Formations will ignore this.</param>
		/// <param name="count">The amount of entries inside this formation</param>
		/// <param name="index">the current entry</param>
		/// <returns>-1 if a flagship does not exist or the index of the flagship.</returns>
		public delegate int Flagship(float gapLeftRight, float gapForwardBackward, int count, int index);
		/// <summary>
		/// The Name of the Formation
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The Description of the Formation
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// If a fleet can be formed by this Formation.
		/// </summary>
		public bool SupportsFleetForming { get; set; }
		/// <summary>
		/// Executes the function for a local position
		/// </summary>
		public LocalPosition DetermineLocalPosition { get; set; }
		/// <summary>
		/// Executes the function for a description of a position
		/// </summary>
		public PositionDescription DeterminePositionDescription { get; set; }
		/// <summary>
		/// Executes the function for a size-estimate of the formation.
		/// </summary>
		public Size DetermineSize { get; set; }
		/// <summary>
		/// Executes the function to get the Flagship of an entry.
		/// </summary>
		public Flagship DetermineFlagship { get; set; }
		/// <summary>
		/// The Formation for unknown Types.
		/// </summary>
		public static readonly Formation UnknownFormation = new Formation()
		{
			Name = "Unknown",
			Description = "You have selected a Type, which has not been implemented yet: Don't expect it to work!",
			SupportsFleetForming = true,
			DetermineLocalPosition = FormationCalculation.UnkownFormation,
			DeterminePositionDescription = FormationPositionDescription.UnknownFormation,
			DetermineSize = FormationSizeCalculation.UnkownFormation,
			DetermineFlagship = FormationFlagship.UnknownFormation
		};
		/// <summary>
		/// The classic "Line"-Formation
		/// </summary>
		public static readonly Formation Line = new Formation()
		{
			Name = "Line",
			Description = "The classic Line Formation by wo0tness and Anickle. The Flagship should be in the middle position.",
			SupportsFleetForming = true,
			DetermineLocalPosition = FormationCalculation.LineFormation,
			DeterminePositionDescription = FormationPositionDescription.LineFormation,
			DetermineSize = FormationSizeCalculation.LineFormation,
			DetermineFlagship = FormationFlagship.LineFormation
		};
		/// <summary>
		/// A "Funnel"- or "Arrow"-Formation
		/// </summary>
		public static readonly Formation Wedge = new Formation()
		{
			Name = "Wedge",
			Description = "Also known as a V-Formation. A positive Forward-Backward gap will result in an Arrow-Style, a negative Forward-Backward gap in a Funnel-style Formation. " +
							"The Flagship should be in first position as it will form the pivot point.",
			SupportsFleetForming = true,
			DetermineLocalPosition = FormationCalculation.WedgeFormation,
			DeterminePositionDescription = FormationPositionDescription.WedgeFormation,
			DetermineSize = FormationSizeCalculation.WedgeFormation,
			DetermineFlagship = FormationFlagship.WedgeTriangleFormation
		};
		/// <summary>
		/// A "Funnel"- or "Arrow"-Formation consisting our of three groups
		/// </summary>
		public static readonly Formation DividedWedge = new Formation()
		{
			Name = "Divided Wedge",
			Description = "This variant of the V-Formation divides the fleet in thee groups. The Flagship should be in first position as it will form the pivot point. " +
			"Commandships are the second and third positions. The distance between the groups grows, the larger the fleet is.",
			SupportsFleetForming = true,
			DetermineLocalPosition = FormationCalculation.DividedWedgeFormation,
			DeterminePositionDescription = FormationPositionDescription.DividedWedgeFormation,
			DetermineSize = FormationSizeCalculation.DividedWedgeFormation,
			DetermineFlagship = FormationFlagship.DividedWedgeFormation
		};
		/// <summary>
		/// a rectangular formation
		/// </summary>
		public static readonly Formation ParallelColumns = new Formation()
		{
			Name = "Columns",
			Description = "The fleet is in a rectangular formation, where the start of one column is at an 22° angle to the end of a neighboring column. " +
			"The Columns are getting evenly filled front to back and the maximum amount of ships in a column gets determined by the ratio of the two gap values.",
			SupportsFleetForming = false,
			DetermineLocalPosition = FormationCalculation.ParallelColumnsFormation,
			DeterminePositionDescription = FormationPositionDescription.ParallelColumnsFormation,
			DetermineSize = FormationSizeCalculation.ParallelColumnsFormation,
			DetermineFlagship = FormationFlagship.UnknownFormation
		};
		/// <summary>
		/// a rectangular formation where each distinct pair of columns has an additional ship in front of them.
		/// </summary>
		public static readonly Formation CommandedParallelColumns = new Formation()
		{
			Name = "Commanded Columns",
			Description = "Similar to the Columns-Formation, but each pair of Columns also has an Commandship diagonally in front of it.",
			SupportsFleetForming = true,
			DetermineLocalPosition = FormationCalculation.CommandedParallelColumnsFormation,
			DeterminePositionDescription = FormationPositionDescription.CommandedParallelColumnsFormation,
			DetermineSize = FormationSizeCalculation.CommandedParallelColumnsFormation,
			DetermineFlagship = FormationFlagship.CommandedParallelColumnsFormation
		};
		/// <summary>
		/// The Roman Manipel Formation when marching.
		/// </summary>
		public static readonly Formation RomanManipelBase = new Formation()
		{
			Name = "Roman Legion, Manipel Base",
			Description = "The Baseformation of the Manipel after Marcus Furius Camillus. Each Group consists of only six Entries, put together in vertical pairs.",
			SupportsFleetForming = false,
			DetermineLocalPosition = FormationCalculation.ManipelBaseFormation,
			DeterminePositionDescription = FormationPositionDescription.ManipelBaseFormation,
			DetermineSize = FormationSizeCalculation.ManipelFormation,
			DetermineFlagship = FormationFlagship.UnknownFormation
		};
		/// <summary>
		/// The Roman Manipel Formation when attacking.
		/// </summary>
		public static readonly Formation RomanManipelAttack = new Formation()
		{
			Name = "Roman Legion, Manipel Attack",
			Description = "Similar to the Baseformation, but the second line of the Baseformation moved into the gaps of the first line, forming a solid line at the front.",
			SupportsFleetForming = false,
			DetermineLocalPosition = FormationCalculation.ManipelAttackFormation,
			DeterminePositionDescription = FormationPositionDescription.ManipelAttackFormation,
			DetermineSize = FormationSizeCalculation.ManipelFormation,
			DetermineFlagship = FormationFlagship.UnknownFormation
		};
		/// <summary>
		/// A straight line.
		/// </summary>
		public static readonly Formation GuardLine = new Formation()
		{
			Name = "Guard Line",
			Description = "Similar to the Line-Formation, but its intent is to be part of a greater \"Super-Formation\", as it ignores the Forward-Backward-Gap.",
			SupportsFleetForming = true,
			DetermineLocalPosition = FormationCalculation.GuardLineFormation,
			DeterminePositionDescription = FormationPositionDescription.GuardLineFormation,
			DetermineSize = FormationSizeCalculation.GuardLineFormation,
			DetermineFlagship = FormationFlagship.LineFormation
		};
		/// <summary>
		/// A triangle.
		/// </summary>
		public static readonly Formation Triangle = new Formation()
		{
			Name = "Triangle",
			Description = "A conceptional Mix between a Wedge- and a Line-Formation. The Flagship should be in the first position, as it will form one of the points. " +
			"The Formation was mentioned by Hibachi on the official FtD-Discord.",
			SupportsFleetForming = true,
			DetermineLocalPosition = FormationCalculation.Triangle,
			DeterminePositionDescription = FormationPositionDescription.Triangle,
			DetermineSize = FormationSizeCalculation.Triangle,
			DetermineFlagship = FormationFlagship.WedgeTriangleFormation
		};
		/// <summary>
		/// An inverted triangle.
		/// </summary>
		public static readonly Formation InvertedTriangle = new Formation()
		{
			Name = "Inverted Triangle",
			Description = "Just like the normal Triangle-Formation, but the Flagship will be at the back of the Formation and not on the Front.",
			SupportsFleetForming = true,
			DetermineSize = FormationSizeCalculation.Triangle,
			DeterminePositionDescription = FormationPositionDescription.Triangle,
			DetermineLocalPosition = FormationCalculation.InvertedTriangle,
			DetermineFlagship = FormationFlagship.WedgeTriangleFormation
		};
	}
}