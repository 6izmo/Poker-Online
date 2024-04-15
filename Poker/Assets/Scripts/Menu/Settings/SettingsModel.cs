namespace Settings
{
    [System.Serializable]
    public class SettingsModel
    {
		public float Music { get; set; }

		public float Effects { get; set; }

		public static Newtonsoft.Json.JsonSerializerSettings SerializeSettings()
		{
			return new Newtonsoft.Json.JsonSerializerSettings
			{
				ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			};
		}
	}
}
