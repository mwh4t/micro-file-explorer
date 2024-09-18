namespace example3;

class Program
{
    static void Main(string[] args)
    {
        string dirName = "/Applications";
        
        DirectoryInfo dirInfo = new DirectoryInfo(dirName);
        
        Console.WriteLine("Название каталога: {0}", dirInfo.Name); 
        Console.WriteLine("Полное название каталога: {0}", dirInfo.FullName);
        Console.WriteLine("Время создания каталога: {0}", dirInfo.CreationTime);
        Console.WriteLine("Корневой каталог: {0}", dirInfo.Root);
    }
}
