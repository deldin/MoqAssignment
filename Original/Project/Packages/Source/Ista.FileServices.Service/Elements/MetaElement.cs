using System.Collections.Generic;
using Ista.FileServices.Service.Enumerations;

namespace Ista.FileServices.Service.Elements
{
    public class MetaElement : AbstractElement
    {
        private readonly Dictionary<string, string> dictionary;
 
        public override ElementTypes ElementType
        {
            get { return ElementTypes.Meta; }
        }

        public bool HasEntries
        {
            get { return (dictionary.Count != 0); }
        }

        public MetaElement()
        {
            dictionary = new Dictionary<string, string>();
        }

        public void AddEntry(string key, string value)
        {
            dictionary[key] = value;
        }

        public bool TryGetEntry(string key, out string value)
        {
            return dictionary.TryGetValue(key, out value);
        }
    }
}
