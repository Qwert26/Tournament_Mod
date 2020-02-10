using BrilliantSkies.Core.CSharp;
using BrilliantSkies.Core.FilesAndFolders;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
namespace Tournament.Serialisation
{
	internal class ParametersFile : BaseFile
	{
		public ParametersFile(IFileSource source) : base(source)
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
				Debug.Log("The given File doesn't exist! Path is " + _fileSource.FilePath);
			}
		}
		public void Save(Parameters parameters) {
			_fileSource.SaveData(parameters, Formatting.Indented);
		}
		public Parameters Load() {
			return _fileSource.LoadData<Parameters>();
		}
	}
}
