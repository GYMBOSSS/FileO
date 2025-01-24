using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileO
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] arr = new string[5] {"Первый","Второй","Третий","Четвёртый","Пятый"};
        ObservableCollection<LBItem> Items;
        public MainWindow()
        {
            InitializeComponent();
            Items = new ObservableCollection<LBItem> 
            {
                new LBItem(arr[0],"Icons/BaseIcon.png"),
                new LBItem(arr[1],"Icons/BaseIcon.png"),
                new LBItem(arr[2],"Icons/BaseIcon.png"),
                new LBItem(arr[3],"Icons/BaseIcon.png"),
                new LBItem(arr[4],"Icons/BaseIcon.png")
            };
            Drives.ItemsSource = Items;
        }
    }
}
