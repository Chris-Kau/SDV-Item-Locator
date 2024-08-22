using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Item_Locator
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        List<List<Vector2>> path = new();
        Texture2D? tileHighlight;
        bool shouldDraw = false;
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //Opens up the custom menu
            tileHighlight = helper.ModContent.Load<Texture2D>("/assets/tileColor.png");
   
            helper.Events.Input.ButtonPressed += this.OpenItemMenu;
            //resizes menu on window resize
            helper.Events.Display.WindowResized += this.resizeCustomMenu;
            helper.Events.Display.RenderedWorld += this.RenderedWorld;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        private void RenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if(path.Count > 0)
            {
                DrawPath(e, path);
            }

        }
        private void DrawPath(RenderedWorldEventArgs e, List<List<Vector2>> path)
        {
            foreach (var i in path[0])
            {
                Vector2 screenpos = Game1.GlobalToLocal(Game1.viewport, i * Game1.tileSize);
                e.SpriteBatch.Draw(tileHighlight, screenpos, Color.White);
            }
        }
        private void OpenItemMenu(object? sender, ButtonPressedEventArgs e)
        {
            GameLocation playerloc = Game1.player.currentLocation;
            List<Vector2> validChestLocs;


            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if(e.Button is SButton.O && Game1.activeClickableMenu is null && Context.IsPlayerFree)
            {
                Game1.activeClickableMenu = new CustomItemMenu();
                // print button presses to the console window
                this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
            }

            // Keybinds U and J are meant for debugging.
            // J prints out the available tiles the player can walk in.
            // U prints out the chest locations
            if (e.Button is SButton.U && Game1.activeClickableMenu is null && Context.IsPlayerFree && CustomItemMenu.SearchedItem is not null)
            {
                validChestLocs = FindChests.get_chest_locs(playerloc, CustomItemMenu.SearchedItem);
                foreach(Vector2 loc in validChestLocs)
                {
                    Console.WriteLine($"{loc.X} {loc.Y}");
                }

                /*temp2 = Path_Finding.Find_Empty_Tiles(playerloc);
                foreach(Vector2 loc2 in temp2)
                {
                    Console.WriteLine($"Empty: {loc2.X}, {loc2.Y}");
                }*/
            }
            if (e.Button is SButton.J && Game1.activeClickableMenu is null && Context.IsPlayerFree && CustomItemMenu.SearchedItem is not null)
            {
                Console.WriteLine($"Mouse cursor: {Game1.currentCursorTile}");
            }

            if(e.Button is SButton.N && Game1.activeClickableMenu is null && Context.IsPlayerFree && CustomItemMenu.SearchedItem is not null)
            {
                Vector2 playerTileLoc = Game1.player.Tile;
                
                validChestLocs = FindChests.get_chest_locs(playerloc, CustomItemMenu.SearchedItem);
                if (validChestLocs.Count > 0)
                {
                    Dictionary<Vector2, List<Vector2>> validEmptyTiles = Path_Finding.genAdjMatrix(validChestLocs[0]);
                    foreach (var i in validChestLocs)
                    {
                        Console.WriteLine("Chest Locs" + i);
                    }
                    Console.WriteLine("YOU PRESSED N");
                    path = Path_Finding.FindPathBFS(validEmptyTiles, validChestLocs, playerTileLoc);
                }else
                {
                    path.Clear();
                }
            }
        }

        private void resizeCustomMenu(object? sender, WindowResizedEventArgs e)
        {
            if(Game1.activeClickableMenu is CustomItemMenu)
            {
                Game1.activeClickableMenu = new CustomItemMenu();
            }
        }
    }
}