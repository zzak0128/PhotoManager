namespace PhotoManager.Views
{
    public static class DisplayView
    {
        public static void StartMessage()
        {
            Console.WriteLine($"PhotoManager{Environment.NewLine}");
            Console.WriteLine($"Usage: PhotoManager [path-to-folder]{Environment.NewLine}");
            Console.WriteLine($"path-to-folder:");
            Console.WriteLine("    The path to the folder you want to organize into date folders");
        }
    }
}
