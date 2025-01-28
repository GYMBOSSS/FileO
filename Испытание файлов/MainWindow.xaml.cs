using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace Испытание_файлов
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MediaFiles mediaFiles = new MediaFiles();
            if (FileTree.SelectedItem != null)
            {
                TreeViewItem treeViewItem = FileTree.SelectedItem as TreeViewItem;
                temp.Text = GetPath(treeViewItem);
                if (treeViewItem.Items.Count == 0) 
                {
                    mediaFiles.path = temp.Text;
                    mediaFiles.OpenFile(filegrid);
                }
            }
        }

        private string GetPath(TreeViewItem treeViewItem)
        {
            string path = "";
            if (treeViewItem.Parent as TreeViewItem != null)
            {
                path = GetPath(treeViewItem.Parent as TreeViewItem) + "\\" + treeViewItem.Header + path;
            }
            else
            {
                path = "C:\\" + path + treeViewItem.Header as string;
            }
            return path;
        }

        abstract class FILEO
        {
            public string path;
            public string name;

            abstract public void OpenFile(Grid filegrid);
            /*abstract public void PlayFile();*/
        }

        class MediaFiles : FILEO
        {
            MediaElement mediaElement;
            public override void OpenFile(Grid filegrid)
            {
                /*mediaElement.PointToScreen();*/
                mediaElement = new MediaElement();
                mediaElement.LoadedBehavior = MediaState.Manual;
                mediaElement.Stretch = System.Windows.Media.Stretch.None;
                Grid.SetColumn(mediaElement, 2);
                filegrid.Children.Add(mediaElement);
                mediaElement.Source = new Uri(path);
                mediaElement.Play();
            }

            /*public override void PlayFile() 
            {
                mediaElement.Play();
            }*/

        }
    }
}
