using Spect.Net.VsPackage.VsxLibrary.Output;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OutputWindow = Spect.Net.VsPackage.VsxLibrary.Output.OutputWindow;

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

        public ZxbRunner(string zxbPath, int timeout = 300000)
        {
            _zxbPath = zxbPath;
            _timeOut = timeout;
        }

        public Task<ZxbResult> RunAsync(ZxbOptions options, bool log = false)
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
                      if (log)
                      {
                          var pane = OutputWindow.GetPane<Z80AssemblerOutputPane>();
                          pane.WriteLine($"Starting ZXB with: {zxbProcess.StartInfo.Arguments}");
                      }

                      var output = new StringBuilder();
                      var error = new StringBuilder();

                      using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                      using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                      {
                          zxbProcess.OutputDataReceived += (sender, e) => {
                              if (e.Data == null)
                              {
                                  outputWaitHandle.Set();
                              }
                              else
                              {
                                  output.AppendLine(e.Data);
                              }
                          };
                          zxbProcess.ErrorDataReceived += (sender, e) =>
                          {
                              if (e.Data == null)
                              {
                                  errorWaitHandle.Set();
                              }
                              else
                              {
                                  error.AppendLine(e.Data);
                              }
                          };

                          zxbProcess.Start();

                          zxbProcess.BeginOutputReadLine();
                          zxbProcess.BeginErrorReadLine();

                          if (zxbProcess.WaitForExit(_timeOut) &&
                              outputWaitHandle.WaitOne(_timeOut) &&
                              errorWaitHandle.WaitOne(_timeOut))
                          {
                              var exitCode = zxbProcess.ExitCode;
                              tcs.SetResult(new ZxbResult(exitCode, error.ToString()));
                          }
                          else
                          {
                              zxbProcess.Kill();
                              tcs.SetException(new InvalidOperationException("ZXB task did not complete within timeout."));
                          }
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
