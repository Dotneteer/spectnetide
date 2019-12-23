using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class is responsible to run the ZXB (ZX BASIC) external utility, provided,
    /// it's set up properly.
    /// </summary>
    public class ZxbRunner
    {
        private readonly string _zxbPath;
        private readonly int _timeOut;

        public ZxbRunner(string zxbPath, int timeout = 1000000)
        {
            _zxbPath = zxbPath;
            _timeOut = timeout;
        }

        public Task<ZxbResult> RunAsync(ZxbOptions options)
        {
            var tcs = new TaskCompletionSource<ZxbResult>();
            var zxbProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(_zxbPath ?? string.Empty, "zxb.exe"),
                    Arguments = options.ToString(),
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            _ = Task.Factory.StartNew(() =>
              {
                  try
                  {
                      zxbProcess.Start();

                      // --- Wait up to 10 seconds to run the process
                      zxbProcess.WaitForExit(_timeOut);
                      if (!zxbProcess.HasExited)
                      {
                          zxbProcess.Kill();
                          tcs.SetException(new InvalidOperationException("ZXB task did not complete within timeout."));
                      }
                      else
                      {
                          var exitCode = zxbProcess.ExitCode;
                          var output = zxbProcess.StandardError.ReadToEnd();

                          tcs.SetResult(new ZxbResult(exitCode, output));
                      }
                  }
                  catch (Exception ex)
                  {
                      tcs.SetException(ex);
                  }
                  finally
                  {
                      zxbProcess.Dispose();
                  }
              });
            return tcs.Task;
        }
    }
}
