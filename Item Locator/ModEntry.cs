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
            //need to make changes to support multiple paths rather than just the path to the first target.
            //perferrebly make the different paths different colors.
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
            //Keybind O opens the search menu
            if(e.Button is SButton.O && Game1.activeClickableMenu is null && Context.IsPlayerFree)
            {
                Game1.activeClickableMenu = new CustomItemMenu();
                // print button presses to the console window
                this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
            }

            //Keybind J prints the tile location of mouse cursor for debugging REMOVE LATER
            if (e.Button is SButton.J && Game1.activeClickableMenu is null && Context.IsPlayerFree && CustomItemMenu.SearchedItem is not null)
            {
                Console.WriteLine($"Mouse cursor: {Game1.currentCursorTile}");
            }


            //Keybind N is in charge of generating the path once the user and inputted a valid item id in the custom menu
            //REMOVE THIS KEYBIND LATER, THE ONLY KEYBIND THAT WE SHOULD HAVE SHOULD BE TO OPEN THE MENU.
            //implement this function (somehow) in the CustomItemMenu.cs using buttons or something idkf
            if(e.Button is SButton.N && Game1.activeClickableMenu is null && Context.IsPlayerFree && CustomItemMenu.SearchedItem is not null)
            {
                //get player tile location
                Vector2 playerTileLoc = Game1.player.Tile;
                
                //finds all chest locations that contain the searched item
                validChestLocs = FindChests.get_chest_locs(playerloc, CustomItemMenu.SearchedItem);

                if (validChestLocs.Count > 0)
                {
                    Dictionary<Vector2, List<Vector2>> validEmptyTiles = Path_Finding.genAdjList(validChestLocs[0]);
                    path = Path_Finding.FindPathBFS(validEmptyTiles, validChestLocs, playerTileLoc);
                }else
                {
                    //if no paths are found, clear the list from previous paths so it doesnt draw it.
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