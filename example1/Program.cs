using System;
using System.Collections.Generic;
using System.IO;


namespace example1;

class Program
{
    static void Main(string[] args)
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
 
        foreach (DriveInfo drive in drives)
        {
            Console.WriteLine("Название: {0}", drive.Name);
                
            if (drive.IsReady)
            {
                Console.WriteLine("Объем диска: {0}", drive.TotalSize);
                Console.WriteLine("Свободное пространство: {0}", drive.TotalFreeSpace);
            }
            Console.WriteLine();
        }
 
        Console.ReadLine();
    }
}
