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
        
        Texture2D? tileHighlight;
        //public static to allow access in CustomItemMenu.cs
        public static bool shouldDraw = false; 
        public static Dictionary<List<Vector2>, Color> pathColors = new();
        public static List<List<Vector2>> paths = new();
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
        /// <summary>
        /// Constantly checks to see if there are paths available to draw and will call the DrawPath function
        /// </summary>
        private void RenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if(paths.Count > 0 && shouldDraw == true)
            {
                DrawPath(e, paths);
            }

        }
        /// <summary>
        /// Draws the paths to the player's screen
        /// </summary>
        private void DrawPath(RenderedWorldEventArgs e, List<List<Vector2>> paths)
        {
            //Draws each path into the user's game
            foreach (var path in paths)
            {
                foreach (var tile in path)
                {
                    Vector2 screenpos = Game1.GlobalToLocal(Game1.viewport, tile * Game1.tileSize);
                    e.SpriteBatch.Draw(tileHighlight, screenpos, pathColors[path]); //pathColors is a dict used to keep track of its distinct color
                }

            }
        }

        /// <summary>
        /// Opens the item search menu upon clicking O
        /// </summary>
        private void OpenItemMenu(object? sender, ButtonPressedEventArgs e)
        {

            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            //Keybind O opens the search menu
            if(e.Button is SButton.O && Game1.activeClickableMenu is null && Context.IsPlayerFree)
            {
                Game1.activeClickableMenu = new CustomItemMenu();
            }

        }

        /// <summary>
        /// resizes the item search menu upon changing window size
        /// </summary>

        private void resizeCustomMenu(object? sender, WindowResizedEventArgs e)
        {
            if (Game1.activeClickableMenu is CustomItemMenu)
            {
                Game1.activeClickableMenu = new CustomItemMenu();
            }
        }
    }
}