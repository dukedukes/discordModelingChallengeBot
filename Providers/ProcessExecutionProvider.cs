using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public class ProcessExecutionProvider : IProcessExecutionProvider
    {
        private readonly ILogger<ProcessExecutionProvider> logger;

        public ProcessExecutionProvider(ILogger<ProcessExecutionProvider> logger)
        {
            this.logger = logger;
        }

        public Task<bool> RunProcess(string path, string args)
        {
            bool result = false;
            try
            {
                using (System.Diagnostics.Process process = new System.Diagnostics.Process())
                {

                    process.StartInfo.FileName = path;
                    process.StartInfo.Arguments = args;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = true;

                    logger.LogInformation($"Running process with path {path} args {args}");
                    process.Start();
                    process.WaitForExit();
                    result = process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception occurred running {path}. Exception {ex}");
            }
            return Task.FromResult(result);
        }
    }
}
