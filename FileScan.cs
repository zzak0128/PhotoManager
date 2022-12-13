namespace PhotoManager
{
    public class FileScan
    {
        public void MoveFiles(string folderPath)
        {
            Console.WriteLine("Moving files to respective Directories");

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
                    Console.WriteLine($"Unable to move file {file.FullName}:{e.Message}");
                }
            }
            Console.WriteLine("File Cleanup has been completed!");
        }

        public void CreateDirectories(string folderPath)
        {
            var dates = ScanFilesForDates(folderPath);
            Console.WriteLine($"Creating {dates.Count()} directories");
            foreach (var date in dates)
            {
                string newDirName = $"{date.Month}-{date.Year}";
                Directory.CreateDirectory($"{folderPath}{Path.DirectorySeparatorChar}{newDirName}");
                Console.Write($"- {newDirName} -");
            }
            Console.WriteLine($"{Environment.NewLine}All Directories have been created");
        }

        private List<DateTime> ScanFilesForDates(string folderPath)
        {
            System.Console.WriteLine("Scanning Files for Dates...");
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