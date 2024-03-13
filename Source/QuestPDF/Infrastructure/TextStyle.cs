using System;
using System.Reflection;
using QuestPDF.Helpers;

namespace QuestPDF.Infrastructure
{
    public record TextStyle
    {
        internal string? Color { get; set; }
        internal string? BackgroundColor { get; set; }
        internal string? FontFamily { get; set; }
        internal float? Size { get; set; }
        internal float? LineHeight { get; set; }
        internal float? LetterSpacing { get; set; }
        internal FontWeight? FontWeight { get; set; }
        internal FontPosition? FontPosition { get; set; }
        internal bool? IsItalic { get; set; }
        internal bool? HasStrikethrough { get; set; }
        internal bool? HasUnderline { get; set; }
        internal bool? WrapAnywhere { get; set; }
        internal TextDirection? Direction { get; set; }

        internal TextStyle? Fallback { get; set; }

        internal static TextStyle LibraryDefault { get; } = new()
        {
            Color = Colors.Black,
            BackgroundColor = Colors.Transparent,
            FontFamily = Fonts.Lato,
            Size = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = Infrastructure.FontWeight.Normal,
            FontPosition = Infrastructure.FontPosition.Normal,
            IsItalic = false,
            HasStrikethrough = false,
            HasUnderline = false,
            WrapAnywhere = false,
            Direction = TextDirection.Auto,
            Fallback = null
        };

        internal object? GetValue(TextStyleProperty property)
        {
            return GetPropertyEntry(property)?
                .GetValue(this);
        }

        internal void SetValue(TextStyleProperty property, object? value)
        {
            GetPropertyEntry(property)?
                .SetValue(this, value);
        }

        private PropertyInfo? GetPropertyEntry(TextStyleProperty property)
        {
            var propertyName = Enum.GetName(typeof(TextStyleProperty), property);

            if (propertyName is null)
                throw ProvidedPropertyTypeIsNotSupported(property);

            const BindingFlags bindingFlags = 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            
            return GetType()
                .GetProperty(propertyName, bindingFlags);
            
            static Exception ProvidedPropertyTypeIsNotSupported(TextStyleProperty property)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(property),
                    property,
                    "Expected to mutate the TextStyle object. Provided property type is not supported.");
            }
        }

        public static TextStyle Default { get; } = new();
    }
}