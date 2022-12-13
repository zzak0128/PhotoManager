using PhotoManager.Views;

namespace PhotoManager
{
    public class PhotoReorganizer
    {
        public void MoveFiles(string folderPath)
        {
            ShowMessage.Info("Moving files to respective Directories");

            DirectoryInfo dir = new DirectoryInfo(folderPath);
            foreach (var file in dir.GetFiles())
            {
                try
                {
                    var slash = Path.DirectorySeparatorChar;
                    file.MoveTo($"{folderPath}{slash}{file.LastWriteTime.Date.Month}-{file.LastWriteTime.Date.Year}{slash}{file.Name}");
                }
                catch (Exception e)
                {
                    ShowMessage.Warning($"Unable to move file {file.FullName}:{e.Message}");
                }
            }
            ShowMessage.Success("File Cleanup has been completed!");
        }

        public void CreateDirectories(string folderPath)
        {
            Console.Clear();
            var dates = ScanFilesForDates(folderPath);
            ShowMessage.Info($"Creating {dates.Count()} directories");
            foreach (var date in dates)
            {
                string newDirName = $"{date.Month}-{date.Year}";
                Directory.CreateDirectory($"{folderPath}{Path.DirectorySeparatorChar}{newDirName}");
                Console.Write($"- {newDirName} -");
            }
            ShowMessage.Success($"{Environment.NewLine}All Directories have been created");
        }

        private List<DateTime> ScanFilesForDates(string folderPath)
        {
            Console.WriteLine("Scanning Files for Dates...");
            List<DateTime> dates = new List<DateTime>();

            DirectoryInfo dir = new DirectoryInfo(folderPath);
            foreach (var file in dir.GetFiles())
            {
                var lastModified = file.LastWriteTime;
                var modLastModified = lastModified.AddDays(-lastModified.Day + 1);
                if (!dates.Contains(modLastModified.Date))
                {
                    dates.Add(modLastModified.Date);
                }
            }

            return dates;
        }
    }
}