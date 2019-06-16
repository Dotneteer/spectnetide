using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Spect.Net.VsPackage.ZxBasic
{
    /// <summary>
    /// This class is responsible to run the ZXB (ZX BASIC) external utility, provided,
    /// it's set up properly.
    /// </summary>
    public class ZxbRunner
    {
        private readonly string _zxbPath;

        public ZxbRunner(string zxbPath)
        {
            _zxbPath = zxbPath;
        }

        public Task<ZxbResult> RunAsync(ZxbOptions options)
        {
            var tcs = new TaskCompletionSource<ZxbResult>();
            var zxbProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(_zxbPath, "zxb.exe"),
                    Arguments = options.ToString(),
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            Task.Factory.StartNew(() =>
            {
                zxbProcess.Start();

                // --- Wait up to 10 seconds to run the process
                zxbProcess.WaitForExit(10000);
                if (!zxbProcess.HasExited)
                {
                    zxbProcess.Kill();
                    tcs.SetException(new InvalidOperationException("ZXB task did not complete within timeout."));
                }
                else
                {
                    tcs.SetResult(new ZxbResult());
                }
            });
            return tcs.Task;
        }
    }
}