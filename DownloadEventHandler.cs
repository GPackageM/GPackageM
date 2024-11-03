using System.ComponentModel;
using System.Net;
using Downloader;

namespace GPacket;

public static class DownloadEventHandler
{
    public static void OnDownloadStarted(object? sender, DownloadStartedEventArgs e)
    {
        Console.WriteLine($"Download started for package {e.FileName}...");
    }
}