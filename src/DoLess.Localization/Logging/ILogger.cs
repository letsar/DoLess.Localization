using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DoLess.Localization
{
    public interface ILogger
    {
        void LogDebug(string message);

        void LogInfo(string message);

        void LogWarning(string message, [CallerFilePath] string file = null, [CallerLineNumber] int lineNumber = 0);

        void LogError(string message, [CallerFilePath] string file = null, [CallerLineNumber] int lineNumber = 0);

        void LogError(Exception ex, [CallerFilePath] string file = null, [CallerLineNumber] int lineNumber = 0);
    }
}
