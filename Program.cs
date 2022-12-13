using PhotoManager;


if (args.Length == 0)
{
    System.Console.WriteLine("Please put a folder path to begin cleanup");
}
else
{
    Console.WriteLine("Beginning cleanup");

    string filePath = args[0];
    FileScan scanner = new FileScan();
    scanner.CreateDirectories(filePath);
    scanner.MoveFiles(filePath);
}

