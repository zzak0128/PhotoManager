using MediaDevices;
using ImageMagick;
using PhotoManager.Views;

namespace PhotoManager
{
    public static class PhotoFetcher
    {
        public static async Task PhotoImport(string filePath)
        {
            FetchPhotos(filePath);
            await ConvertFolder(filePath);
            await CleanupLivePhotos(filePath);
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

        private static async Task ConvertFolder(string folderPath)
        {
            List<Task> tasks = new List<Task>();
            ShowMessage.Info($"Now converting images to jpg files");
            var dir = new DirectoryInfo(folderPath);
            foreach (var file in dir.GetFiles())
            {
                if (file.Extension == ".HEIC")
                {
                    //Console.WriteLine($"Converting: {file.Name}");
                    tasks.Add(ConvertToJpegAsync(MagickFormat.Jpg, file.FullName, file.DirectoryName));
                    //await ConvertToJpegAsync(MagickFormat.Jpg, file.FullName, file.DirectoryName);
                    //file.Delete();
                }
                else
                {
                    //Console.WriteLine($"{file.Name} is not .HEIC... Skipping");
                }
            }

            await Task.WhenAll(tasks);

            foreach (var file in dir.GetFiles())
            {
                if (file.Extension == ".HEIC")
                {
                    file.Delete();
                }
            }

            ShowMessage.Success("Files converted Successfully");
        }

        private static async Task CleanupLivePhotos(string folderPath)
        {
            List<Task> tasks = new List<Task>();
            ShowMessage.Info($"beginning cleanup of LivePhoto files...");
            var dir = new DirectoryInfo(folderPath);
            foreach (var file in dir.GetFiles())
            {
                tasks.Add(Task.Run(() => DeleteFile(file)));
            }

            await Task.WhenAll(tasks);
        }

        private static void DeleteFile(FileInfo file)
        {
            if (file.Extension == ".MOV" || file.Extension == ".AAE")
            {
                var fileLength = (file.Length / 1024f) / 1024f;
                if (fileLength < 6)
                {
                    Console.WriteLine($"Deleting: {file.Name}");
                    file.Delete();
                }
            }
        }

        private static async Task ConvertToJpegAsync(MagickFormat convertToFormat, string fileName, string outputPath)
        {
            try
            {
                using var image = new MagickImage(fileName);
                image.Format = convertToFormat;
                await image.WriteAsync($"{outputPath}/{Path.GetFileNameWithoutExtension(fileName)}.jpg");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not write: {Path.GetFileName(fileName)}. Error: {ex.Message}");
            }
        }
    }
}
