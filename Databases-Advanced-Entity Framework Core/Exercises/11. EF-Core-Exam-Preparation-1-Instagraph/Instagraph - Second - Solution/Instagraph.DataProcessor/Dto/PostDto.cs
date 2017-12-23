using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor.Dto
{
    [XmlType("post")]
    public class PostDto
    {
        [XmlElement("caption")]
        public string Caption { get; set; }

        [XmlElement("user")]
        public string User { get; set; }

        [XmlElement("picture")]
        public string Picture { get; set; }
    }
}
