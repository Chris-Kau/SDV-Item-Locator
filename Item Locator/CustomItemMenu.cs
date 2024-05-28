using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
namespace Item_Locator
{
    public class CustomItemMenu : IClickableMenu
    {
        public static Item? SearchedItem;
        static int UIWidth = 632;
        static int UIHeight = 600;
        //Takes user's zoomlevel and uiscale into account to center menu based off user's settings too
        static int xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
        static int yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);

        ClickableComponent TitleLabel;
        ClickableComponent MenuDesc;
        TextBox getItemID;
        Rectangle getItemIDRect;

        public CustomItemMenu()
        {
            TitleLabel = new ClickableComponent(new Rectangle(xPos + 200, yPos + 96, UIWidth - 400, 64), "Item Locator");
            MenuDesc = new ClickableComponent(new Rectangle(xPos + 200, yPos + 150, UIWidth - 400, 64), "Enter Item ID");
            getItemID = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), Game1.smallFont, Game1.textColor)
            {
                X = MenuDesc.bounds.X,
                Y = MenuDesc.bounds.Y + MenuDesc.bounds.Height + 50,
                Width = MenuDesc.bounds.Width,
            };
            getItemID.OnEnterPressed += EnterPressed;
        }

        private void EnterPressed(TextBox sender)
        {
            Console.WriteLine($"User entered: {sender.Text}");
            SearchedItem = new StardewValley.Object(sender.Text, 1);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            getItemIDRect = new Rectangle(getItemID.X, getItemID.Y, getItemID.Width, getItemID.Height);
            if (getItemIDRect.Contains(x, y))
            {
                Console.WriteLine($"YPI C:COELD ON THET EXTXBOX");
                getItemID.Selected = true;
            }
            else
            {
                getItemID.Selected = false;
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            Game1.drawDialogueBox(xPos, yPos, UIWidth, UIHeight, false, true);

            Utility.drawTextWithShadow(b, TitleLabel.name, Game1.dialogueFont, new Vector2(TitleLabel.bounds.X, TitleLabel.bounds.Y), Color.Black);
            Utility.drawTextWithShadow(b, MenuDesc.name, Game1.dialogueFont, new Vector2(MenuDesc.bounds.X, MenuDesc.bounds.Y), Color.Black);
            getItemID.Draw(b);
            drawMouse(b);
        }
    }
}
