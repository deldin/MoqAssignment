using System;
using System.Xml.Linq;

namespace Ista.FileServices.Infrastructure.Extensions
{
    public static class XmlExtensions
    {
        public static string GetChildText(this XContainer container, XName name)
        {
            var element = container.Element(name);
            if (element == null)
                return string.Empty;

            return (string)element;
        }

        public static void TryAddElement(this XContainer container, XName name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            container.Add(new XElement(name, value));
        }

        public static void TryAddElement(this XContainer container, XName name, string value, Func<string, string> command)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            var result = command(value);
            container.Add(new XElement(name, result));
        }

        public static void TryGetChildText(this XContainer container, XName name, Action<string> command)
        {
            var element = container.Element(name);
            if (element == null)
                return;

            command((string)element);
        }

        public static void TryGetChildInt(this XContainer container, XName name, Action<int> command)
        {
            var element = container.Element(name);
            if (element == null)
                return;

            var value = (string)element;
            if (string.IsNullOrWhiteSpace(value))
                return;

            int parsedValue;
            if (int.TryParse(value, out parsedValue))
                command(parsedValue);
        }

        public static void TryGetChildDecimal(this XContainer element, XName name, Action<decimal> command)
        {
            var child = element.Element(name);
            if (child == null)
                return;

            var value = (string)child;
            if (string.IsNullOrWhiteSpace(value))
                return;

            decimal parsedValue;
            if (decimal.TryParse(value, out parsedValue))
                command(parsedValue);
        }
    }
}
