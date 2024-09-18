﻿namespace example2;

class Program
{
    static void Main(string[] args)
    { 
        string dirName = "/";
        
        if (Directory.Exists(dirName))
        {
            Console.WriteLine("Подкаталоги:");
            string[] dirs = Directory.GetDirectories(dirName);
            foreach (string s in dirs)
            {
                Console.WriteLine(s);
            }
            
            Console.WriteLine();
            
            Console.WriteLine("Файлы:");
            string[] files = Directory.GetFiles(dirName);
            foreach (string s in files)
            {
                Console.WriteLine(s);
            }
        }
    }
}
