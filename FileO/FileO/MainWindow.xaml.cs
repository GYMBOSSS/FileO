using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private void FileTree_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (FileTree.SelectedItem is TreeViewItem selectedTreeViewItem)
            {
                if (selectedTreeViewItem.Tag is DriveInfo || selectedTreeViewItem.Tag is DirectoryInfo)
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
        }

        // Контекстное меню
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is TreeViewItem item && item.Tag is DirectoryInfo dirInfo)
            {
                Process.Start("explorer.exe", dirInfo.FullName);
            }
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is TreeViewItem item && item.Tag is DirectoryInfo dirInfo)
            {
                string oldPath = dirInfo.FullName;
                string newPath = Microsoft.VisualBasic.Interaction.InputBox("Enter new name:", "Rename", Path.GetFileName(oldPath));
                if (!string.IsNullOrEmpty(newPath))
                {
                    Directory.Move(oldPath, Path.Combine(Path.GetDirectoryName(oldPath), newPath));
                    item.Header = newPath;
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is TreeViewItem item && item.Tag is DirectoryInfo dirInfo)
            {
                try
                {
                    Directory.Delete(dirInfo.FullName, true);
                    FileTree.Items.Remove(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (FileTree.SelectedItem is TreeViewItem item && item.Tag is DirectoryInfo dirInfo)
            {
                string destination = Microsoft.VisualBasic.Interaction.InputBox("Enter destination path:", "Copy");
                if (!string.IsNullOrEmpty(destination))
                {
                    try
                    {
                        Directory.CreateDirectory(destination);
                        foreach (var file in dirInfo.GetFiles())
                        {
                            file.CopyTo(Path.Combine(destination, file.Name), true);
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
            if (FileTree.SelectedItem is TreeViewItem item && item.Tag is DirectoryInfo dirInfo)
            {
                string destination = Microsoft.VisualBasic.Interaction.InputBox("Enter destination path:", "Move");
                if (!string.IsNullOrEmpty(destination))
                {
                    try
                    {
                        Directory.Move(dirInfo.FullName, destination);
                        FileTree.Items.Remove(item);
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
            if (FileTree.SelectedItem is TreeViewItem item && item.Tag is DirectoryInfo dirInfo)
            {
                MessageBox.Show($"Directory: {dirInfo.FullName}\nCreation Time: {dirInfo.CreationTime}");
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