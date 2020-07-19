using BrilliantSkies.Core.FilesAndFolders;
namespace TournamentMod.Serialisation
{
	internal class TCCFolder : BaseFolder<TCCFile>
	{
		public TCCFolder(IFolderSource source, bool forceReadOnly = false) : base(source, forceReadOnly) { }
		protected override string FileExtension => ".tournament.teamsettings";
		protected override BaseFolder<TCCFile> MakeAnotherOfUs(IFolderSource folder)
		{
			return new TCCFolder(folder, IsReadOnly);
		}
		protected override TCCFile MakeFile(IFileSource path)
		{
			return new TCCFile(path);
		}
	}
}