using System;
using System.Collections.Generic;
using System.IO;

namespace DoLess.Localization.ResourceFileHandlers
{
    public abstract class ResourceFileHandler
    {
        protected readonly string filePath;
        protected readonly string projectPath;
        protected readonly string language;
        protected readonly ILogger logger;
        protected readonly bool overwrite;
        private readonly SortedDictionary<string, string> resources;

        public ResourceFileHandler(string projectPath, string language, bool overwrite, ILogger logger)
        {
            this.projectPath = projectPath;
            this.language = language;
            this.overwrite = overwrite;
            this.logger = logger;
            this.filePath = this.GetFilePath();
            this.resources = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public void Write(SortedDictionary<string, string> newResources)
        {            
            this.CreateFileIfMissing();

            string originalContent = File.ReadAllText(this.filePath);

            if (!this.overwrite)
            {
                this.ReadContent(originalContent);
            }

            this.Add(newResources);

            string content = this.GetContent(this.resources);

            if (!string.Equals(originalContent, content, StringComparison.InvariantCulture))
            {
                File.WriteAllText(this.filePath, content);
            }
            else
            {
                this.logger.LogInfo($"Skipping update of '{this.filePath}' there are no differences.");
            }
        }

        protected void Add(string key, string value)
        {
            if (!this.resources.ContainsKey(key))
            {
                this.resources[key] = value;
            }
            else
            {
                this.logger.LogWarning($"The key: '{key}' has already been added for the file: '{this.filePath}'.");
            }
        }

        protected void CreateFileIfMissing()
        {
            if (!File.Exists(this.filePath))
            {
                string directoryPath = Path.GetDirectoryName(this.filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.Create(this.filePath).Dispose();
                this.logger.LogInfo($"The file '{this.filePath}' has been created.");
            }
        }

        protected abstract string GetContent(IEnumerable<KeyValuePair<string, string>> resources);

        protected abstract string GetFilePath();

        protected abstract void ReadContent(string content);

        private void Add(IEnumerable<KeyValuePair<string, string>> resources)
        {
            foreach (var resource in resources)
            {
                this.Add(resource.Key, resource.Value);
            }
        }

        protected virtual string Decode(string text)
        {
            return text;
        }

        protected virtual string Encode(string text)
        {
            return text;
        }
    }
}
