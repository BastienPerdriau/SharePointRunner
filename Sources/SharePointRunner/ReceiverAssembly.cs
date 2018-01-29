using Newtonsoft.Json;
using System.Collections.Generic;
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
        /// List of the parameters passed to the receiver
        /// </summary>
        [XmlElement("Parameter")]
        [JsonProperty("parameters")]
        public List<ClassParameter> Parameters { get; } = new List<ClassParameter>();
    }
}