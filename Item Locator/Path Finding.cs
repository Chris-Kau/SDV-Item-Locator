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
                    //checks to see if the tile x,y is on the map and if the player is able to walk through it.
                    if (location.isTileOnMap(new Vector2(x,y)) && !location.IsTileBlockedBy(new Vector2(x, y),CollisionMask.All, CollisionMask.All, true))
                    {
                        Empty_Tiles.Add(new Vector2(x, y));
                    }
                }
            }
            return Empty_Tiles;
        }



        public class Point
        {
            public int X; public int Y;
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
            public override bool Equals(object obj)
            {
                if (obj is Point point)
                {
                    return X == point.X && Y == point.Y; ;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(X,Y);
            }
        }

        // public static double ComputeEuclideanDist(Point a, Point b)
        public static int[,] genAdjMatrix(List<Point> points, double threshold)
        {
            int n = points.Count;
            int[,] graph = new int[n, n]; // automatically filled with 0s



            return graph;
        }


    }
}
