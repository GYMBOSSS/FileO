using FileO.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
            LoadFolder(new DirectoryInfo(driveName), Items);
        }

        private void LoadFolder(DirectoryInfo dir, ObservableCollection<DtoItem> col)
        {
            try
            {
                var dto = new DtoItem(dir);
                col.Add(dto);

                // Загрузка поддиректорий
                foreach (var subDir in dir.GetDirectories())
                {
                    LoadFolder(subDir, dto.Children);
                }

                // Загрузка файлов
                foreach (var file in dir.GetFiles())
                {
                    dto.Children.Add(new DtoItem(file));
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show($"Нет доступа к каталогу {dir.FullName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке содержимого: {ex.Message}");
            }
        }
    }
}