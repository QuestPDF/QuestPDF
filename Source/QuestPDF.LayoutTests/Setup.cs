﻿namespace QuestPDF.LayoutTests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public static void Configure()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
    }
}