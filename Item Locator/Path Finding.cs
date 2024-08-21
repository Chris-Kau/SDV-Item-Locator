using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
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
        public static List<Vector2> genAdjMatrix()
        {
            /*int n = points.Count;
            int[,] graph = new int[n, n];*/ // automatically filled with 0s
                                            // note that this is diff from x by y table, it's a point by point table

            // assign a point to each index of graph somehow. dictionary or list probably so we know which is which
            // making the graph a point graph instead of an int graph doesnt work. those table nums are supposed to be ints, the indexes are still ints


            // no corners right. yeah.

            //key : value
            //[5,5] : [[5,4], [5,6], [4,5], [6,5]]

            //Creates adj_list with the key being the point and the value being all connected points
            GameLocation player_loc = Game1.player.currentLocation;
            Dictionary<Vector2, List<Vector2>> adj_list = new Dictionary<Vector2, List<Vector2>>();
            List<Vector2> empty_tiles = Find_Empty_Tiles(player_loc);
            foreach (Vector2 p in empty_tiles)
            {
                List<Vector2> temp = [];
                foreach (Vector2 o in empty_tiles)
                {
                    if ((new Vector2(p.X - 1, p.Y)) == o)
                    {
                        temp.Add(o);
                    }
                    if ((new Vector2(p.X + 1, p.Y)) == o)
                    {
                        temp.Add(o);
                    }
                    if ((new Vector2(p.X, p.Y + 1)) == o)
                    {
                        temp.Add(o);
                    }
                    if ((new Vector2(p.X, p.Y - 1)) == o)
                    {
                        temp.Add(o);
                    }

                    if (temp.Count == 4)
                    {
                        break;
                    }
                }
                adj_list.Add(p, temp);
            }

            //prints out for debugging/testing (REMOVE LATER)
/*            foreach (KeyValuePair<Vector2, List<Vector2>> pair in adj_list)
            {
                Console.Write($"{pair.Key.X}, {pair.Key.Y} : ");
                foreach(Vector2 point in pair.Value)
                {
                    Console.Write($"({point.X}, {point.Y}), ");
                }
                Console.WriteLine(' ');
            }*/
            return empty_tiles;  
        }
        public static List<Vector2> dijkstras(List<Vector2> adjlist, List<Vector2> targets, Vector2 playerLocation)
        {
            Vector2 start = playerLocation;
            List<Vector2> tilepath = new List<Vector2>();

            return tilepath;
        }
    }
}
