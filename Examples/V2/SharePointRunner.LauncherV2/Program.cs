namespace SharePointRunner.LauncherV2
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlConfigFilePath = "ConfigFiles/ConfigFile.xml";
            string jsonConfigFilePath = "ConfigFiles/ConfigFile.json";

            SharePointRunner.Run(xmlConfigFilePath);
            //SharePointRunner.Run(jsonConfigFilePath);
        }
    }
}
