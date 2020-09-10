using BrilliantSkies.Core.Logger;
using BrilliantSkies.Core.CSharp;
using BrilliantSkies.Core.FilesAndFolders;
using Newtonsoft.Json;
using System.Linq;
namespace TournamentMod.Serialisation
{
	internal class TCCFile : BaseFile
	{
		public TCCFile(IFileSource source) : base(source)
		{
			if (_fileSource.Exists)
			{
				string[] array = _fileSource.FileName.Split('.').ToArray();
				if (array.Length != 1)
				{
					array = array.NotLast().ToArray();
				}
				Name = string.Join(".", array);
			}
			else
			{
				AdvLogger.LogError("The given File doesn't exist! Path is " + _fileSource.FilePath, LogOptions.Popup);
			}
		}
		public void Save(TeamCompositionConfiguration tcc)
		{
			_fileSource.SaveData(tcc, Formatting.Indented);
		}
		public TeamCompositionConfiguration Load()
		{
			return _fileSource.LoadData<TeamCompositionConfiguration>();
		}
	}
}
