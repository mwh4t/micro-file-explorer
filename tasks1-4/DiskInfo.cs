using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace tasks1_4;

public static class DiskInfo
{
    public static string GetRootDiskName()
    {
        // получение информации о корневом диске
        var drive = Path.GetPathRoot("/");
        
        return drive;
    }
    
    public static string[] GetDirectories(string rootPath)
    {
        // получение каталогов
        return Directory.GetDirectories(rootPath);
    }
    
    public static string[] GetFiles(string path)
    {
        // получение всех файлов
        return Directory.GetFiles(path);
    }
    
    public static (long totalSize, long freeSpace) GetDiskInfo(string path)
    {
        // получение общего объёма диска и свободного пространства
        var driveInfo = new DriveInfo(Path.GetPathRoot(path));
        return (driveInfo.TotalSize, driveInfo.AvailableFreeSpace);
    }
    
    public static string GetDirectoryInfo(string path)
    {
        // получение информации о каталоге
        var directoryInfo = new DirectoryInfo(path);
        string fullPath = directoryInfo.FullName;
        string creationTime = directoryInfo.CreationTime.ToString();
        string rootDirectory = directoryInfo.Root.ToString();
        
        return $"• Полный путь: {fullPath}\n" +
               $"• Время создания: {creationTime}\n" +
               $"• Корневой каталог: {rootDirectory}";
    }
    
    public static string FormatBytes(long bytes)
    {
        // преображение байтов в читаемый формат
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        if (bytes >= GB)
            return $"{bytes / (double)GB:F2} GB";
        if (bytes >= MB)
            return $"{bytes / (double)MB:F2} MB";
        if (bytes >= KB)
            return $"{bytes / (double)KB:F2} KB";
        return $"{bytes} Bytes";
    }
}
