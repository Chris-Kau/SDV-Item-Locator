using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        Rectangle getItemIDRect;
        Rectangle LocateButtonRect;

        public CustomItemMenu()
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
            locateButton = new ClickableTextureComponent(new Rectangle(xPos + (UIWidth / 2) - (14 * 6 / 2), yPos + 96 * 5 - (15 * 7 / 2), 14, 15), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(208, 321, 14, 15),6f);
            getItemID.OnEnterPressed += EnterPressed;
        }

        /// <summary>
        /// Sets the SearchedItem to the item in the textbox upon pressing enter
        /// </summary>
        private void EnterPressed(TextBox sender)
        {
            SearchedItem = new StardewValley.Object(sender.Text, 1);
        }

        private void scaleTransition(ClickableTextureComponent icon, float scaleResult, float delta)
        {
            //if delta > 0, that means we want to scale up, otherwise scale down
            if (delta > 0)
            {
                if (icon.scale < scaleResult)
                {
                    icon.scale += delta;
                }
                else
                {
                    icon.scale = scaleResult;
                }
            }
            else
            {
                if (icon.scale > scaleResult)
                {
                    icon.scale += delta;
                }
                else
                {
                    icon.scale = scaleResult;
                }
            }
        }

        /// <summary>
        /// Detects if a player clicked in the area of a clickable component
        /// </summary>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            //Rectangles are used for click detection to see if the player clicked on the clickable components
            getItemIDRect = new Rectangle(getItemID.X, getItemID.Y, getItemID.Width, getItemID.Height);
            LocateButtonRect = new Rectangle(locateButton.bounds.X, locateButton.bounds.Y, locateButton.bounds.Width * (int)locateButton.scale, locateButton.bounds.Height * (int)locateButton.scale);
            
            if (getItemIDRect.Contains(x, y))
            {
                getItemID.Selected = true; // user is able to type in text box
            }
            else
            {
                getItemID.Selected = false; // user is unable to type in text box
            }
            if(LocateButtonRect.Contains(x, y))
            {
                if(SearchedItem is not null && Game1.activeClickableMenu is CustomItemMenu)
                {
                    Path_Finding.GetPaths(); //helps finds and draw paths
                    Game1.activeClickableMenu = null; //close menu
                }
                
            }
        }
        /// <summary>
        /// allows visual hover changes such as animation or text
        /// </summary>
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            LocateButtonRect = new Rectangle(locateButton.bounds.X, locateButton.bounds.Y, locateButton.bounds.Width * (int)locateButton.scale, locateButton.bounds.Height * (int)locateButton.scale);
            if(LocateButtonRect.Contains(x,y))
            {
                locateButton.hoverText = "Locate Item";
                scaleTransition(locateButton, 6.3f, 0.08f);
            }
            else
            {
                locateButton.hoverText = "";
                scaleTransition(locateButton, 6f, -0.08f);
            }

        }
        /// <summary>
        /// Draws the menu and menu components onto screen
        /// </summary>
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPos, yPos, UIWidth, UIHeight, false, true);
            Utility.drawTextWithShadow(b, TitleLabel.name, Game1.dialogueFont, new Vector2(TitleLabel.bounds.X, TitleLabel.bounds.Y), Color.Black);
            Utility.drawTextWithShadow(b, MenuDesc.name, Game1.dialogueFont, new Vector2(MenuDesc.bounds.X, MenuDesc.bounds.Y), Color.Black);
            locateButton.draw(b);

            //draws hovertext
            if (!string.IsNullOrEmpty(locateButton.hoverText))
            {
                IClickableMenu.drawHoverText(b, locateButton.hoverText, Game1.smallFont);
            }
            getItemID.Draw(b);
            drawMouse(b);
        }

        /// <summary>
        /// Automatically fix window's size and position based off player's window dimensions
        /// </summary>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            //resets x and y pos 
            xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
            yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);

            //ensures that it stays in the same area despire window dimensions
            xPos = Math.Max(0, Math.Min(xPos, Game1.viewport.Width - UIWidth));
            yPos = Math.Max(0, Math.Min(yPos, Game1.viewport.Height - UIHeight));
        }
    }
}
