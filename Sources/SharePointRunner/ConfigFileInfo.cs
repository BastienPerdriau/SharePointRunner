using Newtonsoft.Json;
using SharePointRunner.SDK;
using System;
using System.Collections.Generic;
using System.Security;
using System.Xml.Serialization;

namespace SharePointRunner
{
    /// <summary>
    /// Information from the configuration file
    /// </summary>
    [XmlRoot("Configuration")]
    [JsonObject(MemberSerialization.OptIn)]
    public class ConfigFileInfo
    {
        /// <summary>
        /// Login of the user
        /// </summary>
        [XmlElement("Login")]
        [JsonProperty("login")]
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Password of the user
        /// </summary>
        [XmlElement("Password")]
        [JsonProperty("password")]
        public string Password { get; set; } = null;

        /// <summary>
        /// Scured password of the user
        /// </summary>
        [XmlIgnore]
        public SecureString SecuredPassword
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    SecureString secString = new SecureString();

                    foreach (char c in Password)
                    {
                        secString.AppendChar(c);
                    }

                    return secString;
                }

                return null;
            }
        }

        // TODO Map Receiver parameters
        /// <summary>
        /// List of the receivers assemblies
        /// </summary>
        [XmlArray("Receivers")]
        [XmlArrayItem("Receiver")]
        [JsonProperty("receivers")]
        public List<ReceiverAssembly> Receivers { get; } = new List<ReceiverAssembly>();

        /// <summary>
        /// List of URLs
        /// </summary>
        [XmlArray("Urls")]
        [XmlArrayItem("Url")]
        [JsonProperty("urls")]
        public List<string> Urls { get; } = new List<string>();

        /// <summary>
        /// Starting running level string value
        /// </summary>
        [XmlElement("StartRunningLevel")]
        [JsonProperty("startRunningLevel")]
        public string StartRunningLevelString { get; set; } = string.Empty;

        /// <summary>
        /// Starting running level
        /// </summary>
        [XmlIgnore]
        public RunningLevel StartRunningLevel
        {
            get
            {
                if (Enum.TryParse(StartRunningLevelString, out BaseRunningLevel enumParsed) && RunningLevel.Values.ContainsKey(enumParsed))
                {
                    return RunningLevel.Values[enumParsed];
                }

                return null;
            }
        }
    }
}
