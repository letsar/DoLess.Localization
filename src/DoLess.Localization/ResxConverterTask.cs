using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DoLess.Localization
{
    public class ResxConverterTask : Task
    {
        [Required]
        public string ProjectDirectory { get; set; }

        public override bool Execute()
        {
            ResxConverter resxConverter = new ResxConverter(this.ProjectDirectory, new BuildLogger(this.BuildEngine));

            try
            {
                return resxConverter.Execute();
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
