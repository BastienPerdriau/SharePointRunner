using log4net;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using SharePointRunner.SDK;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using IO = System.IO;

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

            // TODO Adding XML validation schema and validate before deserialization
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

            // TODO Adding JSON validation schema and validate before deserialization
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
        /// <param name="credentials">SharePoint Online credentials</param>
        /// <returns>Running manager</returns>
        private static RunningManager GetRunningManagerFromConfigFile(ConfigFileInfo configFileInfo, SharePointOnlineCredentials credentials = null)
        {
            RunningManager runningManager = new RunningManager();
            string executablePath = Directory.GetCurrentDirectory();

            // Get DLLs classes from assemblies
            foreach (ReceiverAssembly receiverAssembly in configFileInfo.Receivers)
            {
                // Check if the dll file exists
                if (!IO.File.Exists($"{receiverAssembly.AssemblyName}.dll"))
                {
                    Exception ex = new Exception($"The '{receiverAssembly.AssemblyName}.dll' file does not exist");
                    Logger.Warn(ex.Message, ex);
                    continue;
                }

                // Load the assembly
                Assembly assembly = Assembly.LoadFile(Path.Combine(executablePath, $"{receiverAssembly.AssemblyName}.dll"));

                // Check if the assembly is null
                if (assembly == null)
                {
                    Exception ex = new Exception($"The '{receiverAssembly.AssemblyName}' assembly is not loaded");
                    Logger.Warn(ex.Message, ex);
                    continue;
                }

                // Get the type of the class
                Type receiverClass = assembly.GetType($"{receiverAssembly.AssemblyName}.{receiverAssembly.ClassName}");

                // Check if the type exists
                if (receiverClass == null)
                {
                    Exception ex = new Exception($"The '{receiverAssembly.ClassName}' type does not exist");
                    Logger.Warn(ex.Message, ex);
                    continue;
                }

                // Instantiate the class
                var receiver = (Receiver)Activator.CreateInstance(receiverClass);

                // Check if the class is instantiated 
                if (receiver == null)
                {
                    Exception ex = new Exception($"The '{receiverAssembly.ClassName}' class is not instantiated");
                    Logger.Warn(ex.Message, ex);
                    continue;
                }

                // Set properties
                receiver.IncludeSubSites = receiverAssembly.IncludeSubSites;
                receiver.IncludeHiddenLists = receiverAssembly.IncludeHiddenLists;

                // TODO Pass parameters


                // Add receiver to receivers list of the running manager
                runningManager.Receivers.Add(receiver);
            }

            // Create the SharePoint Online credentials if none is passed to parameters
            if (credentials == null)
            {
                if (!string.IsNullOrWhiteSpace(configFileInfo.Login) && configFileInfo.SecuredPassword != null)
                {
                    credentials = new SharePointOnlineCredentials(configFileInfo.Login, configFileInfo.SecuredPassword);
                }
                else
                {
                    Exception ex = new Exception("No credentials provided. Please provide SharePoint Online credentials in the configuration file or calling the Run() method");
                    Logger.Error(ex.Message, ex);
                    throw ex;
                }
            }

            // Set credentials
            runningManager.Credentials = credentials;

            // Get the URLs
            runningManager.Urls.AddRange(configFileInfo.Urls);

            // Get the StartingRunningLevel from its string name
            runningManager.StartingRunningLevel = configFileInfo.StartRunningLevel;

            // Return the running manager
            return runningManager;
        }

        /// <summary>
        /// Start a run using the information of the configuration file
        /// </summary>
        /// <param name="configFilePath">Path of the JSON configuration file</param>
        /// <param name="credentials">SharePoint Online credentials</param>
        /// <returns>Running manager used</returns>
        public static RunningManager Run(string configFilePath, SharePointOnlineCredentials credentials = null)
        {
            // Check file exists
            if (!IO.File.Exists(configFilePath))
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
            RunningManager runningManager = GetRunningManagerFromConfigFile(configFileInfo, credentials);

            // Start the process
            runningManager?.Run();

            // Return running manager
            return runningManager;
        }
    }
}
