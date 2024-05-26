using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
namespace Item_Locator
{
    public class CustomItemMenu : IClickableMenu
    {
        static int UIWidth = 632;
        static int UIHeight = 600;
        static int xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
        static int yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);


    }
}
