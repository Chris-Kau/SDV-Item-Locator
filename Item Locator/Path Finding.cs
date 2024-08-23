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
        public static Dictionary<Vector2, List<Vector2>> genAdjList(List<Vector2> targets)
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
            foreach(var target in targets)
            {
                empty_tiles.Add(target); //add the chest to the list of empty tiles to allow pathfind to it.
            }
            
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
        public static void GetPaths()
        {
            //get player tile location
            Random random = new Random();
            GameLocation playerloc = Game1.player.currentLocation;
            List<Vector2> validChestLocs;

            Vector2 playerTileLoc = Game1.player.Tile;

            //finds all chest locations that contain the searched item
            validChestLocs = FindChests.get_chest_locs(playerloc, CustomItemMenu.SearchedItem);

            if (validChestLocs.Count > 0)
            {
                Dictionary<Vector2, List<Vector2>> validEmptyTiles = Path_Finding.genAdjList(validChestLocs);
                ModEntry.paths = Path_Finding.FindPathBFS(validEmptyTiles, validChestLocs, playerTileLoc);
                //assign random color to each path
                foreach (var path in ModEntry.paths)
                {
                    ModEntry.pathColors[path] = new Color(random.Next(125, 256), random.Next(125, 256), random.Next(125, 256));
                }
                ModEntry.shouldDraw = true;
            }
            else
            {
                //if no paths are found, clear the list from previous paths so it doesnt draw it.
                ModEntry.paths.Clear();
                ModEntry.pathColors.Clear();
                ModEntry.shouldDraw = false;
            }
        }
        private static List<List<Vector2>> FindPathBFS(Dictionary<Vector2, List<Vector2>> adjlist, List<Vector2> targets, Vector2 playerLocation)
        {
            var start = playerLocation;
            var paths = new List<List<Vector2>>();
            //NEED TO MAKE CHANGES TO SUPPORT MULTIPLE TARGETS (CHESTS)
            //instead of target being the first value in targets, we need to
            //make a forloop that loops through each target and adds the path list to var paths.
            //var target = targets[0];
            foreach(var target in targets)
            {
                var previous = solve(start, target, adjlist);
                var reconstructedPath = reconstructPath(start, target, previous);
                paths.Add(reconstructedPath);
            }
            //NEED TO MAKE CHANGES IN ModEntry.cs DrawPath() to have a nested for loop to go through all list of path lists.
            return paths;
           
        }

        private static Dictionary<Vector2, Vector2?> solve(Vector2 start, Vector2 target, Dictionary<Vector2, List<Vector2>> adjlist)
        {

            Queue<Vector2> queue = new Queue<Vector2>();
            queue.Enqueue(start);
            Dictionary<Vector2, bool> visited = new(); //helps us keep track of visited tiles
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
            //set the prev node of the target to null becauase we have not found the path yet
            prev[target] = null;
            while(queue.Count > 0)
            {
                Vector2 node = queue.Dequeue();
                //adjlist is a dictionary Key : Value ==> Tile : NeighboringTiles
                List<Vector2> neighbors = adjlist[node];
                //loop through the tile's neighbors
                foreach (var neighbor in neighbors)
                {
                    //if that neighbor has not been visited, add it to the queue, set it to visited, and set the prev node of it to the the parenting tile.
                    if (visited[neighbor] == false)
                    {
                        queue.Enqueue(neighbor);
                        visited[neighbor] = true;
                        prev[neighbor] = node;
                    }
                }
            }
            return prev;
        }

        private static List<Vector2> reconstructPath(Vector2 start, Vector2 end, Dictionary<Vector2, Vector2?> prev)
        {
            List<Vector2> path = new();
            //go through the previous tiles from prev list until we hit null and add it the the path list
            for(Vector2? e = end; e != null; e = prev[e.Value] ?? null)
            {
                if (e == null)
                {
                    break;
                }
                path.Add(e.Value);
            }
            //reverse the path list so it goes from start -> end instead of end -> start
            path.Reverse();

            //return the path if a valid path was found, if not then return an empty list.
            if (path[0] == start)
            {
                return path;
            }
            return new List<Vector2>();
        }
    }
}
