using Assets.Scripts.Persistence;
using BrilliantSkies.Core.FilesAndFolders;
namespace Tournament.Serialisation
{
    internal class TournamentParametersFolder : BaseFolder<TournamentParametersFile>
    {
        public TournamentParametersFolder(IFolderSource source, bool forceReadOnly = false) : base(source, forceReadOnly) {}

        /// <summary>
        /// Für erste .json, bis mir etwas besseres einfällt.
        /// </summary>
        protected override string FileExtension => ".json";

        protected override BaseFolder<TournamentParametersFile> MakeAnotherOfUs(IFolderSource folder)
        {
            return new TournamentParametersFolder(folder);
        }

        protected override TournamentParametersFile MakeFile(IFileSource path)
        {
            return new TournamentParametersFile(path);
        }
    }
}
