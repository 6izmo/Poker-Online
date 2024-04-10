namespace Settings
{
    [System.Serializable]
    public class SettingsModel
    {
        public float Music { get; private set; }

		public float Effects { get; private set; }

        public SettingsModel(float music, float effects)
        {
            Music = music;
			Effects = effects;
		}

		public static Newtonsoft.Json.JsonSerializerSettings SerializeSettings()
		{
			return new Newtonsoft.Json.JsonSerializerSettings
			{
				ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			};
		}
	}
}
