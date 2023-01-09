using System;
using System.IO;
using NUnit.Framework;
using QuestPDF.Drawing;

namespace QuestPDF.UnitTests
{
    public class FontManagerTests
    {
        [Test]
        public void LoadFontFromFile()
        {
            using var stream = File.OpenRead("Resources/FontContent.ttf"); 
            FontManager.RegisterFont(stream);
        }
        
        [Test]
        public void LoadFontFromEmbeddedResource()
        {
            FontManager.RegisterFontFromEmbeddedResource("QuestPDF.UnitTests.Resources.FontEmbeddedResource.ttf");
        }
        
        [Test]
        public void LoadFontFromEmbeddedResource_ShouldThrowException_WhenResourceIsNotAvailable()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                FontManager.RegisterFontFromEmbeddedResource("QuestPDF.UnitTests.WrongPath.ttf");
            });
        }
    }
}