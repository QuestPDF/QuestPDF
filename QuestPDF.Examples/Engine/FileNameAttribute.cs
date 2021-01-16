using System;

namespace QuestPDF.Examples.Engine
{
    public class FileNameAttribute : Attribute
    {
        public string FileName { get; }

        public FileNameAttribute(string fileName)
        {
            FileName = fileName;
        }
    }
}