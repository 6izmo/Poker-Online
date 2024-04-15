using Settings;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Utilities
{
	public class SettingsSaveLoadUtils
	{
		private const string _settingsFileName = "settingsData.json";

		public static SettingsModel LoadSettingsData()
		{
			string path = Path.Combine(Application.streamingAssetsPath, _settingsFileName);

			if (!File.Exists(path))
				return null;

			string serializedData = File.ReadAllText(path);

			if (string.IsNullOrEmpty(serializedData))
				return null;

			SettingsModel data = JsonConvert.DeserializeObject<SettingsModel>(serializedData);

			if (data == null)
				return new SettingsModel();

			return data;
		}

		public static void SaveSettingsData(SettingsModel dataModel)
		{
			if (dataModel == null)
				return;

			string serializedObject = JsonConvert.SerializeObject(dataModel, SettingsModel.SerializeSettings());
			string path = Path.Combine(Application.streamingAssetsPath, _settingsFileName);

			CreateDirectoryIfNoteExists(Application.streamingAssetsPath);

			File.WriteAllText(path, serializedObject);
		}

		public static void CreateDirectoryIfNoteExists(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}
	}
}
