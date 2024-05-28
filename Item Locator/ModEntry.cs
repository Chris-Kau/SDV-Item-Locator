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
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //Opens up the custom menu
            helper.Events.Input.ButtonPressed += this.OpenItemMenu;
            //resizes menu on window resize
            helper.Events.Display.WindowResized += this.resizeCustomMenu;
            helper.Events.Display.RenderedWorld += this.test;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        private void test(object? sender, RenderedWorldEventArgs e)
        {
            Vector2 temp = new Vector2(68, 20);
            Vector2 screenpos = Game1.GlobalToLocal(Game1.viewport, temp * Game1.tileSize);
            e.SpriteBatch.Draw(Game1.bobbersTexture, screenpos, Color.Black);

        }
        private void OpenItemMenu(object? sender, ButtonPressedEventArgs e)
        {
            GameLocation playerloc = Game1.player.currentLocation;
            List<Vector2> temp;
            List<Vector2> temp2;

            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if(e.Button is SButton.O && Game1.activeClickableMenu is null && Context.IsPlayerFree)
            {
                Game1.activeClickableMenu = new CustomItemMenu();
                // print button presses to the console window
                this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
            }

            // Keybinds P and J are meant for debugging.
            if (e.Button is SButton.P && Game1.activeClickableMenu is null && Context.IsPlayerFree && CustomItemMenu.SearchedItem is not null)
            {
                temp = FindChests.get_chest_locs(playerloc, CustomItemMenu.SearchedItem);
                foreach(Vector2 loc in temp)
                {
                    Console.WriteLine($"{loc.X} {loc.Y}");
                }

                temp2 = Path_Finding.Find_Empty_Tiles(playerloc);
                foreach(Vector2 loc2 in temp2)
                {
                    Console.WriteLine($"Empty: {loc2.X}, {loc2.Y}");
                }
            }
            if (e.Button is SButton.J && Game1.activeClickableMenu is null && Context.IsPlayerFree && CustomItemMenu.SearchedItem is not null)
                Console.WriteLine($"Mouse cursor: {Game1.currentCursorTile}");

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