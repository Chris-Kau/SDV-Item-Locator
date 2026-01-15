using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Buildings;

namespace Item_Locator
{
    public class FindContainers
    {
        /// <summary>
        /// Returns a list of tiles that have a Chest containing the specified item
        /// </summary>
        public static List<Vector2> get_container_locs(GameLocation location, string i)
        {
            List<Vector2> chest_locs = new();
            if (location is null || string.IsNullOrWhiteSpace(i))
                return chest_locs;
            
            Vector2? farmFridge = getHouseFridgeTile(location, i);
            List<Vector2> junimoHutsLocations = getJunimoHutTiles(location, i);
            //add fridges and junimo huts to the container locations
            if (farmFridge != null)
                chest_locs.Add((Vector2)farmFridge);
            if (junimoHutsLocations != null)
                chest_locs.AddRange(junimoHutsLocations);

            foreach (var pair in location.Objects.Pairs)
            {
                Vector2 tile = pair.Key;

                if (pair.Value is not StardewValley.Objects.Chest chest)
                    continue;
                // Junimo Chests
                if (string.Equals(chest.Name, "Junimo Chest", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(chest.name, "Junimo Chest", StringComparison.OrdinalIgnoreCase))
                {
                    var team = Game1.player?.team;
                    var inv = team?.GetOrCreateGlobalInventory("JunimoChests");
                    if (inv is null) continue;

                    foreach (var a in inv)
                    {
                        if (a is null) continue;
                        if (string.Equals(i, a.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            chest_locs.Add(tile);
                            break;
                        }
                    }
                }
                else // Any other chest objects
                {
                    var items = chest.Items;
                    if (items is null) continue;
                    foreach (var a in items)
                    {
                        if (a is null) continue;
                        if (string.Equals(i, a.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            chest_locs.Add(tile);
                            break;
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
