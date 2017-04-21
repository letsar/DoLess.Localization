using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DoLess.Localization.ResourceFileHandlers
{
    public class iOSResourceFileHandler : ResourceFileHandler
    {
        private static readonly char[] PairSeparator = new[] { '=' };
        private readonly string NeutralFolderName = "Base";

        public iOSResourceFileHandler(string projectPath, string language, bool overwrite, ILogger logger) : base(projectPath, language, overwrite, logger)
        {
        }

        protected override string GetContent(IEnumerable<KeyValuePair<string, string>> resources)
        {
            StringBuilder str = new StringBuilder();

            foreach (var resource in resources)
            {
                str.AppendLine($"\"{resource.Key}\"=\"{Encode(resource.Value)}\";");
            }

            return str.ToString();
        }

        protected override string GetFilePath()
        {
            var folderName = this.language ?? NeutralFolderName;
            return Path.Combine(this.projectPath, "Resources", $"{folderName}.lproj", "Localizable.strings");
        }

        protected override void ReadContent(string content)
        {
            string line = null;
            using (TextReader reader = new StringReader(content))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    var resourceParts = line.Split(PairSeparator, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => x.Trim().Trim('"'))
                                            .ToList();
                    if (resourceParts.Count == 2)
                    {
                        this.Add(resourceParts[0], Decode(resourceParts[1]));
                    }
                    else
                    {
                        this.logger.LogWarning($"The line '{line}' could not be parsed in the file {this.filePath}.");
                    }
                }
            }
        }

        protected override string Decode(string text)
        {
            // https://developer.xamarin.com/guides/ios/advanced_topics/localization_and_internationalization/.
            return text.Replace("\\\"", "\"")
                       .Replace("\\\\", "\\")
                       .Replace("\\n", "\n");
        }

        protected override string Encode(string text)
        {
            // https://developer.xamarin.com/guides/ios/advanced_topics/localization_and_internationalization/.
            return text.Replace("\"", "\\\"")
                       .Replace("\\", "\\\\")
                       .Replace("\n", "\\n");
        }
    }
}
