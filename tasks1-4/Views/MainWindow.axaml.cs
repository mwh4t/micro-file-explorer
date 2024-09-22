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
    // Хранение имени файла и времени его открытия
    private List<(string fileName, DateTime openTime)> openedFiles = new List<(string, DateTime)>();
    private DateTime programStartTime;
    
    public MainWindow()
    {
        InitializeComponent();
        
        programStartTime = DateTime.Now;
        
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        
        // очистка txt
        File.WriteAllText("/Users/mwh4t/Desktop/Учёба/Колледж/3 курс/1 семестр/РПМ/lr1rpm/tasks1-4/Открываемые_файлы.txt",
            string.Empty);
    
        // получение combobox для отображения дисков
        var comboBox = this.FindControl<ComboBox>("DiskComboBox");

        // получение списка дисков
        var diskNames = Utils.GetDisksNames();
    
        // добавляем "Ничего" как первый элемент
        var items = new List<string> { "Ничего" };
    
        // добавляем остальные диски
        items.AddRange(diskNames);
    
        // присваиваем источнику данных combobox
        comboBox.ItemsSource = items;
        comboBox.SelectedItem = "Ничего"; // элемент по умолчанию
    
        // обработка события изменения выбранного элемента
        comboBox.SelectionChanged += ComboBox_SelectionChanged;
    }

    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox != null && comboBox.SelectedItem != null)
        {
            // получение выбранного элемента
            string selectedDisk = comboBox.SelectedItem.ToString();

            // получение listbox для отображения каталогов
            var catalogsListBox = this.FindControl<ListBox>("CatalogsListBox");
            
            // получение textbox для отображения информации о диске
            var infoTextBox = this.FindControl<TextBox>("InfoTextBox");
            
            // получение textbox для отображения информации о каталоге
            var catalogInfoTextBox = this.FindControl<TextBox>("CatalogInfoTextBox");
            
            if (selectedDisk == "Ничего")
            {
                catalogsListBox.ItemsSource = null;
                
                var filesListBox = this.FindControl<ListBox>("FilesListBox");
                filesListBox.ItemsSource = null;
                
                infoTextBox.Text = string.Empty;
                catalogInfoTextBox.Text = string.Empty;
            }
            else
            {
                // получение списка каталогов
                var directories = Utils.GetDirectories(selectedDisk).Select(Path.GetFileName);
                catalogsListBox.ItemsSource = directories;
                
                // обновление информации о диске
                var (totalSize, freeSpace) = Utils.GetDiskInfo(selectedDisk);
                infoTextBox.Text = $"• Общий объём: {Utils.FormatBytes(totalSize)}\n" +
                                   $"• Свободное пространство: {Utils.FormatBytes(freeSpace)}";
                
                catalogsListBox.SelectionChanged += CatalogsListBox_SelectionChanged;
            }
        }
    }

    private void CatalogsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var catalogsListBox = sender as ListBox;

        if (catalogsListBox != null && catalogsListBox.SelectedItem != null)
        {
            // получение выбранного каталога
            string selectedCatalog = catalogsListBox.SelectedItem.ToString();

            // получение combobox для получения выбранного диска
            var comboBox = this.FindControl<ComboBox>("DiskComboBox");
            string selectedDisk = comboBox.SelectedItem.ToString();

            // получение полного пути к выбранному каталогу
            string catalogPath = Path.Combine(selectedDisk, selectedCatalog);

            // получение listbox для отображения файлов
            var filesListBox = this.FindControl<ListBox>("FilesListBox");

            // получение списка файлов в выбранном каталоге
            var files = Utils.GetFiles(catalogPath).Select(Path.GetFileName);
            filesListBox.ItemsSource = files;

            // получение textbox для вывода информации о каталоге
            var catalogInfoTextBox = this.FindControl<TextBox>("CatalogInfoTextBox");

            // получение информации о каталоге с помощью метода из DiskInfo
            string directoryInfo = Utils.GetDirectoryInfo(catalogPath);

            // Добавляем обработчик для открытия файла при выборе
            filesListBox.SelectionChanged += FilesListBox_SelectionChanged;
            
            // обновление textbox с информацией о каталоге
            catalogInfoTextBox.Text = directoryInfo;
        }
    }
    
    private void FilesListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var filesListBox = sender as ListBox;

        if (filesListBox != null && filesListBox.SelectedItem != null)
        {
            // Получаем выбранный файл и время его открытия
            string selectedFile = filesListBox.SelectedItem.ToString();
            DateTime openTime = DateTime.Now;

            // Добавляем в список открытых файлов с временем открытия
            openedFiles.Add((selectedFile, openTime));

            // Выводим информацию о файле (например, открытие)
            var errorTextBox = this.FindControl<TextBox>("InfoTextBox");
            errorTextBox.Text = $"Файл \"{selectedFile}\" был открыт в {openTime.ToString("HH:mm:ss")}";

            // получение combobox для получения выбранного диска
            var comboBox = this.FindControl<ComboBox>("DiskComboBox");
            string selectedDisk = comboBox.SelectedItem.ToString();

            // получение listbox для каталогов
            var catalogsListBox = this.FindControl<ListBox>("CatalogsListBox");

            // получение выбранного каталога
            string selectedCatalog = catalogsListBox.SelectedItem?.ToString() ?? "";

            // получение полного пути к файлу
            string filePath = Path.Combine("/Volumes/", selectedDisk, selectedCatalog, selectedFile);

            // открытие файла
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (System.Exception ex)
            {
                var errorTextBox1 = this.FindControl<TextBox>("InfoTextBox");
                errorTextBox1.Text = $"Не удалось открыть файл: {ex.Message}";
            }
        }
    }
    
    private void OnProcessExit(object? sender, EventArgs e)
    {
        // Текущее время завершения программы
        DateTime programEndTime = DateTime.Now;

        // Ищем файлы, которые были открыты за последние 10 секунд
        var recentFiles = openedFiles
            .Where(f => (programEndTime - f.openTime).TotalSeconds <= 10)
            .Select(f => $"Файл: {f.fileName}, Время открытия: {(programEndTime - f.openTime).TotalSeconds:F2} секунд до завершения")
            .ToList();

        // Записываем файлы в "Открываемые_файлы.txt"
        File.WriteAllLines("/Users/mwh4t/Desktop/Учёба/Колледж/3 курс/1 семестр/РПМ/lr1rpm/tasks1-4/Открываемые_файлы.txt", recentFiles);

        // Задержка на 1 секунду перед завершением
        Task.Delay(1000).Wait();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
