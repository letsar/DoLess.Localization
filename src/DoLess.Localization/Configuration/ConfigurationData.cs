using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoLess.Localization.Configuration
{
    internal class ConfigurationData
    {
        [JsonProperty("android-project-folder-path")]
        public string FolderPathOfAndroidProject { get; set; }

        [JsonProperty("ios-project-folder-path")]
        public string FolderPathOfiOSProject { get; set; }

        [JsonProperty("overwrite")]
        public bool Overwrite { get; set; }        
    }
}
