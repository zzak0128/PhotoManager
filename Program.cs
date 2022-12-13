using PhotoManager;
using PhotoManager.Views;

if (args.Length == 0)
{
    DisplayView.StartMessage();
}
else
{
    Console.WriteLine("Beginning cleanup");

    string filePath = args[0];
    PhotoReorganizer scanner = new PhotoReorganizer();
    scanner.CreateDirectories(filePath);
    scanner.MoveFiles(filePath);
}

