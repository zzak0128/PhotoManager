using MediaDevices;
using ImageMagick;
using PhotoManager.Views;

namespace PhotoManager
{
    public static class PhotoFetcher
    {
        public static void PhotoImport(string filePath)
        {
            FetchPhotos(filePath);
            ConvertFolder(filePath);
            CleanupMOV(filePath);
        }

        private static void FetchPhotos(string filePath)
        {
            var devices = MediaDevice.GetDevices();

            using (var device = devices.First(d => d.FriendlyName.Contains("iPhone")))
            {
                ShowMessage.Info($"Connecting to {device.FriendlyName}");
                device.Connect();

                string photoPath = @"/Internal Storage/DCIM";

                if (device.DirectoryExists(photoPath))
                {
                    var deviceDirectories = device.GetDirectories(photoPath);
                    foreach (var directory in deviceDirectories)
                    {
                        Console.WriteLine(directory);

                        device.DownloadFolder(directory, filePath, true);
                        Console.WriteLine($"Copying: {directory}");
                    }
                }

                device.Disconnect();
            }
        }

        private static void ConvertFolder(string folderPath)
        {
            ShowMessage.Info($"Now converting images to jpg files");
            var dir = new DirectoryInfo(folderPath);
            foreach (var file in dir.GetFiles())
            {
                if (file.Extension == ".HEIC")
                {
                    Console.WriteLine($"Converting: {file.Name}");
                    ConvertToJpeg(MagickFormat.Jpg, file.FullName, file.DirectoryName);
                    file.Delete();
                }
                else
                {
                    Console.WriteLine($"{file.Name} is not .HEIC... Skipping");
                }
            }
            ShowMessage.Success("Files converted Successfully");
        }

        private static void CleanupMOV(string folderPath)
        {
            ShowMessage.Info($"beginning cleanup of .MOV files...");
            var dir = new DirectoryInfo(folderPath);
            foreach (var file in dir.GetFiles())
            {
                if(file.Extension == ".MOV" || file.Extension == ".AAE")
                {
                    var fileLength = (file.Length / 1024f) / 1024f;
                    if (fileLength < 6)
                    {
                        Console.WriteLine($"Deleting: {file.Name}");
                        file.Delete();
                    }
                }
            }
        }

        private static void ConvertToJpeg(MagickFormat convertToFormat, string fileName, string outputPath)
        {
            try
            {
                using var image = new MagickImage(fileName);
                image.Format = convertToFormat;
                image.Write($"{outputPath}/{Path.GetFileNameWithoutExtension(fileName)}.jpg");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not write: {Path.GetFileName(fileName)}. Error: {ex.Message}");
            }
        }

        private static void ConvertFile(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (ext == ".heic")
            {
                Console.WriteLine($"Found {Path.GetFileName(fileName)}. Converting to JPG...");
                ConvertToJpeg(MagickFormat.Jpg, fileName, "./output");
            }
            else
            {
                Console.WriteLine($"{Path.GetFileName(fileName)} is not .HEIC... Skipping");
            }
        }
    }
}
