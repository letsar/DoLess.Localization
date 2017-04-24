using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DoLess.Localization.ResourceFileHandlers
{
    public sealed class AndroidResourceFileHandler : ResourceFileHandler
    {
        private const string ElementName = "string";
        private const string KeyAttributeName = "name";
        private const string NeutralFolderName = "values";
        private const string FolderNameFormat = "values-{0}";

        public AndroidResourceFileHandler(string projectPath, string language, bool overwrite, ILogger logger) : base(projectPath, language, overwrite, logger)
        {
        }

        protected override string GetContent(IEnumerable<KeyValuePair<string, string>> resources)
        {
            XDocument xDoc = new XDocument();
            XElement xResources = new XElement("resources");
            xDoc.Add(xResources);

            foreach (var resource in resources)
            {
                xResources.Add(new XElement(ElementName, new XAttribute(KeyAttributeName, resource.Key), Encode(resource.Value)));
            }

            // The ToString method on XDocument does not keep the declaration.
            string result;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            using (MemoryStream ms = new MemoryStream())
            using (StreamReader sr = new StreamReader(ms))
            using (XmlWriter writer = XmlWriter.Create(ms, settings))
            {
                xDoc.Save(writer);
                writer.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                result = sr.ReadToEnd();
            }
            return result;
        }

        protected override string GetFilePath()
        {
            var folderName = this.language == null ? NeutralFolderName : string.Format(FolderNameFormat, this.language);
            return Path.Combine(this.projectPath, "Resources", folderName, "Strings.xml");
        }

        protected override void ReadContent(string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                XDocument xDoc = XDocument.Parse(content);
                var items = xDoc.Descendants(ElementName);
                foreach (var item in items)
                {
                    var key = item.Attribute(KeyAttributeName).Value;
                    var value = item.Value;
                    this.Add(key, Decode(value));
                }
            }
        }

        protected override string Decode(string text)
        {
            return text.Replace("\\'", "'");
        }

        protected override string Encode(string text)
        {            
            return text.Replace("'", "\\'");
        }
    }
}
