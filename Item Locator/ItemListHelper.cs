using StardewValley;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using StardewValley.Extensions;
using StardewValley.GameData.BigCraftables;

namespace Item_Locator
{
    using SObject = StardewValley.Object;

    public class ItemListHelper
    {
        public static SortedSet<string> GetAllItemNames()
        {
            var names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            var objectDef = ItemRegistry.GetObjectTypeDefinition();
            // Base items:
            foreach (var def in ItemRegistry.ItemTypes)
            {
                foreach (string qid in def.GetAllIds())
                {
                    var it = ItemRegistry.Create(qid);

                    // Skip placeholder flavored bases; we'll add real flavored outputs below.
                    if (it is SObject so &&
                        (so.QualifiedItemId == "(O)DriedFruit"
                      || so.QualifiedItemId == "(O)DriedMushrooms"
                      || so.QualifiedItemId == "(O)SmokedFish"
                      || so.QualifiedItemId == "(O)SpecificBait"))
                        continue;

                    // Replace base Honey with Wild Honey.
                    // Handle both QID notations and legacy PSI check to be safe.
                    if (it is SObject honeyCandidate &&
                        (honeyCandidate.ParentSheetIndex == 340
                         || qid.EndsWith(")340", StringComparison.Ordinal)
                         || qid.EndsWith(":340", StringComparison.Ordinal)))
                    {
                        var wild = objectDef.CreateFlavoredHoney(null); // -> "Wild Honey"
                        if (wild != null && !string.IsNullOrEmpty(wild.DisplayName))
                            names.Add(wild.DisplayName);
                        continue; // don't add plain "Honey"
                    }

                    if (it != null && !string.IsNullOrEmpty(it.DisplayName))
                        names.Add(it.DisplayName);
                }
            }
            // Big Craftables
            var bigCraftables = ItemRegistry.GetTypeDefinition(ItemRegistry.type_bigCraftable);
            if (bigCraftables is not null)
            {
                foreach (ParsedItemData data in bigCraftables.GetAllData())
                {
                    names.Add(data.DisplayName);
                }
            }
            // Flavored / variant object names
            foreach (string qid in objectDef.GetAllIds())
            {
                if (ItemRegistry.Create(qid) is not SObject item)
                    continue;

                bool createdJuice = false;
                bool createdPickle = false;

                switch (item.Category)
                {
                    case SObject.FishCategory:
                        {
                            // Smoked fish + specific bait
                            AddName(objectDef.CreateFlavoredSmokedFish(item));
                            AddName(objectDef.CreateFlavoredBait(item));

                            // Roe + Aged Roe (skip sturgeon -> becomes Caviar elsewhere)
                            var roe = objectDef.CreateFlavoredRoe(item);
                            AddName(roe);
                            if (roe is SObject roeObj && item.QualifiedItemId != "(O)698")
                                AddName(objectDef.CreateFlavoredAgedRoe(roeObj));
                            break;
                        }

                    case SObject.FruitsCategory:
                        {
                            AddName(objectDef.CreateFlavoredWine(item));
                            AddName(objectDef.CreateFlavoredJelly(item));
                            if (item.QualifiedItemId != "(O)398") // raisins already an item
                                AddName(objectDef.CreateFlavoredDriedFruit(item));
                            break;
                        }

                    case SObject.GreensCategory:
                        {
                            AddName(objectDef.CreateFlavoredPickle(item)); createdPickle = true;

                            if (item.Edibility > 0 && !item.HasContextTag("edible_mushroom"))
                            {
                                AddName(objectDef.CreateFlavoredJuice(item));
                                createdJuice = true;
                            }
                            break;
                        }

                    case SObject.VegetableCategory:
                        {
                            AddName(objectDef.CreateFlavoredJuice(item)); createdJuice = true;
                            AddName(objectDef.CreateFlavoredPickle(item)); createdPickle = true;
                            break;
                        }

                    case SObject.flowersCategory:
                        {
                            AddName(objectDef.CreateFlavoredHoney(item)); // "<Flower> Honey"
                            break;
                        }
                }

                // Extra coverage via tags
                if (!createdJuice && item.HasContextTag("keg_juice"))
                    AddName(objectDef.CreateFlavoredJuice(item));

                if (!createdPickle && item.HasContextTag("preserves_pickle") &&
                    item.Category is not (SObject.GreensCategory or SObject.VegetableCategory))
                    AddName(objectDef.CreateFlavoredPickle(item));

                if (item.HasContextTag("edible_mushroom"))
                    AddName(objectDef.CreateFlavoredDriedMushroom(item));
            }

            // Ensure Wild Honey is present even if the base-loop logic didn’t trigger for some reason
            AddName(objectDef.CreateFlavoredHoney(null));

            return names;

            void AddName(Item? it)
            {
                if (it != null && !string.IsNullOrEmpty(it.DisplayName))
                    names.Add(it.DisplayName);
            }
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
