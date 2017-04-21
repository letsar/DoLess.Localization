using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DoLess.Localization.Configuration;
using DoLess.Localization.ResourceFileHandlers;
using Newtonsoft.Json;

namespace DoLess.Localization
{
    internal class ResxConverter
    {
        private const string ConfigurationFileName = "doless.localization.json";
        private const string NeutralLanguageName = "neutral";

        private readonly ILogger logger;
        private readonly string projectDirectory;

        public ResxConverter(string projectDirectory, ILogger logger)
        {
            this.projectDirectory = projectDirectory;
            this.logger = logger;
        }

        public static string GetLanguageOfResx(string resxFilePath)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(resxFilePath);
            var parts = fileNameWithoutExtension.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts?.Length > 1)
            {
                return parts.Last().ToLowerInvariant();
            }
            else
            {
                // neutral language.
                return null;
            }
        }

        public bool Execute()
        {
            this.PrintVersion();

            string configurationFilePath = this.FindConfigurationFile();
            if (configurationFilePath != null)
            {
                ConfigurationData config = this.ReadConfigurationFile(configurationFilePath);
                if (this.IsConfigurationValid(config))
                {
                    this.MakePlatformProjectFolderFullPaths(config);
                    if (!this.IsFolderExists(config.FolderPathOfAndroidProject))
                    {
                        config.FolderPathOfAndroidProject = null;
                    }

                    if (!this.IsFolderExists(config.FolderPathOfiOSProject))
                    {
                        config.FolderPathOfiOSProject = null;
                    }

                    Dictionary<string, SortedDictionary<string, string>> languageToResources = new Dictionary<string, SortedDictionary<string, string>>();

                    var resxFilePaths = this.FindResxFiles();
                    foreach (var resxFilePath in resxFilePaths)
                    {
                        this.ReadResxFile(resxFilePath, languageToResources);
                    }

                    if (config.FolderPathOfAndroidProject != null)
                    {
                        this.WritePlatformProject(config, languageToResources, this.GetAndroidResourceFileHandler);
                    }

                    if (config.FolderPathOfiOSProject != null)
                    {
                        this.WritePlatformProject(config, languageToResources, this.GetiOSResourceFileHandler);
                    }

                    return true;
                }
            }

            return false;
        }

        public IEnumerable<string> FindResxFiles()
        {
            return Directory.EnumerateFiles(this.projectDirectory, "*.resx", SearchOption.AllDirectories);
        }

        private string FindConfigurationFile()
        {
            var projectConfigFilePath = Path.Combine(this.projectDirectory, ConfigurationFileName);
            if (File.Exists(projectConfigFilePath))
            {
                this.logger.LogDebug($"Found path to configuration file: '{projectConfigFilePath}'.");
                return projectConfigFilePath;
            }

            this.logger.LogError($"Could not find path to configuration file '{ConfigurationFileName}'.");
            return null;
        }

        private ResourceFileHandler GetAndroidResourceFileHandler(ConfigurationData config, string language)
        {
            return new AndroidResourceFileHandler(config.FolderPathOfAndroidProject, language, config.Overwrite, this.logger);
        }

        private string GetFullPath(string relativePathToProjectFolder)
        {
            if (string.IsNullOrWhiteSpace(relativePathToProjectFolder))
            {
                return null;
            }

            return Path.GetFullPath(Path.Combine(this.projectDirectory, relativePathToProjectFolder));
        }

        private ResourceFileHandler GetiOSResourceFileHandler(ConfigurationData config, string language)
        {
            return new iOSResourceFileHandler(config.FolderPathOfiOSProject, language, config.Overwrite, this.logger);
        }

        private bool IsConfigurationValid(ConfigurationData config)
        {
            if (config == null)
            {
                this.logger.LogError("The configuration file is null");
                return false;
            }

            if (string.IsNullOrWhiteSpace(config.FolderPathOfAndroidProject) && string.IsNullOrWhiteSpace(config.FolderPathOfiOSProject))
            {
                this.logger.LogError("One platform project must be set");
                return false;
            }

            return true;
        }

        private bool IsFolderExists(string folderPath)
        {
            if (folderPath != null && !Directory.Exists(folderPath))
            {
                this.logger.LogWarning($"The folder '{folderPath}' does not exists.");
                return false;
            }
            return true;
        }

        private void MakePlatformProjectFolderFullPaths(ConfigurationData config)
        {
            config.FolderPathOfAndroidProject = this.GetFullPath(config.FolderPathOfAndroidProject);
            config.FolderPathOfiOSProject = this.GetFullPath(config.FolderPathOfiOSProject);
        }

        private void PrintVersion()
        {
            this.logger.LogInfo($"DoLess.Localization (version: {typeof(ResxConverter).Assembly.GetName().Version}) executing.");
        }

        private ConfigurationData ReadConfigurationFile(string configurationFilePath)
        {
            try
            {
                string configurationFileContent = File.ReadAllText(configurationFilePath);
                ConfigurationData configurationData = JsonConvert.DeserializeObject<ConfigurationData>(configurationFileContent);
                return configurationData;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex);
                return null;
            }
        }

        private void ReadResxFile(string resxFilePath, Dictionary<string, SortedDictionary<string, string>> languageToResources)
        {
            this.logger.LogInfo($"Processing resx '{resxFilePath}'.");

            var language = GetLanguageOfResx(resxFilePath) ?? NeutralLanguageName;
            this.logger.LogInfo($"The language '{language}' is being used.");

            SortedDictionary<string, string> resources = null;
            if (!languageToResources.TryGetValue(language, out resources))
            {
                resources = new SortedDictionary<string, string>();
                languageToResources[language] = resources;
            }

            XDocument xDoc = XDocument.Load(resxFilePath);
            var data = xDoc.Descendants("data");
            foreach (var item in data)
            {
                var key = item.Attribute("name").Value;
                var value = item.Element("value").Value;
                if (resources.ContainsKey(key))
                {
                    this.logger.LogWarning($"The key: '{key}' from '{resxFilePath}' has already been found in a previous resx file. Skipping this one.");
                }
                else
                {
                    resources[key] = value;
                }
            }
        }

        private void WritePlatformProject(ConfigurationData config, Dictionary<string, SortedDictionary<string, string>> languageToResources, Func<ConfigurationData, string, ResourceFileHandler> getResourceFileHandler)
        {
            foreach (var item in languageToResources)
            {
                var language = item.Key == NeutralLanguageName ? null : item.Key;
                var resourceFileHandler = getResourceFileHandler(config, language);
                resourceFileHandler.Write(item.Value);
            }
        }
    }
}
