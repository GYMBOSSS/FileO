using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileO
{
    internal class LBItem
    {
        public string Name { get; set; }
        public string IconPath {  get; set; }

        public LBItem(string name, string iconPath)
        {
            Name = name;
            IconPath = iconPath;
        }
    }
}
