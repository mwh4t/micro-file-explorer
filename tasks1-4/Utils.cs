using Avalonia.Controls;
using System.IO;
using System.Linq;

namespace tasks1_4;

public static class Utils
{
    // получение имён всех дисков
    public static string[] GetDisksNames()
    {
        var volumesPath = "/Volumes";
        var directories = Directory.GetDirectories(volumesPath);
        
        // извлекаем только имена каталогов (дисков)
        var diskNames = directories.Select(Path.GetFileName).ToArray();
        return diskNames;
    }
    
    // получение каталогов
    public static string[] GetDirectories(string selectedDisk)
    {
        return Directory.GetDirectories("/Volumes/" + selectedDisk);
    }
    
    // получение файлов в каталоге
    public static string[] GetFiles(string path)
    {
        return Directory.GetFiles("/Volumes/" + path);
    }
    
    // получение размера и свободного места на диске
    public static (long totalSize, long freeSpace) GetDiskInfo(string path)
    {
        var driveInfo = new DriveInfo("/Volumes/" + path);
        return (driveInfo.TotalSize, driveInfo.AvailableFreeSpace);
    }
    
    // получение информации о каталоге
    public static string GetDirectoryInfo(string path)
    {
        var directoryInfo = new DirectoryInfo("/Volumes/" + path);
        string fullPath = directoryInfo.FullName;
        string creationTime = directoryInfo.CreationTime.ToString();
        string rootDirectory = directoryInfo.Root.ToString();
        
        // возвращаем строку с подробной информацией о каталоге
        return $"• Полный путь: {fullPath}\n" +
               $"• Время создания: {creationTime}\n" +
               $"• Корневой каталог: {rootDirectory}";
    }
    
    // метод отображения окна ошибки
    public static void ShowErrorMessage(Window parentWindow, string message)
    {
        var errorBox = new Views.ErrorMessageBox();
        errorBox.FindControl<TextBlock>("MessageTextBlock").Text = message;
        errorBox.ShowDialog(parentWindow);
    }
    
    // форматирование размера в читаемый формат
    public static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
