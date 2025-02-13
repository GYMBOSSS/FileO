using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileO
{
    public class LBItem
    {
        public DriveInfo ThisDrive { get; set; }
        public string IconPath {  get; set; }

        public LBItem(DriveInfo drive, string iconPath)
        {
            ThisDrive = drive;
            IconPath = iconPath;
        }
        public string DriveName => ThisDrive.Name;
    }
}
