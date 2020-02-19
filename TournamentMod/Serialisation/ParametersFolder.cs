using BrilliantSkies.Core.FilesAndFolders;
namespace TournamentMod.Serialisation
{
	internal class ParametersFolder : BaseFolder<ParametersFile>
	{
		public ParametersFolder(IFolderSource source, bool forceReadOnly = false) : base(source, forceReadOnly) {}
		/// <summary>
		/// Für erste .json, bis mir etwas besseres einfällt.
		/// </summary>
		protected override string FileExtension => ".json";
		protected override BaseFolder<ParametersFile> MakeAnotherOfUs(IFolderSource folder)
		{
			return new ParametersFolder(folder);
		}
		protected override ParametersFile MakeFile(IFileSource path)
		{
			return new ParametersFile(path);
		}
	}
}
