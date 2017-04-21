using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DoLess.Localization.Core.Configuration;
using Newtonsoft.Json;

namespace DoLess.Localization.Core
{
    public class ResxConverter
    {
        private static readonly Regex ResxFileRegex = new Regex(".{}.resx");
        private readonly ConfigurationData configurationData;


        public ResxConverter(string configurationFilePath) : this(ReadConfigurationFile(configurationFilePath)) { }

        public ResxConverter(ConfigurationData configurationData)
        {
            this.configurationData = configurationData ?? throw new ArgumentNullException("configurationData");
        }

        public bool GenerateLocalization()
        {
            string[] resxFolderPaths = this.configurationData.ResxFolderPaths;
            if (resxFolderPaths?.Length == 0)
            {
                Console.Error.WriteLine("There are no resx folders");
                return false;
            }

            

            return true;
        }

        private static ConfigurationData ReadConfigurationFile(string configurationFilePath)
        {
            var configurationFile = new FileInfo(configurationFilePath);
            if (!configurationFile.Exists)
            {
                throw new FileNotFoundException("The configuration file has not been found", configurationFilePath);
            }

            string configurationFileContent = File.ReadAllText(configurationFile.FullName);
            ConfigurationData configurationData = JsonConvert.DeserializeObject<ConfigurationData>(configurationFileContent);
            return configurationData;
        }
    }
}
