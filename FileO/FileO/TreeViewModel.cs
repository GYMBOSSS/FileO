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

        /// <summary>
        /// Загружает содержимое указанного диска или каталога.
        /// </summary>
        /// <param name="driveName">Путь к диску или каталогу.</param>
        public void Load(string driveName)
        {
            Items.Clear();
            _ = Task.Run(async () => await LoadFolderAsync(new DirectoryInfo(driveName), Items));
        }

        /// <summary>
        /// Асинхронно загружает содержимое каталога и его подкаталогов.
        /// </summary>
        /// <param name="dir">Каталог для загрузки.</param>
        /// <param name="col">Коллекция, в которую добавляются элементы.</param>
        /// <param name="maxDepth">Максимальная глубина рекурсии.</param>
        /// <param name="currentDepth">Текущая глубина рекурсии.</param>
        /// <returns></returns>
        private async Task LoadFolderAsync(DirectoryInfo dir, ObservableCollection<DtoItem> col, int maxDepth = 3, int currentDepth = 0)
        {
            if (currentDepth > maxDepth) return;

            try
            {
                // Проверяем, является ли текущий каталог системным
                if (IsSystemDirectory(dir.FullName))
                {
                    return; // Пропускаем системные каталоги
                }

                var dto = new DtoItem(dir);

                // Добавляем элемент в коллекцию через Dispatcher
                Application.Current.Dispatcher.Invoke(() => col.Add(dto));

                // Рекурсивно загружаем подкаталоги
                foreach (var subDir in dir.GetDirectories())
                {
                    if (IsSystemDirectory(subDir.FullName)) continue; // Пропускаем системные подкаталоги
                    await LoadFolderAsync(subDir, dto.Children, maxDepth, currentDepth + 1);
                }

                // Добавляем файлы из текущего каталога через Dispatcher
                foreach (var file in dir.GetFiles())
                {
                    Application.Current.Dispatcher.Invoke(() => dto.Children.Add(new DtoItem(file)));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Игнорируем ошибки доступа к каталогам
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке содержимого: {ex.Message}");
            }
        }

        /// <summary>
        /// Метод для проверки, является ли каталог системным.
        /// </summary>
        /// <param name="path">Путь к каталогу.</param>
        /// <returns>True, если каталог системный; иначе False.</returns>
        private bool IsSystemDirectory(string path)
        {
            // Список системных каталогов, которые нужно исключить
            var systemDirs = new[]
            {
                "Windows", "Program Files", "Program Files (x86)", "ProgramData",
                "System Volume Information", "Recovery", "MSOCache", "$Recycle.Bin"
            };

            // Проверяем, содержит ли путь одно из системных имен
            foreach (var dir in systemDirs)
            {
                if (path.IndexOf(dir, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }
    }
}