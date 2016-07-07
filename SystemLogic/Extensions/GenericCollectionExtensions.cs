using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemLogic.Extensions
{
    public static class GenericCollectionExtensions
    {
        public static Dictionary<string, string> BuildDictionary(this List<KeyValuePair<string, string>> entries)
        {
            Dictionary<string, string> argDictionary = new Dictionary<string, string>();
            entries.ForEach(e => argDictionary.Add(e.Key, e.Value));

            return argDictionary;
        }
    }
}
