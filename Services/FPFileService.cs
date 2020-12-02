using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RockTransactions.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Services
{
    public class FPFileService : IFPFileService
    {
        private readonly DefaultSettings _defaultSettings;
        public FPFileService(IOptions<DefaultSettings> defaultSettings)
        {
            _defaultSettings = defaultSettings.Value;
        }

        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            MemoryStream memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var byteFile = memoryStream.ToArray();
            memoryStream.Close();
            memoryStream.Dispose();
            return byteFile;
        }

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            string imageBase64Data = Convert.ToBase64String(fileData);
            return string.Format($"data:image/{extension};base64,{imageBase64Data}");
        }

        public string GetFileIcon(string file)
        {
            string ext = Path.GetExtension(file).Replace(".", "");
            return $"/img/png/{ext}.png";
        }

        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }

        public string ShortenFileName(string name, int length)
        {
            name = Path.GetFileNameWithoutExtension(name);
            var len = name.Length > length ? length : name.Length;
            string result = name.Substring(0, len);
            result = $"{result}...";
            return result;
        }

        public string GetDefaultAvatarFileName()
        {
            return _defaultSettings.Avatar;
        }

        public async Task<byte[]> GetDefaultAvatarFileBytesAsync()
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/assets/img", _defaultSettings.Avatar);
                return await File.ReadAllBytesAsync(path);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
