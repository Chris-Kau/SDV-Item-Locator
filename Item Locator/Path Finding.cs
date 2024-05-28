using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;

namespace Item_Locator
{
    public class Path_Finding
    {
        public static List<Vector2> Find_Empty_Tiles(GameLocation location)
        {
            List<Vector2> Empty_Tiles = new();
            for (int x = 0; x < location.map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < location.map.Layers[0].LayerHeight; y++)
                {
                    //checks to see if the x,y is a valid tile and checks to see if there is a chest object on said tile.
                    if (location.isTileOnMap(new Vector2(x,y)) && !location.IsTileBlockedBy(new Vector2(x, y),CollisionMask.All, CollisionMask.All, true))
                    {
                        Empty_Tiles.Add(new Vector2(x, y));
                    }
                }
            }
            return Empty_Tiles;
        }
    }
}
