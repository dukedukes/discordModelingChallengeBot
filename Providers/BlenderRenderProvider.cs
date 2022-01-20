using Microsoft.Extensions.Logging;
using ModelChallengeBot.Configuration;
using ModelChallengeBot.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers
{
    public class BlenderRenderProvider : IRenderProvider
    {
        private readonly ILogger<BlenderRenderProvider> logger;
        private readonly BlenderSettings blenderSettings;
        private readonly WorkspaceSettings workspaceSettings;
        private readonly IProcessExecutionProvider processExecutionProvider;

        public BlenderRenderProvider(ILogger<BlenderRenderProvider> logger, BlenderSettings blenderSettings, WorkspaceSettings workspaceSettings, IProcessExecutionProvider processExecutionProvider)
        {
            this.logger = logger;
            this.blenderSettings = blenderSettings;
            this.workspaceSettings = workspaceSettings;
            this.processExecutionProvider = processExecutionProvider;
        }

        public async Task<string> Render(string blendFile, string newFileName)
        {
            string renderDirectory = workspaceSettings.RenderDirectory;
            string fileNamePrefix = StringUtil.GenerateUniqueName();
            string renderFile = Path.Combine(renderDirectory, fileNamePrefix);

            string blenderArguments = string.Format(blenderSettings.RenderArgs, blendFile, renderFile);
            try
            {
                logger.LogInformation($"Rendering {blendFile}");
                await processExecutionProvider.RunProcess(blenderSettings.Path, blenderArguments);
            }
            catch (Exception)
            {
                logger.LogError("Blender background render failed");
                throw;
            }
            string fullFileName = string.Concat(fileNamePrefix, blenderSettings.OutputSuffix);
            if (!string.IsNullOrEmpty(newFileName))
            {
                string extension = fullFileName.Substring(fullFileName.LastIndexOf('.'));
                newFileName = string.Concat(newFileName, extension);
                File.Move(Path.Combine(renderDirectory, fullFileName), Path.Combine(renderDirectory, newFileName));
                fullFileName = newFileName;
            }
            return GetOutputFile(renderDirectory, fullFileName);
        }
        public async Task<string> Render(string blendFile)
        {
            return await Render(blendFile, "");
        }

        private string GetOutputFile(string renderPath, string fullFileName)
        {
            var file = Directory.GetFiles(renderPath, fullFileName).SingleOrDefault();
            if (file == null)
            {
                logger.LogError($"Failed to find output of render. Expected {fullFileName}. dumping directory");
                Directory.GetFiles(renderPath).ToList().ForEach(file => logger.LogInformation(file));
                logger.LogInformation("done dump");
                throw new Exception("Render unsuccessful, file not found");
            }

            return file;
        }

        public async Task DeleteRenderOutput(string file)
        {
            try
            {
                await Task.Run(() => File.Delete(file));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete file {file}. Exception {ex}");
            }
        }    
    }
}
