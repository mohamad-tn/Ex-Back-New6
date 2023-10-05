using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bwr.Exchange.Shared.Dto
{
    public class FileUploadDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public string FileAsBase64 { get; set; }
        public byte[] FileAsByteArray { get; set; }

        public string CreateUniqueFileName(string path)
        {
            return path + Path.GetFileNameWithoutExtension(FileName) + "-" +
                DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                Path.GetExtension(FileName);
        }

        public void RemoveFileType()
        {
            if (FileAsBase64.Contains(","))
            {
                FileAsBase64 = FileAsBase64.Substring(FileAsBase64.IndexOf(",") + 1);
            }
        }

        public void ConvertToBinary()
        {
            FileAsByteArray = Convert.FromBase64String(FileAsBase64);
        }

        public void WriteToFile(string filePathName)
        {
            RemoveFileType();
            ConvertToBinary();

            using (var fs = new FileStream(filePathName, FileMode.CreateNew))
            {
                fs.Write(FileAsByteArray, 0, FileAsByteArray.Length);
            }
        }

        public string GetExtension()
        {
            return FileName.Split('.')[1];
        }

        public string SaveFileAndGetUrl(string path, string folderName)
        {
            try
            {
                var extension = Path.GetExtension(FileName);
                FileName = $"{Guid.NewGuid().ToString()}";

                var filePath = path + $"\\{folderName}\\{FileName}{extension}";

                WriteToFile(filePath);

                return $"{folderName}/{FileName}{extension}";
            }
            catch
            {
                return String.Empty;
            }
        }

    }
}
