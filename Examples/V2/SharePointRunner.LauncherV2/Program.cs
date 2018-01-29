namespace SharePointRunner.LauncherV2
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlConfigFilePath = "ConfigFiles/ConfigFile.xml";
            string jsonConfigFilePath = "ConfigFiles/ConfigFile.json";

            //RunningManager runmanager = SharePointRunner.Run(xmlConfigFilePath);
            RunningManager runmanager = SharePointRunner.Run(jsonConfigFilePath);
        }
    }
}
