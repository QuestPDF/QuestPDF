using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

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
        
        [Test]
        public void LoadFontFromStream_ShouldThrowException_WhenDataIsNotFont()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            Assert.Throws<InvalidOperationException>(() => FontManager.RegisterFont(stream));
        }
        
        [Test]
        public void GetRegisteredFonts_ContainsLibraryDefaultFontAndManuallyRegisteredFont()
        {
            using var stream = File.OpenRead("Resources/FontContent.ttf");
            FontManager.RegisterFont(stream);
            
            var fonts = FontManager.GetRegisteredFonts();
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(fonts.Select(x => x.FamilyName), Is.SupersetOf(new[] { "Lato", "Libre Barcode 39" }));
                Assert.That(fonts.Select(x => x.PostScriptName), Is.All.Not.Null.And.All.Not.Empty);
                Assert.That(fonts.Select(x => x.Weight), Is.All.InRange(100, 900));
                
                Assert.That(fonts, Is.SupersetOf(new[]
                {
                    new FontInfo { FamilyName = "Lato", PostScriptName = "Lato-Regular", Weight = 400, IsItalic = false, IsVariable = false },
                    new FontInfo { FamilyName = "Lato", PostScriptName = "Lato-Bold", Weight = 700, IsItalic = false, IsVariable = false },
                    new FontInfo { FamilyName = "Lato", PostScriptName = "Lato-Italic", Weight = 400, IsItalic = true, IsVariable = false },
                }));
                
                Assert.That(fonts.Where(x => x.FamilyName == "Lato").Select(x => x.IsVariable), Is.All.False);
            }
        }
        
        [Test]
        public void GetRegisteredFonts_ListsFontRegisteredWithCustomName_UnderBothNames()
        {
            using var stream = File.OpenRead("Resources/FontContent.ttf");
            FontManager.RegisterFontWithCustomName("Custom Barcode Font", stream);
            
            var barcodeFaces = FontManager
                .GetRegisteredFonts()
                .Where(x => x.PostScriptName == "LibreBarcode39-Regular")
                .Select(x => x.FamilyName)
                .ToList();
            
            Assert.That(barcodeFaces, Does.Contain("Custom Barcode Font").And.Contain("Libre Barcode 39"));
        }
        
        [Test]
        public void GetSystemFonts_ReturnsValidEntries()
        {
            var fonts = FontManager.GetSystemFonts();
            
            Assert.That(fonts, Is.Not.Null);
            Assert.That(fonts.Select(x => x.FamilyName), Is.All.Not.Null.And.All.Not.Empty);
            Assert.That(fonts.Select(x => x.Weight), Is.All.InRange(100, 900));
        }
    }
}
