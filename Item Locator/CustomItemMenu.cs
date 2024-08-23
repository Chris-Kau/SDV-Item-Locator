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
        //public static Texture2D? locateButtonTexture;
        public static ClickableTextureComponent? locateButton = new ClickableTextureComponent(new Rectangle(xPos + (UIWidth / 2) - (14 * 6 / 2), yPos + 96 * 5 - (15 * 7 / 2), 14, 15), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(208, 321, 14, 15),6f);
        static int UIWidth = 632;
        static int UIHeight = 600;
        //Takes user's zoomlevel and uiscale into account to center menu based off user's settings too
        static int xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
        static int yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);
        ClickableComponent TitleLabel;
        ClickableComponent MenuDesc;
        TextBox getItemID;
        ClickableComponent LocateButtonText;
        Rectangle getItemIDRect;
        Rectangle LocateButtonRect;

        public CustomItemMenu(Texture2D lBT)
        {
            xPos = Math.Max(0, Math.Min(xPos, Game1.viewport.Width - UIWidth));
            yPos = Math.Max(0, Math.Min(yPos, Game1.viewport.Height - UIHeight));
            TitleLabel = new ClickableComponent(new Rectangle(xPos + 200, yPos + 96, UIWidth - 400, 64), "Item Locator");
            MenuDesc = new ClickableComponent(new Rectangle(xPos + 200, yPos + 150, UIWidth - 400, 64), "Enter Item ID");
            getItemID = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), Game1.smallFont, Game1.textColor)
            {
                X = MenuDesc.bounds.X,
                Y = MenuDesc.bounds.Y + MenuDesc.bounds.Height + 50,
                Width = MenuDesc.bounds.Width,
            };
            LocateButtonText = new ClickableComponent(new Rectangle(xPos + 200 , yPos + 96 * 5, UIWidth - 400, 64), "Locate Item");
            //locateButtonTexture = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            locateButton = new ClickableTextureComponent(new Rectangle(xPos + (UIWidth / 2) - (14 * 6 / 2), yPos + 96 * 5 - (15 * 7 / 2), 14, 15), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(208, 321, 14, 15),6f);
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
            LocateButtonRect = new Rectangle(locateButton.bounds.X, locateButton.bounds.Y, locateButton.bounds.Width * (int)locateButton.scale, locateButton.bounds.Height * (int)locateButton.scale);
            if (getItemIDRect.Contains(x, y))
            {
                getItemID.Selected = true;
            }
            else
            {
                getItemID.Selected = false;
            }
            if(LocateButtonRect.Contains(x, y))
            {
                if(SearchedItem is not null && Game1.activeClickableMenu is CustomItemMenu)
                {
                    Console.WriteLine("YOU PRESSED LOCATE");
                }
                
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            Game1.drawDialogueBox(xPos, yPos, UIWidth, UIHeight, false, true);

            Utility.drawTextWithShadow(b, TitleLabel.name, Game1.dialogueFont, new Vector2(TitleLabel.bounds.X, TitleLabel.bounds.Y), Color.Black);
            Utility.drawTextWithShadow(b, MenuDesc.name, Game1.dialogueFont, new Vector2(MenuDesc.bounds.X, MenuDesc.bounds.Y), Color.Black);
            //b.Draw(locateButtonTexture, new Rectangle(xPos + 200 + (14 * 8 / 2), yPos + 96 * 5 - (15 * 8 / 2), 14 * 8, 15 * 8), new Rectangle(208,321,14,15), Color.White);
            locateButton.draw(b);
            //Utility.drawTextWithShadow(b, LocateButtonText.name, Game1.dialogueFont, new Vector2(LocateButtonText.bounds.X, LocateButtonText.bounds.Y), Color.Black);
            getItemID.Draw(b);
            //LocateButtonText.Draw(b);
            drawMouse(b);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
            yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);
            xPos = Math.Max(0, Math.Min(xPos, Game1.viewport.Width - UIWidth));
            yPos = Math.Max(0, Math.Min(yPos, Game1.viewport.Height - UIHeight));
        }
    }
}
