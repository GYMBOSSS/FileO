using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace FileO.Models
{
    public enum Kind { File, Directory }

    public class DtoItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Kind ItemKind { get; private set; }
        public string Name { get; set; }
        public string Size { get; private set; }
        public ObservableCollection<DtoItem> Children { get; set; } = new ObservableCollection<DtoItem>();
        public object Tag { get; private set; } // Добавляем свойство Tag

        public DtoItem(FileInfo fileInfo)
        {
            Name = fileInfo.Name;
            Size = ToStringView(fileInfo.Length);
            ItemKind = Kind.File;
            Tag = fileInfo.FullName; // Сохраняем полный путь к файлу
        }

        public DtoItem(DirectoryInfo directoryInfo)
        {
            Name = directoryInfo.Name;
            Size = "0"; // Можно добавить логику для подсчета размера директории
            ItemKind = Kind.Directory;
            Tag = directoryInfo.FullName; // Сохраняем полный путь к директории
        }

        private string ToStringView(double size)
        {
            const double thousand = 1024.0;
            int counter = 0;
            do
            {
                size /= thousand;
                counter++;
            } while (size > thousand);

            string result = "";
            switch (counter)
            {
                case 1:
                    result = string.Format("{0:F2} Kb", size);
                    break;
                case 2:
                    result = string.Format("{0:F2} Mb", size);
                    break;
                case 3:
                    result = string.Format("{0:F2} Gb", size);
                    break;
                default:
                    result = string.Format("{0:F2} Bytes", size);
                    break;
            }

            return result;
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}