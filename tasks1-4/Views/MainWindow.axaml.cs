using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tasks1_4.Views;

public partial class MainWindow : Window
{
    // список открытых файлов со временем
    private List<(string fileName, DateTime openTime)> openedFiles = new List<(string, DateTime)>();
    
    // время запуска программы
    private DateTime programStartTime;
    
    public MainWindow()
    {
        InitializeComponent();
        
        programStartTime = DateTime.Now;
        
        // подписка на событие завершения работы
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        
        // очистка файла
        File.WriteAllText("/Users/mwh4t/Desktop/Уче\u0308ба/Колледж/3 курс/1 семестр/РПМ/lr1rpm/tasks1-4/Открываемые_файлы.txt", string.Empty);
        
        // получение ссылки на ComboBox для выбора диска
        var comboBox = this.FindControl<ComboBox>("DiskComboBox");
        
        // получение списка дисков
        var diskNames = Utils.GetDisksNames();
        
        var items = new List<string> { "Ничего" };
        items.AddRange(diskNames);
        
        comboBox.ItemsSource = items;
        comboBox.SelectedItem = "Ничего"; // элемент по умолчанию
        
        // подписка на событие изменения диска
        comboBox.SelectionChanged += ComboBox_SelectionChanged;
    }

    // обработка изменения диска
    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox != null && comboBox.SelectedItem != null)
        {
            // получение диска
            string selectedDisk = comboBox.SelectedItem.ToString();
            
            var catalogsListBox = this.FindControl<ListBox>("CatalogsListBox");
            var infoTextBox = this.FindControl<TextBox>("InfoTextBox");
            var catalogInfoTextBox = this.FindControl<TextBox>("CatalogInfoTextBox");

            if (selectedDisk == "Ничего")
            {
                // очистка всех связанных элементов управления
                catalogsListBox.ItemsSource = null;
                var filesListBox = this.FindControl<ListBox>("FilesListBox");
                filesListBox.ItemsSource = null;
                infoTextBox.Text = string.Empty;
                catalogInfoTextBox.Text = string.Empty;
            }
            else
            {
                // получение каталогов
                var directories = Utils.GetDirectories(selectedDisk).Select(Path.GetFileName);
                
                catalogsListBox.ItemsSource = directories;
                
                // получение размера и свободного места на диске
                var (totalSize, freeSpace) = Utils.GetDiskInfo(selectedDisk);
                
                infoTextBox.Text = $"• Общий объём: {Utils.FormatBytes(totalSize)}\n" +
                                   $"• Свободное пространство: {Utils.FormatBytes(freeSpace)}";
                
                // подписка на событие изменения каталога
                catalogsListBox.SelectionChanged += CatalogsListBox_SelectionChanged;
            }
        }
    }

    // обработка изменения каталога
    private void CatalogsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var catalogsListBox = sender as ListBox;
        if (catalogsListBox != null && catalogsListBox.SelectedItem != null)
        {
            // получение каталога и диска
            string selectedCatalog = catalogsListBox.SelectedItem.ToString();
            var comboBox = this.FindControl<ComboBox>("DiskComboBox");
            string selectedDisk = comboBox.SelectedItem.ToString();
            
            // полный путь к каталогу
            string catalogPath = Path.Combine(selectedDisk, selectedCatalog);
            
            var filesListBox = this.FindControl<ListBox>("FilesListBox");
            
            // получение и отображение файлов в каталоге
            var files = Utils.GetFiles(catalogPath).Select(Path.GetFileName);
            filesListBox.ItemsSource = files;
            
            var catalogInfoTextBox = this.FindControl<TextBox>("CatalogInfoTextBox");
            
            // получение и отображение информации о каталоге
            string directoryInfo = Utils.GetDirectoryInfo(catalogPath);
            catalogInfoTextBox.Text = directoryInfo;

            // подписка на событие изменения файла
            filesListBox.SelectionChanged += FilesListBox_SelectionChanged;
        }
    }

    // обработка изменения файла
    private void FilesListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var filesListBox = sender as ListBox;
        if (filesListBox != null && filesListBox.SelectedItem != null)
        {
            // получение файла и запись времени его открытия
            string selectedFile = filesListBox.SelectedItem.ToString();
            DateTime openTime = DateTime.Now;
            openedFiles.Add((selectedFile, openTime));

            // получение пути к файлу
            var comboBox = this.FindControl<ComboBox>("DiskComboBox");
            string selectedDisk = comboBox.SelectedItem.ToString();
            var catalogsListBox = this.FindControl<ListBox>("CatalogsListBox");
            string selectedCatalog = catalogsListBox.SelectedItem?.ToString() ?? "";
            string filePath = Path.Combine("/Volumes/", selectedDisk, selectedCatalog, selectedFile);
            
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                Utils.ShowErrorMessage(this, "Не удалось открыть файл!");
            }
        }
    }
    
    // обработка завершения работы программы
    private void OnProcessExit(object? sender, EventArgs e)
    {
        DateTime programEndTime = DateTime.Now;
        
        // получение списка файлов, открытых за последние 10 секунд
        var recentFiles = openedFiles
            .Where(f => (programEndTime - f.openTime).TotalSeconds <= 10)
            .GroupBy(f => f.fileName) // группировка по имени файла
            .Select(g => g.OrderByDescending(f => f.openTime).First()) // выбор самого последнего файла в каждой группе
            .Select(f => $"{f.fileName} - {(programEndTime - f.openTime).TotalSeconds:F2} сек до завершения")
            .ToList();
        
        // запись списка в файл
        File.WriteAllLines("/Users/mwh4t/Desktop/Уче\u0308ба/Колледж/3 курс/1 семестр/РПМ/lr1rpm/tasks1-4/Открываемые_файлы.txt", recentFiles);
        
        Task.Delay(1000).Wait();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
