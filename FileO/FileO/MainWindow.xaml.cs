using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FileO.Models;
using FileO.ViewModels;
using System.Collections.Generic;

namespace FileO
{
    public partial class MainWindow : Window
    {
        private readonly TreeViewModel _treeViewModel;

        public ObservableCollection<LBItem> Items { get; set; }
        public DriveInfo CurrentDrive { get; set; }
        public List<FileInfo> FeaturedFiles = new List<FileInfo>();

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация модели представления для дерева
            _treeViewModel = new TreeViewModel();
            DataContext = _treeViewModel;

            // Загрузка списка дисков
            Items = new ObservableCollection<LBItem>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                Items.Add(new LBItem(drive, "Icons/BaseIcon.png"));
            }
            Drives.ItemsSource = Items;
            FeaturedFiles_Read();
        }

        private void Drives_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is LBItem selectedItem)
            {
                CurrentDrive = selectedItem.ThisDrive;

                // Очищаем содержимое дерева и загружаем данные через ViewModel
                _treeViewModel.Load(CurrentDrive.RootDirectory.FullName);
                FileTree.ItemsSource = _treeViewModel.Items;
            }
        }

        // Контекстное меню
        private void FileTree_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Проверяем, выбран ли элемент в TreeView
            if (FileTree.SelectedItem is DtoItem selectedItem)
            {
                // Включаем контекстное меню, если выбран диск или директория
                if (selectedItem.ItemKind == Kind.Directory || selectedItem.ItemKind == Kind.File)
                {
                    var contextMenu = (ContextMenu)FileTree.ContextMenu;
                    contextMenu.IsEnabled = true;
                }
                else
                {
                    var contextMenu = (ContextMenu)FileTree.ContextMenu;
                    contextMenu.IsEnabled = false;
                }
            }
            else
            {
                // Если ничего не выбрано, отключаем контекстное меню
                var contextMenu = (ContextMenu)FileTree.ContextMenu;
                contextMenu.IsEnabled = false;
            }
        }

        private void UserButtons_ListBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Проверяем, выбран ли элемент в ListBox
            if (UserButtons_ListBox.SelectedItem != null)
            {
                var contextMenu = (ContextMenu)UserButtons_ListBox.ContextMenu;
                contextMenu.IsEnabled = true;
            }
            else
            {
                // Если ничего не выбрано, отключаем контекстное меню
                var contextMenu = (ContextMenu)UserButtons_ListBox.ContextMenu;
                contextMenu.IsEnabled = false;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is DtoItem selectedItem && selectedItem.ItemKind == Kind.Directory)
            {
                Process.Start("explorer.exe", selectedItem.Tag.ToString());
            }
            else if (FileTree.SelectedItem is DtoItem fileItem && fileItem.ItemKind == Kind.File)
            {
                Process.Start(fileItem.Tag.ToString());
            }
        }

        private void ListBox_Open_Click(object sender, RoutedEventArgs e)
        {
            if (UserButtons_ListBox.SelectedItem != null)
            {
                string selectedPath = UserButtons_ListBox.SelectedItem.ToString();
                try
                {
                    if (Directory.Exists(selectedPath))
                    {
                        Process.Start("explorer.exe", selectedPath);
                    }
                    else if (File.Exists(selectedPath))
                    {
                        Process.Start(selectedPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is DtoItem selectedItem)
            {
                string oldPath = selectedItem.Tag.ToString();
                string newPath = Microsoft.VisualBasic.Interaction.InputBox("Enter new name:", "Rename", Path.GetFileName(oldPath));

                if (!string.IsNullOrEmpty(newPath))
                {
                    string directory = Path.GetDirectoryName(oldPath);
                    string fullNewPath = Path.Combine(directory, newPath);

                    try
                    {
                        if (selectedItem.ItemKind == Kind.Directory)
                        {
                            Directory.Move(oldPath, fullNewPath);
                        }
                        else if (selectedItem.ItemKind == Kind.File)
                        {
                            File.Move(oldPath, fullNewPath);
                        }

                        // Обновляем имя в дереве
                        selectedItem.Name = Path.GetFileName(fullNewPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void ListBox_Rename_Click(object sender, RoutedEventArgs e)
        {
            if (UserButtons_ListBox.SelectedItem != null)
            {
                string oldPath = UserButtons_ListBox.SelectedItem.ToString();
                string newPath = Microsoft.VisualBasic.Interaction.InputBox("Enter new name:", "Rename", Path.GetFileName(oldPath));

                if (!string.IsNullOrEmpty(newPath))
                {
                    string directory = Path.GetDirectoryName(oldPath);
                    string fullNewPath = Path.Combine(directory, newPath);

                    try
                    {
                        if (Directory.Exists(oldPath))
                        {
                            Directory.Move(oldPath, fullNewPath);
                        }
                        else if (File.Exists(oldPath))
                        {
                            File.Move(oldPath, fullNewPath);
                        }

                        // Обновляем ListBox
                        int index = UserButtons_ListBox.SelectedIndex;
                        UserButtons_ListBox.Items[index] = fullNewPath;

                        MessageBox.Show("Переименовано успешно.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is DtoItem selectedItem)
            {
                try
                {
                    if (selectedItem.ItemKind == Kind.Directory)
                    {
                        Directory.Delete(selectedItem.Tag.ToString(), true);
                    }
                    else if (selectedItem.ItemKind == Kind.File)
                    {
                        File.Delete(selectedItem.Tag.ToString());
                    }

                    // Удаляем элемент из дерева
                    var parent = GetParentTreeViewItem((TreeViewItem)((FrameworkElement)e.OriginalSource).TemplatedParent);
                    if (parent != null)
                    {
                        parent.Items.Remove(FileTree.SelectedItem);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void ListBox_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (UserButtons_ListBox.SelectedItem != null)
            {
                string selectedPath = UserButtons_ListBox.SelectedItem.ToString();

                try
                {
                    if (Directory.Exists(selectedPath))
                    {
                        Directory.Delete(selectedPath, true);
                    }
                    else if (File.Exists(selectedPath))
                    {
                        File.Delete(selectedPath);
                    }

                    // Удаляем элемент из ListBox
                    UserButtons_ListBox.Items.Remove(UserButtons_ListBox.SelectedItem);

                    MessageBox.Show("Удалено успешно.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private TreeViewItem GetParentTreeViewItem(DependencyObject child)
        {
            while (child != null && !(child is TreeViewItem))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as TreeViewItem;
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is DtoItem selectedItem && selectedItem.ItemKind == Kind.Directory)
            {
                string destination = Microsoft.VisualBasic.Interaction.InputBox("Enter destination path:", "Copy");
                if (!string.IsNullOrEmpty(destination))
                {
                    try
                    {
                        Directory.CreateDirectory(destination);
                        foreach (var file in Directory.GetFiles(selectedItem.Tag.ToString()))
                        {
                            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void ListBox_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (UserButtons_ListBox.SelectedItem != null)
            {
                string sourcePath = UserButtons_ListBox.SelectedItem.ToString();
                string destination = Microsoft.VisualBasic.Interaction.InputBox("Enter destination path:", "Copy");

                if (!string.IsNullOrEmpty(destination))
                {
                    try
                    {
                        if (Directory.Exists(sourcePath))
                        {
                            foreach (var file in Directory.GetFiles(sourcePath))
                            {
                                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
                            }
                        }
                        else if (File.Exists(sourcePath))
                        {
                            File.Copy(sourcePath, Path.Combine(destination, Path.GetFileName(sourcePath)), true);
                        }

                        MessageBox.Show("Скопировано успешно.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is DtoItem selectedItem)
            {
                string destination = Microsoft.VisualBasic.Interaction.InputBox("Enter destination path:", "Move");
                if (!string.IsNullOrEmpty(destination))
                {
                    try
                    {
                        string sourcePath = selectedItem.Tag.ToString();
                        Directory.Move(sourcePath, destination);

                        // Удаляем элемент из дерева
                        var parent = GetParentTreeViewItem((TreeViewItem)((FrameworkElement)e.OriginalSource).TemplatedParent);
                        if (parent != null)
                        {
                            parent.Items.Remove(FileTree.SelectedItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void ListBox_Move_Click(object sender, RoutedEventArgs e)
        {
            if (UserButtons_ListBox.SelectedItem != null)
            {
                string sourcePath = UserButtons_ListBox.SelectedItem.ToString();
                string destination = Microsoft.VisualBasic.Interaction.InputBox("Enter destination path:", "Move");

                if (!string.IsNullOrEmpty(destination))
                {
                    try
                    {
                        if (Directory.Exists(sourcePath))
                        {
                            Directory.Move(sourcePath, destination);
                        }
                        else if (File.Exists(sourcePath))
                        {
                            File.Move(sourcePath, Path.Combine(destination, Path.GetFileName(sourcePath)));
                        }

                        // Удаляем элемент из ListBox
                        UserButtons_ListBox.Items.Remove(UserButtons_ListBox.SelectedItem);

                        MessageBox.Show("Перемещено успешно.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void Properties_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is DtoItem selectedItem)
            {
                FileInfo fileInfo = null;
                DirectoryInfo dirInfo = null;

                if (selectedItem.ItemKind == Kind.File)
                {
                    fileInfo = new FileInfo(selectedItem.Tag.ToString());
                    MessageBox.Show($"File: {fileInfo.FullName}\nSize: {fileInfo.Length} bytes\nCreation Time: {fileInfo.CreationTime}");
                }
                else if (selectedItem.ItemKind == Kind.Directory)
                {
                    dirInfo = new DirectoryInfo(selectedItem.Tag.ToString());
                    MessageBox.Show($"Directory: {dirInfo.FullName}\nCreation Time: {dirInfo.CreationTime}");
                }
            }
        }

        private void ListBox_Properties_Click(object sender, RoutedEventArgs e)
        {
            if (UserButtons_ListBox.SelectedItem != null)
            {
                string selectedPath = UserButtons_ListBox.SelectedItem.ToString();

                try
                {
                    if (Directory.Exists(selectedPath))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(selectedPath);
                        MessageBox.Show($"Directory: {dirInfo.FullName}\n" +
                                        $"Creation Time: {dirInfo.CreationTime}\n" +
                                        $"Last Write Time: {dirInfo.LastWriteTime}");
                    }
                    else if (File.Exists(selectedPath))
                    {
                        FileInfo fileInfo = new FileInfo(selectedPath);
                        MessageBox.Show($"File: {fileInfo.FullName}\n" +
                                        $"Size: {fileInfo.Length} bytes\n" +
                                        $"Creation Time: {fileInfo.CreationTime}\n" +
                                        $"Last Write Time: {fileInfo.LastWriteTime}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private void AddToFeatured_Click(object sender,RoutedEventArgs e)
        {
            if(FileTree.SelectedItem is DtoItem selectedItem)
            {
                FileInfo fileInfo = null;
                
                if(selectedItem.ItemKind == Kind.File)
                {
                    fileInfo = new FileInfo(selectedItem.Tag.ToString());
                    FeaturedFiles.Add(fileInfo);
                    MessageBox.Show($"File {fileInfo.FullName} добавлен в избранные файлы");
                }
                FeaturedFiles_Save();
            }
        }
        private void ListBox_SearchByName_Click(object sender, RoutedEventArgs e)
        {
            string fileName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя файла для поиска:", "Search by Name");

            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    ObservableCollection<string> searchResults = new ObservableCollection<string>();
                    foreach (string path in UserButtons_ListBox.Items)
                    {
                        if (Path.GetFileName(path).Contains(fileName))
                        {
                            searchResults.Add(path);
                        }
                    }

                    if (searchResults.Count > 0)
                    {
                        UserButtons_ListBox.ItemsSource = searchResults; // Отображаем результаты поиска
                    }
                    else
                    {
                        MessageBox.Show("Файлы не найдены.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void ListBox_SearchByExtension_Click(object sender, RoutedEventArgs e)
        {
            string extension = Microsoft.VisualBasic.Interaction.InputBox("Введите расширение файла (например, .txt):", "Search by Extension");

            if (!string.IsNullOrEmpty(extension))
            {
                try
                {
                    ObservableCollection<string> searchResults = new ObservableCollection<string>();
                    foreach (string path in UserButtons_ListBox.Items)
                    {
                        if (Path.GetExtension(path).Equals(extension, StringComparison.OrdinalIgnoreCase))
                        {
                            searchResults.Add(path);
                        }
                    }

                    if (searchResults.Count > 0)
                    {
                        UserButtons_ListBox.ItemsSource = searchResults; // Отображаем результаты поиска
                    }
                    else
                    {
                        MessageBox.Show("Файлы не найдены.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        /*private void ListBox_Archive_Click(object sender, RoutedEventArgs e)
        {
            if (UserButtons_ListBox.SelectedItem != null)
            {
                string archiveName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя архива (без расширения):", "Archive");
                string archivePath = Path.Combine(Path.GetDirectoryName(UserButtons_ListBox.SelectedItem.ToString()), $"{archiveName}.zip");

                try
                {
                    ZipFile.CreateFromDirectory(Path.GetDirectoryName(UserButtons_ListBox.SelectedItem.ToString()), archivePath);
                    MessageBox.Show($"Файлы успешно архивированы: {archivePath}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }*/

        private void ListBox_SearchByDate_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate, endDate;

            string startInput = Microsoft.VisualBasic.Interaction.InputBox("Введите начальную дату (гггг-мм-дд):", "Search by Date");
            if (!DateTime.TryParse(startInput, out startDate))
            {
                MessageBox.Show("Неверный формат даты.");
                return;
            }

            string endInput = Microsoft.VisualBasic.Interaction.InputBox("Введите конечную дату (гггг-мм-дд):", "Search by Date");
            if (!DateTime.TryParse(endInput, out endDate))
            {
                MessageBox.Show("Неверный формат даты.");
                return;
            }

            try
            {
                ObservableCollection<string> searchResults = new ObservableCollection<string>();
                foreach (string path in UserButtons_ListBox.Items)
                {
                    if (File.Exists(path))
                    {
                        FileInfo fileInfo = new FileInfo(path);
                        if (fileInfo.CreationTime >= startDate && fileInfo.CreationTime <= endDate)
                        {
                            searchResults.Add(path);
                        }
                    }
                }

                if (searchResults.Count > 0)
                {
                    UserButtons_ListBox.ItemsSource = searchResults; // Отображаем результаты поиска
                }
                else
                {
                    MessageBox.Show("Файлы не найдены.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Настройки
        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = true;
        }

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            Resources["LightThemeBackground"] = new SolidColorBrush(Color.FromArgb(255, 147, 255, 253));
            Resources["LightThemeForeground"] = new SolidColorBrush(Color.FromArgb(255, 0, 156, 255));
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            Resources["LightThemeBackground"] = new SolidColorBrush(Color.FromArgb(255, 33, 33, 33));
            Resources["LightThemeForeground"] = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = false;
        }
        private void FeaturedFiles_Save()
        {
            string PathFile = "FeaturedFiles.txt";
            List<string> lines = new List<string>();
            foreach (FileInfo fileInfo in FeaturedFiles)
            {
                lines.Add(fileInfo.FullName);   
            }
            File.WriteAllLines(PathFile, lines);
        }
        private void FeaturedFiles_Read()
        {
            string PathFile = "FeaturedFiles.txt";
            string[] FeaturedFilesArr = File.ReadAllLines(PathFile);
            foreach (string FeaturedFile in FeaturedFilesArr) 
            { 
                FileInfo file = new FileInfo(FeaturedFile);
                FeaturedFiles.Add(file);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] files = File.ReadAllLines("FeaturedFiles.txt");
            UserButtons_ListBox.ItemsSource = files;
            /*foreach (FileInfo fileInfo in FeaturedFiles)
            {
                string text = fileInfo.FullName;
                UserButtons_ListBox.Items.Add(text);
            }*/
        }

        private void Downloads_Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем путь к папке "Загрузки"
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";

            // Проверяем, существует ли папка "Загрузки"
            if (Directory.Exists(downloadsPath))
            {
                try
                {
                    // Получаем список всех файлов в папке "Загрузки"
                    string[] files = Directory.GetFiles(downloadsPath);

                    // Создаем коллекцию для хранения полных путей файлов
                    ObservableCollection<string> filePaths = new ObservableCollection<string>();

                    // Добавляем полные пути файлов в коллекцию
                    foreach (string file in files)
                    {
                        filePaths.Add(file);
                    }

                    // Привязываем коллекцию к ListBox
                    UserButtons_ListBox.ItemsSource = filePaths;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Нет доступа к папке 'Загрузки': {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файлов: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Папка 'Загрузки' не найдена.");
            }
        }

        private void Music_Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем путь к папке "Музыка"
            string musicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            // Проверяем, существует ли папка "Музыка"
            if (Directory.Exists(musicPath))
            {
                try
                {
                    // Создаем коллекцию для хранения полных путей файлов
                    ObservableCollection<string> filePaths = new ObservableCollection<string>();

                    // Получаем список всех файлов в папке "Музыка"
                    var files = Directory.GetFiles(musicPath, "*.*", SearchOption.AllDirectories);

                    // Добавляем полные пути файлов в коллекцию
                    foreach (var file in files)
                    {
                        filePaths.Add(file);
                    }

                    // Привязываем коллекцию к ListBox
                    UserButtons_ListBox.ItemsSource = filePaths;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Нет доступа к папке 'Музыка': {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файлов: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Папка 'Музыка' не найдена.");
            }
        }

        private void Pictures_Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем путь к папке "Фото"
            string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            // Проверяем, существует ли папка "Фото"
            if (Directory.Exists(picturesPath))
            {
                try
                {
                    // Создаем коллекцию для хранения полных путей файлов
                    ObservableCollection<string> filePaths = new ObservableCollection<string>();

                    // Получаем список всех файлов в папке "Фото", включая поддиректории
                    var files = Directory.GetFiles(picturesPath, "*.*", SearchOption.AllDirectories);

                    // Добавляем полные пути файлов в коллекцию
                    foreach (var file in files)
                    {
                        filePaths.Add(file);
                    }

                    // Привязываем коллекцию к ListBox
                    UserButtons_ListBox.ItemsSource = filePaths;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Нет доступа к папке 'Фото': {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файлов: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Папка 'Фото' не найдена.");
            }
        }
    }
}