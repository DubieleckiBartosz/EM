using System;
using System.IO;

namespace EventManagement.Application.Helpers
{
    public static class FileHelper
    {
        public static string CreatePath(this string rootPath, string eventName, string fileName)
        {
            var path = $"{eventName ?? "Event"}-{DateTime.Now.ToShortDateString()}-{fileName}";
            return Path.Combine(rootPath, path);
        }
    }
}