using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace Task10.Models
{
    public class Folder
    {
        public DirectoryInfo DirInfo { get; private set; }
        public DirectoryInfo[] Directories => DirInfo.GetDirectories();
        public FileInfo[] Files => DirInfo.GetFiles();
        private static Dictionary<string, Folder> _folders = new Dictionary<string, Folder>();

        private Folder(string fullName)
        {
            DirInfo = new DirectoryInfo(fullName);
            TreeViewItem rew = new TreeViewItem();

        }

        public static Folder GetNewFolder(string fullName)
        {
            if (_folders.ContainsKey(fullName))
            {
                return _folders[fullName];
            }

            var folder = new Folder(fullName);
            _folders.Add(fullName, folder);

            return folder;
        }
    }
}
