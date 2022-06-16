using System;
using System.IO;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using Microsoft.AspNetCore.Http;

namespace EventManagement.Application.Services
{
    public class FileService : IFileService
    {
        private readonly ILoggerManager<FileService> _loggerManager;

        public FileService(ILoggerManager<FileService> loggerManager)
        {
            this._loggerManager = loggerManager;
        }

        public async Task<string> SaveFileAsync(IFormFile formFile, string rootPath, string eventName = null)
        {
            var rootFilePath = rootPath;
            if (!Directory.Exists(rootFilePath))
            {
                Directory.CreateDirectory(rootFilePath);
            }

            var path = $"{eventName ?? "Event"}-{DateTime.Now.ToShortDateString()}-{formFile.FileName}";
            var finalPath = Path.Combine(rootPath, path);

            await using Stream fileStream = new FileStream(finalPath, FileMode.Create);
            await formFile.CopyToAsync(fileStream);
            this._loggerManager.LogInformation($"The file has been CREATED: {finalPath}");
            return finalPath;
        }

        public void RemoveFile(string filePath)
        {
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                        this._loggerManager.LogInformation($"The file has been DELETED: {filePath}");
                    }
                    catch
                    {
                        this._loggerManager.LogError($"file deletion FAILED: {filePath}");
                        throw;
                    }
                }
            }
        }
    }
}