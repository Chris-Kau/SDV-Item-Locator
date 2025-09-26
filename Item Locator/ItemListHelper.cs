using StardewValley;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Item_Locator
{
    public class ItemListHelper
    {
        public static SortedSet<string> GetAllItemNames()
        {
            SortedSet<string> allItemNames = new(StringComparer.OrdinalIgnoreCase);
            foreach (IItemDataDefinition itemType in ItemRegistry.ItemTypes)
            {

                foreach (string id in itemType.GetAllIds())
                {
                    string? temp = ItemRegistry.Create(id).DisplayName;
                    if (temp != null)
                        allItemNames.Add(temp);
                }
            }
            return allItemNames;
        }

        private static int LowerBound(List<string> words, string prefix)
        {
            int low = 0;
            int high = words.Count;
            while(low < high)
            {
                int mid = (low + high) / 2;
                if (String.Compare(words[mid], prefix, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }
            return low;
        }

        private static int UpperBound(List<string> words, string prefix)
        {
            int low = 0;
            int high = words.Count;
            while (low < high)
            {
                int mid = (low + high) / 2;
                if (words[mid].StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
                    String.Compare(words[mid], prefix, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }
            return low;
        }

        public static Tuple<int,int> GetRange(List<string> words, string prefix)
        {
            prefix = prefix.ToLower();
            int lower = LowerBound(words, prefix);
            int higher = UpperBound(words, prefix);
            return Tuple.Create(lower, higher);
        }
    }
}
