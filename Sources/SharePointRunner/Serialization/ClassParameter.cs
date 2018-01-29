using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SharePointRunner
{
    /// <summary>
    /// Parameter passed to a class
    /// </summary>
    [XmlType("Parameter")]
    public class ClassParameter
    {
        /// <summary>
        /// Name of the parameter
        /// </summary>
        [XmlAttribute("Name")]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Value of the parameter
        /// </summary>
        [XmlText]
        [JsonProperty("value")]
        public string Value { get; set; } = string.Empty;
    }
}
