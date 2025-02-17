using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FileO.Models;
using FileO.ViewModels;

namespace FileO
{
    public partial class MainWindow : Window
    {
        private readonly TreeViewModel _treeViewModel;

        public ObservableCollection<LBItem> Items { get; set; }
        public DriveInfo CurrentDrive { get; set; }

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
    }
}