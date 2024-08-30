using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Buildings;

namespace Item_Locator
{
    public class FindChests
    {
        /// <summary>
        /// Returns a list of tiles that have a Chest containing the specified item
        /// </summary>
        public static List<Vector2> get_chest_locs(GameLocation location, string i)
        {
            getJunimoHutTiles(location, i);
            List<Vector2> chest_locs = new();
            Vector2? farmFridge = getHouseFridgeTile(location, i);
            List<Vector2> junimoHutsLocations = getJunimoHutTiles(location, i);
            //add fridges and junimo huts to the container locations
            if (farmFridge != null)
                chest_locs.Add((Vector2)farmFridge);
            if (junimoHutsLocations != null)
                chest_locs.AddRange(junimoHutsLocations);
            //first 2 for loops loop through all tiles on player's location map
            for (int x = 0; x < location.map.Layers[0].LayerWidth; x++)
            {
                for(int y = 0; y < location.map.Layers[0].LayerHeight; y++)
                {
                    //checks to see if the there is an object at x,y and checks to see if there is a chest object on said tile.
                    if(location.objects.ContainsKey(new Vector2(x, y)) && location.Objects[new Vector2(x,y)] is StardewValley.Objects.Chest chest)
                    {
                       //if we found a match in item names, add it to the chest_locs list

                        //Check for junimo chest
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
                            //check for other types of chests that arent junom chests
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

        /// <summary>
        /// Returns the location of the farmhouse fridge or ginger island farm house fridge
        /// </summary>
        private static Vector2? getHouseFridgeTile(GameLocation playerloc, string i)
        {
            GameLocation farmhouse = Game1.getLocationFromName("FarmHouse");
            GameLocation islandFarmHouse = Game1.getLocationFromName("IslandFarmHouse");
            //check to see if the player is in their farm house or their farm house on ginger island
            if(playerloc == farmhouse || playerloc == islandFarmHouse)
            {
                //check to see if there is a fridge because the starting house does not come with a fridge, if there are fridges, loop through its items
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
        /// <summary>
        /// Returns a list of the Junimo Huts' tiles if there exists Junimo Huts with the searched item
        /// </summary>
        private static List<Vector2> getJunimoHutTiles(GameLocation playerloc, string i)
        {
            List <Vector2> JunimoHutLocations = new();
            GameLocation playerFarm = Game1.getLocationFromName("Farm");
            Farm farm = Game1.getFarm();
            //make sure the player is on the farm
            if(playerloc == playerFarm)
            {
                //loop through all buildings on the player's farm
                foreach(Building building in farm.buildings)
                {
                    if(building is JunimoHut junimoHut)
                    {
                        //if there is a Junimo Hut, we loop through its items and look for the searched item.
                        foreach(Item item in junimoHut.GetOutputChest().Items)
                        {
                            if(i.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                //add 1 just so the tile is centered with the junimo hut since the junimo hut is a 3x2
                                JunimoHutLocations.Add(new Vector2(junimoHut.tileX.Value + 1, junimoHut.tileY.Value + 1));
                                break;
                            }
                        }
                    }
                }
            }
            return JunimoHutLocations;
        }
    }
}
