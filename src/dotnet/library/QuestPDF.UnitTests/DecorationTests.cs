using System;
using NUnit.Framework;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

[TestFixture]
public class DecorationTests
{
    #region Fluent API

    [TestCase("Before")]
    [TestCase("Content")]
    [TestCase("After")]
    public void SlotCannotBeDefinedTwice(string slotName)
    {
        var exception = Assert.Throws<DocumentComposeException>(() =>
        {
            EmptyContainer
                .Create()
                .Decoration(decoration =>
                {
                    DefineSlot(decoration);
                    DefineSlot(decoration);
                });
        });

        Assert.That(exception.Message, Is.EqualTo($"The 'Decoration.{slotName}' layer has already been defined. Please call this method only once."));

        void DefineSlot(DecorationDescriptor decoration)
        {
            _ = slotName switch
            {
                "Before" => decoration.Before(),
                "Content" => decoration.Content(),
                "After" => decoration.After(),
                _ => throw new ArgumentOutOfRangeException(nameof(slotName))
            };
        }
    }

    #endregion
}
