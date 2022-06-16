using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EventManagement.Application.Contracts
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile formFile, string rootPath, string eventName = null);
        void RemoveFile(string filePath);
    }
}
