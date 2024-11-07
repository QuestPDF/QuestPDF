using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QuestPDF.Qpdf;

class SimpleJsonPropertyNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

/// <summary>
/// Never use in performance critical scenarios!
/// </summary>
class SimpleJsonSerializer
{
    public static string Serialize(object obj)
    {
        if (obj == null) 
            return "null";

        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        var stringBuilder = new StringBuilder();
        stringBuilder.Append('{');

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            
            if (value == default)
                continue;
            
            var name = property.GetCustomAttribute<SimpleJsonPropertyNameAttribute>().Name;
            stringBuilder.Append($"\"{name}\": {SerializeValue(value)}");
            stringBuilder.Append(", ");
        }
        
        if (properties.Length > 1) 
            stringBuilder.Length -= 2;

        stringBuilder.Append('}');
        return stringBuilder.ToString();
    }

    private static string SerializeValue(object value)
    {
        if (value == null) 
            return "null";
        
        if (value is string) 
            return $"\"{value}\"";
        
        if (value is bool)
            return value.ToString().ToLower();
        
        if (value is IEnumerable<object> enumerable)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('[');

            foreach (var item in enumerable)
                stringBuilder.Append($"{SerializeValue(item)}, ");
            
            // remove trailing comma and space
            if (enumerable.Any()) 
                stringBuilder.Length -= 2;
            
            stringBuilder.Append(']');
            return stringBuilder.ToString();
        }
        
        if (!value.GetType().IsPrimitive)
            return Serialize(value);
        
        return value.ToString();
    }
}