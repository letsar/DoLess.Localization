using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace DoLess.Localization
{
    internal class BuildLogger : ILogger
    {
        private const string SenderName = "DoLess.Localization";
        private readonly IBuildEngine buildEngine;

        public BuildLogger(IBuildEngine buildEngine)
        {
            this.buildEngine = buildEngine;
        }

        public void LogMessage(string message, MessageImportance level)
        {
            this.buildEngine.LogMessageEvent(new BuildMessageEventArgs(FormatMessage(message), string.Empty, SenderName, level));
        }

        public void LogDebug(string message)
        {
            this.LogMessage(message, MessageImportance.Normal);
        }

        public void LogInfo(string message)
        {
            this.LogMessage(message, MessageImportance.High);
        }

        public void LogWarning(string message, [CallerFilePath] string file = null, [CallerLineNumber] int lineNumber = 0)
        {
            this.buildEngine.LogWarningEvent(new BuildWarningEventArgs(string.Empty, string.Empty, file, lineNumber, 0, 0, 0, FormatMessage(message), string.Empty, SenderName));
        }

        public void LogError(string message, [CallerFilePath] string file = null, [CallerLineNumber] int lineNumber = 0)
        {
            this.buildEngine.LogErrorEvent(new BuildErrorEventArgs(string.Empty, string.Empty, file, lineNumber, 0, 0, 0, FormatMessage(message), string.Empty, SenderName));
        }

        public void LogError(Exception ex, [CallerFilePath] string file = null, [CallerLineNumber] int lineNumber = 0)
        {
            this.LogError(ex.ToString(), file, lineNumber);
        }

        private static string FormatMessage(string message)
        {
            return $"{SenderName}: {message}";
        }
    }
}
