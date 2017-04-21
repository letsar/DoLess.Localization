using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoLess.Localization.Core.Configuration
{
    public class ConfigurationData
    {
        [JsonProperty("default-localization-file-path-for-android")]
        public string DefaultLocalizationFilePathForAndroid { get; set; }

        [JsonProperty("default-localization-file-path-for-ios")]
        public string DefaultLocalizationFilePathForiOS { get; set; }

        [JsonProperty("resx-folder-paths")]
        public string[] ResxFolderPaths { get; set; }

        [JsonProperty("overwrite")]
        public bool Overwrite { get; set; }
    }
}
