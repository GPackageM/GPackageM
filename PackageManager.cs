using Chilkat;
using Downloader;
using GPacket.Enums;
using Task = System.Threading.Tasks.Task;

namespace GPacket;

public static class PackageManager
{
    public static async Task InstallPackage(PackageType pkgType, string packageName)
    {
        var downloadOpt = new DownloadConfiguration()
        {
            ChunkCount = 8, // Number of file parts, default is 1
            ParallelDownload = true // Download parts in parallel (default is false)
        };
        
        var downloader = new DownloadService(downloadOpt);
        
        downloader.DownloadStarted += DownloadEventHandler.OnDownloadStarted;
        
        string file = $"temp/{packageName}.tar.gz";
        string url = $"http://{Program.ReadConfig().Servers[0]}/packages/{packageName}";
        
        await downloader.DownloadFileTaskAsync(url, file);
        
        if (downloader.Status == DownloadStatus.Failed || downloader.Status == DownloadStatus.Stopped)
        {
            Console.WriteLine($"Failed to download package {packageName}.");
            if (!downloader.IsCancelled) Console.WriteLine($"Maybe this package does not exist or the server is not available.");
            return;
        }
        
        Console.WriteLine($"Download completed for package {packageName}, unarchiving...");
        
        Gzip gzip = new Gzip();
        
        bool bNoAbsolute = true;
        string untarToDirectory = $"temp/{packageName}";

        bool success = gzip.UnTarGz(file,untarToDirectory,bNoAbsolute);
        if (success != true) {
            Console.WriteLine(gzip.LastErrorText);
            return;
        }
        
        Console.WriteLine($"Package {packageName} unarchived successfully.");
        
        Console.WriteLine($"Package {packageName} successfully installed.");
    }

    public static void RemovePackage(PackageType pkgType, string pkgName)
    {
        if (!IsPackageInstalled(pkgType, pkgName))
        {
            Console.WriteLine($"Package {pkgName} is not installed.");
            return;
        }

        Directory.Delete($"temp/{pkgName}", true);

        Console.WriteLine($"Package {pkgName} successfully removed.");
    }

    public static bool IsPackageInstalled(PackageType pkgType, string pkgName)
    {
        return Directory.Exists($"temp/{pkgName}");
    }
}