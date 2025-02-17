using FileO.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FileO.ViewModels
{
    public class TreeViewModel
    {
        public ICollectionView View => _cvs.View;
        public ObservableCollection<DtoItem> Items { get; private set; } = new ObservableCollection<DtoItem>();
        private CollectionViewSource _cvs = new CollectionViewSource();

        public TreeViewModel()
        {
            _cvs.Source = Items;
        }

        public void Load(string driveName)
        {
            Items.Clear();
            LoadFolderAsync(new DirectoryInfo(driveName), Items);
        }

        private async Task LoadFolderAsync(DirectoryInfo dir, ObservableCollection<DtoItem> col, int maxDepth = 5, int currentDepth = 0)
        {
            if (currentDepth > maxDepth) return;

            try
            {
                var dto = new DtoItem(dir);
                col.Add(dto);

                foreach (var subDir in dir.GetDirectories())
                {
                    await LoadFolderAsync(subDir, dto.Children, maxDepth, currentDepth + 1);
                }

                foreach (var file in dir.GetFiles())
                {
                    dto.Children.Add(new DtoItem(file));
                }
            }
            catch (UnauthorizedAccessException)
            {
                //MessageBox.Show($"Нет доступа к каталогу {dir.FullName}");
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Ошибка при загрузке содержимого: {ex.Message}");
            }
        }
    }
}