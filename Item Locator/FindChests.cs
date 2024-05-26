using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;

namespace Item_Locator
{
    public class FindChests
    {
        public static List<Vector2> get_chest_locs(GameLocation location, Item i)
        {
            List<Vector2> chest_locs = new();
            //first 2 for loops loop through all tiles on player's location map
            for(int x = 0; x < location.map.Layers[0].LayerWidth; x++)
            {
                for(int y = 0; y < location.map.Layers[0].LayerHeight; y++)
                {
                    //checks to see if the x,y is a valid tile and checks to see if there is a chest object on said tile.
                    if(location.objects.ContainsKey(new Vector2(x, y)) && location.Objects[new Vector2(x,y)] is StardewValley.Objects.Chest chest)
                    {
                        //if we found a match in item names, add it to the chest_locs list
                        foreach(Item a in chest.Items)
                        {
                            if (i.Name == a.Name)
                            {
                                chest_locs.Add(new Vector2(x, y));
                                break;
                            }
                        }
                    }
                }
            }
            return chest_locs;
        }
    }
}
