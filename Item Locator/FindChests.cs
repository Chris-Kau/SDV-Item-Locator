using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace Item_Locator
{
    public class FindChests
    {
        /// <summary>
        /// Returns a list of tiles that have a Chest containing the specified item
        /// </summary>
        public static List<Vector2> get_chest_locs(GameLocation location, string i)
        {
            List<Vector2> chest_locs = new();
            Vector2? farmFridge = getHouseFridgeItems(location, i);
            if (farmFridge != null)
                chest_locs.Add((Vector2)farmFridge);
            //first 2 for loops loop through all tiles on player's location map
            for (int x = 0; x < location.map.Layers[0].LayerWidth; x++)
            {
                for(int y = 0; y < location.map.Layers[0].LayerHeight; y++)
                {
                    //checks to see if the there is an object at x,y and checks to see if there is a chest object on said tile.
                    if(location.objects.ContainsKey(new Vector2(x, y)) && location.Objects[new Vector2(x,y)] is StardewValley.Objects.Chest chest)
                    {
                       //if we found a match in item names, add it to the chest_locs list
                       if(chest.name == "Junimo Chest")
                        {
                            foreach (var a in Game1.player.team.GetOrCreateGlobalInventory("JunimoChests"))
                            {
                                if (i.Equals(a.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    chest_locs.Add(new Vector2(x, y));
                                    break;
                                }
                            }
                        }else
                        {
                            foreach (Item a in chest.Items)
                            {
                                if (i.Equals(a.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    chest_locs.Add(new Vector2(x, y));
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            return chest_locs;
        }

        private static Vector2? getHouseFridgeItems(GameLocation playerloc, string i)
        {
            GameLocation farmhouse = Game1.getLocationFromName("FarmHouse");
            GameLocation islandFarmHouse = Game1.getLocationFromName("IslandFarmHouse");
            if(playerloc == farmhouse || playerloc == islandFarmHouse)
            {
                if (playerloc.GetFridgePosition() != null)
                {
                    foreach(Item a in playerloc.GetFridge().Items)
                    {
                        if (i.Equals(a.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            return new Vector2(playerloc.GetFridgePosition().Value.X, playerloc.GetFridgePosition().Value.Y);
                        }
                    }
                }
            }

            return null;
        }
    }
}
