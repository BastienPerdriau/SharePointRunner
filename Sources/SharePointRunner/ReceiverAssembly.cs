using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace SharePointRunner
{
    /// <summary>
    /// Representation of a receiver assembly
    /// </summary>
    [XmlType("Receiver")]
    public class ReceiverAssembly
    {
        /// <summary>
        /// Name of the assembly
        /// </summary>
        [XmlAttribute("AssemblyName")]
        [JsonProperty("assemblyName")]
        public string AssemblyName { get; set; } = string.Empty;

        /// <summary>
        /// Name of the class
        /// </summary>
        [XmlAttribute("ClassName")]
        [JsonProperty("className")]
        public string ClassName { get; set; } = string.Empty;

        /// <summary>
        /// String value of the IncludeSubSites property
        /// </summary>
        [XmlAttribute("IncludeSubSites"), Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("includeSubSites")]
        public string IncludeSubSitesString
        {
            get { return IncludeSubSites ? "True" : "False"; }
            set
            {
                switch (value.ToLowerInvariant())
                {
                    case "true": IncludeSubSites = true; break;
                    case "false": IncludeSubSites = false; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// True if the receiver process include sub sites, False if not
        /// </summary>
        [XmlIgnore]
        public bool IncludeSubSites { get; set; } = true;

        /// <summary>
        /// String value of the IncludeHiddenLists property
        /// </summary>
        [XmlAttribute("IncludeHiddenLists"), Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("includeHiddenLists")]
        public string IncludeHiddenListsString
        {
            get { return IncludeHiddenLists ? "True" : "False"; }
            set
            {
                switch (value.ToLowerInvariant())
                {
                    case "true": IncludeHiddenLists = true; break;
                    case "false": IncludeHiddenLists = false; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// True if the receiver process include hidden lists, False if not
        /// </summary>
        [XmlIgnore]
        public bool IncludeHiddenLists { get; set; } = false;

        /// <summary>
        /// List of the parameters passed to the receiver
        /// </summary>
        [XmlElement("Parameter")]
        [JsonProperty("parameters")]
        public List<ClassParameter> Parameters { get; } = new List<ClassParameter>();
    }
}