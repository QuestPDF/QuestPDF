using System.Collections.Generic;
using System.IO;

namespace QuestPdfSchedulerExample.icons
{
    public static class IconReader
    {
        public static Dictionary<string, byte[]> ReadIcons()
        {
            Dictionary<string, byte[]> icons = new();

            DirectoryInfo Folder;
            FileInfo[] Images;

            Folder = new DirectoryInfo("png");
            Images = Folder.GetFiles();

            foreach (var imagepath in Images)
            {
                icons[Path.GetFileNameWithoutExtension(imagepath.Name)] = File.ReadAllBytes(imagepath.ToString());
            }

            return icons;
        }
    }
}
