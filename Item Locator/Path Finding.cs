using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

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
        public static Dictionary<Vector2, List<Vector2>> genAdjMatrix(Vector2 target)
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
            empty_tiles.Add(target);
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

            return adj_list;  
        }
        public static List<List<Vector2>> FindPathBFS(Dictionary<Vector2, List<Vector2>> adjlist, List<Vector2> targets, Vector2 playerLocation)
        {
            var start = playerLocation;
            var paths = new List<List<Vector2>>();
            var target = targets[0];
            //Console.WriteLine("Target: " + target + target.X + target.Y);
            var previous = solve(start, target, adjlist);
 /*           foreach(var key in previous.Keys)
            {
                Console.WriteLine("PREVIOUS!! " + key + previous[key]);
            }*/
            paths.Add(reconstructPath(start, target, previous));
/*            foreach(var tile in paths[0])
            {
                Console.WriteLine($"Path!: {tile}");
            }*/
            return paths;
           
        }

        private static Dictionary<Vector2, Vector2?> solve(Vector2 start, Vector2 target, Dictionary<Vector2, List<Vector2>> adjlist)
        {
            //List<Vector2> path = new();
            Queue<Vector2> queue = new Queue<Vector2>();
            queue.Enqueue(start);
            Dictionary<Vector2, bool> visited = new();
            Dictionary<Vector2, Vector2?> prev = new(); // prev helps us reconstruct path, tracks who the parent of a tile is
            //set all visited tiles to false except for the start (player's location) tile
            //and set all prev tiles to null 
            foreach (var tile in adjlist.Keys)
            {
                if (tile == start){
                    visited[start] = true;
                }else
                {
                    visited[tile] = false;
                }
                prev[tile] = null;
            }
            prev[target] = null;
            while(queue.Count > 0)
            {
                Vector2 node = queue.Dequeue();
                List<Vector2> neighbors = adjlist[node];
                foreach (var neighbor in neighbors)
                {
                    //Console.WriteLine("NEIGHBOR!! " + neighbor);
                    if (visited[neighbor] == false)
                    {
                        queue.Enqueue(neighbor);
                        visited[neighbor] = true;
                        prev[neighbor] = node;
                    }
                }
            }
            //Console.WriteLine("Successfully return prev in solve()");
            return prev;
        }

        private static List<Vector2> reconstructPath(Vector2 start, Vector2 end, Dictionary<Vector2, Vector2?> prev)
        {
            List<Vector2> path = new();
            for(Vector2? e = end; e != null; e = prev[e.Value] ?? null)
            {
                if (e == null)
                {
                    break;
                }
                Console.WriteLine($"E: {e}");
                path.Add(e.Value);
            }
            path.Reverse();

            if (path[0] == start)
            {
                //Console.WriteLine("Successfully returned path in reconstructPath()");
                return path;
            }
            //Console.WriteLine("returned empty path in reconstructPath()");
            return new List<Vector2>();
        }
    }
}
