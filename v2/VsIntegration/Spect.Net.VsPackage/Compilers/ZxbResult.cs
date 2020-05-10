using Spect.Net.Assembler.Assembler;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the result of running the ZBX utility
    /// </summary>
    public class ZxbResult
    {
        /// <summary>
        /// Process exit code
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// Gets the standard error output
        /// </summary>
        public List<AssemblerErrorInfo> Errors { get; private set; }

        public ZxbResult(int exitCode, string error)
        {
            ExitCode = exitCode;
            GetErrors(error);
        }

        /// <summary>
        /// Create the list of errors from error text
        /// </summary>
        /// <param name="errorOutput"></param>
        private void GetErrors(string errorOutput)
        {
            Errors = new List<AssemblerErrorInfo>();
            var errLines = Regex.Split(errorOutput, "\r\n");
            foreach (var errLine in errLines)
            {
                var parts = errLine.Split(':');
                if (parts.Length >= 3) 
                {
                    var message = string.Join("", parts.Skip(3).ToArray());
                    var errorInfo = new AssemblerErrorInfo(
                        "ZXB",
                        $"{parts[0]}:{parts[1]}",
                        int.TryParse(parts[2], out var parsed) ? parsed : 0,
                        1,
                        message,
                        message.ToLower().Trim().StartsWith("warning"));
                    Errors.Add(errorInfo);
                }
            }
        }
    }
}
