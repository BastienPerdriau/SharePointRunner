using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml.Serialization;

namespace SharePointRunner
{
    /// <summary>
    /// Launcher class
    /// </summary>
    public static class SharePointRunner
    {
        /// <summary>
        /// Logger
        /// </summary>
        internal static readonly ILog Logger = LogManager.GetLogger(typeof(SharePointRunner).Namespace);

        /// <summary>
        /// Get the configurationinfo parsing the XML config file
        /// </summary>
        /// <param name="configFilePath">Path of the XML configuration file</param>
        /// <returns>Configuration informations from the file</returns>
        private static ConfigFileInfo GetConfigFileInfoFromXml(string configFilePath)
        {
            ConfigFileInfo configFileInfo = default;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigFileInfo));
                using (var reader = new StreamReader(configFilePath))
                {
                    configFileInfo = (ConfigFileInfo)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Exception ex = new Exception("Error during XML configuration file parsing", e);
                Logger.Error(ex.Message, ex);
                throw ex;
            }

            return configFileInfo;
        }

        /// <summary>
        /// Get the configurationinfo parsing the JSON config file
        /// </summary>
        /// <param name="configFilePath">Path of the JSON configuration file</param>
        /// <returns>Configuration informations from the file</returns>
        private static ConfigFileInfo GetConfigFileInfoFromJson(string configFilePath)
        {
            ConfigFileInfo configFileInfo = default;

            try
            {
                JsonSerializer serializer = new JsonSerializer();
                using (var reader = new StreamReader(configFilePath))
                {
                    configFileInfo = (ConfigFileInfo)serializer.Deserialize(reader, typeof(ConfigFileInfo));
                }
            }
            catch (Exception e)
            {
                Exception ex = new Exception("Error during JSON configuration file parsing", e);
                Logger.Error(ex.Message, ex);
                throw ex;
            }

            return configFileInfo;
        }

        /// <summary>
        /// Get the running manager from the configuration file info
        /// </summary>
        /// <param name="configFileInfo">Configuration information from the file</param>
        /// <returns>Running manager</returns>
        private static RunningManager GetRunningManagerFromConfigFile(ConfigFileInfo configFileInfo)
        {
            // TODO Get DLLs classes from assemblies

            // TODO Create the SharePoint Online credentials or prompt the user if they are not in the config file

            // TODO Get the URLs

            // TODO Get the StartingRunningLevel from its string name

            // Return the running manager
            return new RunningManager();
        }

        /// <summary>
        /// Start a run using the information of the configuration file
        /// </summary>
        /// <param name="configFilePath">Path of the JSON configuration file</param>
        /// <returns>Running manager used</returns>
        // TODO Add credentials optionnal parameter, overriding file credentials if there is
        public static RunningManager Run(string configFilePath)
        {
            // Check file exists
            if (!File.Exists(configFilePath))
            {
                Exception ex = new Exception("File does not exist");
                Logger.Error(ex.Message, ex);
                throw ex;
            }

            // Get the extension of the file
            string extension = Path.GetExtension(configFilePath);

            ConfigFileInfo configFileInfo = null;
            switch (extension.ToLowerInvariant())
            {
                case ".xml":
                    // If XML, parse XML
                    configFileInfo = GetConfigFileInfoFromXml(configFilePath);
                    break;
                case ".json":
                    // If JSON, parse JSON
                    configFileInfo = GetConfigFileInfoFromJson(configFilePath);
                    break;
                default:
                    Exception ex = new Exception("Extension file not valid");
                    Logger.Error(ex.Message, ex);
                    throw ex;
            }

            // Instanciate and feed running manager
            RunningManager runningManager = GetRunningManagerFromConfigFile(configFileInfo);

            // Start the process
            runningManager?.Run();

            // Return running manager
            return runningManager;
        }
    }
}
