using System;
using System.IO;
using System.Xml.Serialization;

namespace subdown
{
    public interface ISubtitleResult
    {
        [XmlIgnore]
        string SubtitleEncoded { get; set; }

        [XmlIgnore]
        string SubtitleDecoded { get; set; }

        string SubtitleFileName { get; set; }
        string SubtitleFormat { get; set; }

        [XmlIgnore]
        string LocalFilename { get; set; }

        FileInfo VideoFileInfo { get; set; }

        string Id { get; set; }
    }
}