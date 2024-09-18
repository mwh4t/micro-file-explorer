using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace tasks1_4.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // получение combobox для отображения корневого диска
        var comboBox = this.FindControl<ComboBox>("DiskComboBox");

        // получение имени диска
        var diskName = DiskInfo.GetRootDiskName();
        
        var items = new[] { "Ничего", diskName };
        comboBox.ItemsSource = items;
        comboBox.SelectedItem = "Ничего"; // элемент по умолчанию
        
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
                var directories = DiskInfo.GetDirectories(selectedDisk).Select(Path.GetFileName);
                catalogsListBox.ItemsSource = directories;
                
                // обновление информации о диске
                var (totalSize, freeSpace) = DiskInfo.GetDiskInfo(selectedDisk);
                infoTextBox.Text = $"• Общий объём: {DiskInfo.FormatBytes(totalSize)}\n" +
                                   $"• Свободное пространство: {DiskInfo.FormatBytes(freeSpace)}";
                
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
            var files = DiskInfo.GetFiles(catalogPath).Select(Path.GetFileName);
            filesListBox.ItemsSource = files;

            // получение textbox для вывода информации о каталоге
            var catalogInfoTextBox = this.FindControl<TextBox>("CatalogInfoTextBox");

            // получение информации о каталоге с помощью метода из DiskInfo
            string directoryInfo = DiskInfo.GetDirectoryInfo(catalogPath);

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
            // получение выбранный файл
            string selectedFile = filesListBox.SelectedItem.ToString();

            // полчение combobox для получения выбранного диска
            var comboBox = this.FindControl<ComboBox>("DiskComboBox");
            string selectedDisk = comboBox.SelectedItem.ToString();

            // получение listbox для каталогов
            var catalogsListBox = this.FindControl<ListBox>("CatalogsListBox");

            // получение выбранного каталога
            string selectedCatalog = catalogsListBox.SelectedItem?.ToString() ?? "";

            // получение полного путя к файлу
            string filePath = Path.Combine(selectedDisk, selectedCatalog, selectedFile);

            // открытие файла
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (System.Exception ex)
            {
                var errorTextBox = this.FindControl<TextBox>("InfoTextBox");
                errorTextBox.Text = $"Не удалось открыть файл: {ex.Message}";
            }
        }
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
