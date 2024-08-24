using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Item_Locator
{
    public class CustomItemMenu : IClickableMenu
    {
        public static string SearchedItem = "";
        //public static Texture2D? locateButtonTexture;
        public static ClickableTextureComponent? locateButton;
        public static ClickableTextureComponent? clearButton;
        public static ClickableTextureComponent? finishedButton;
        static int UIWidth = 632;
        static int UIHeight = 600;
        //Takes user's zoomlevel and uiscale into account to center menu based off user's settings too
        static int xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
        static int yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);
        ClickableComponent TitleLabel;
        ClickableComponent? noChestsFound;
        TextBox getItem;
        Rectangle getItemRect;
        Rectangle locateButtonRect;
        Rectangle clearButtonRect;

        public CustomItemMenu()
        {
            xPos = Math.Max(0, Math.Min(xPos, Game1.viewport.Width - UIWidth));
            yPos = Math.Max(0, Math.Min(yPos, Game1.viewport.Height - UIHeight));
            Vector2 spaceSize = Game1.smallFont.MeasureString("    "); //used to artifically justify-center for text in TitleLabel
            TitleLabel = new ClickableComponent(new Rectangle(xPos + (UIWidth / 2) - ((UIWidth - 400) / 2) - (int)spaceSize.X, yPos + 96, UIWidth - 400, 64), "    ItemLocator\nEnter Item Name:");
            getItem = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), Game1.smallFont, Game1.textColor)
            {
                X = xPos + (UIWidth / 2) - (TitleLabel.bounds.Width / 2) - 35,
                Y = TitleLabel.bounds.Y + TitleLabel.bounds.Height + 30,
                Width = TitleLabel.bounds.Width,
            };
            
            locateButton = new ClickableTextureComponent(new Rectangle(xPos + (UIWidth / 2) + (14 * 6), getItem.Y + 40 + (15 * 7 / 2), 14, 15), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(208, 321, 14, 15),6f);
            locateButtonRect = new Rectangle(locateButton.bounds.X, locateButton.bounds.Y, locateButton.bounds.Width * (int)locateButton.scale, locateButton.bounds.Height * (int)locateButton.scale);
            
            clearButton = new ClickableTextureComponent(new Rectangle(xPos + (UIWidth / 2) - (14 * 6 * 2), getItem.Y + 40 + (15 * 7 / 2), 14, 15), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(269, 471, 14, 15), 6f);
            clearButtonRect = new Rectangle(clearButton.bounds.X, clearButton.bounds.Y, clearButton.bounds.Width * (int)clearButton.scale, clearButton.bounds.Height * (int)clearButton.scale);

            finishedButton = new ClickableTextureComponent(new Rectangle(getItem.X + 10 + getItem.Width, getItem.Y, 64, 64), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(128,256,64,64), 0.7f);
            getItem.OnEnterPressed += EnterPressed;
        }

        /// <summary>
        /// Sets the SearchedItem to the item in the textbox upon pressing enter
        /// </summary>
        private void EnterPressed(TextBox sender)
        {
            SearchedItem = sender.Text.ToLower();
        }

        /// <summary>
        /// used for any sort of smooth animation of a component becoming bigger/smaller
        /// </summary>
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

        ///<summary>
        /// Ingores certain key presses to prevent menu closing while typing in textbox
        /// </summary>
        public override void receiveKeyPress(Keys key)
        {

            if(getItem != null && getItem.Selected)
            {
                if (key == Keys.Escape) //ESC is now used to deselect text box while typing, and will close window if textbox is not selected
                {
                    getItem.Selected = false;
                    return;
                }
                if(key == Keys.E) //Keybind E closes the window when typing in textbox, so we check here so it doesnt close while typing
                {
                    return;
                }
                base.receiveKeyPress(key);
            }
            else
            {
                base.receiveKeyPress(key);
            }
            
        }
        /// <summary>
        /// Detects if a player clicked in the area of a clickable component
        /// </summary>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            //Rectangles are used for click detection to see if the player clicked on the clickable components
            getItemRect = new Rectangle(getItem.X, getItem.Y, getItem.Width, getItem.Height);
            if (getItemRect.Contains(x, y))
            {
                getItem.Selected = true; // user is able to type in text box
            }
            else
            {
                SearchedItem = getItem.Text;
                getItem.Selected = false; // user is unable to type in text box
            }
            if(locateButtonRect.Contains(x, y))
            {
                if(SearchedItem is not null && Game1.activeClickableMenu is CustomItemMenu)
                {

                    Path_Finding.GetPaths(); //helps find and draw paths
                    List<Vector2> chestlocs = FindChests.get_chest_locs(Game1.player.currentLocation, SearchedItem);
                    if((Path_Finding.pathCount == 0 || ModEntry.getPathsCount() == 0) && chestlocs.Count == 0)
                    {
                        //add text if there were not paths found
                        noChestsFound = new ClickableComponent(new Rectangle(getItem.X, getItem.Y + 50, 30,30), "No paths or containers found :(");
                    }
                    else if((Path_Finding.pathCount == 0  || ModEntry.getPathsCount() == 0) && chestlocs.Count > 0)
                    {
                        noChestsFound = new ClickableComponent(new Rectangle(getItem.X, getItem.Y + 50, 30, 30), $"No paths found, {chestlocs.Count} containers found");

                    }else
                    {
                        Console.WriteLine($"paths count: {ModEntry.getPathsCount()} or {Path_Finding.pathCount}, chestlocs count: {chestlocs.Count}");
                        Game1.activeClickableMenu = null; //close menu
                        noChestsFound = null;
                    }

                }
                
            }
            if(clearButtonRect.Contains(x,y))
            {
                ModEntry.paths.Clear(); // clear all paths
                ModEntry.shouldDraw = false; 
                Game1.activeClickableMenu = null; //close menu
            }
        }
        /// <summary>
        /// allows visual hover changes such as animation or text
        /// </summary>
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (locateButton is null || clearButton is null)
                return;

            if(locateButtonRect.Contains(x,y))
            {
                locateButton.hoverText = "Locate Item";
                scaleTransition(locateButton, 6.3f, 0.08f); 
            }
            else
            {
                locateButton.hoverText = "";
                scaleTransition(locateButton, 6f, -0.08f); //6f is the original scale of the locateButton
            }

            if(clearButtonRect.Contains(x,y))
            {
                clearButton.hoverText = "Clear All Paths";
                scaleTransition(clearButton, 6.3f, 0.08f);
            }
            else
            {
                clearButton.hoverText = "";
                scaleTransition(clearButton, 6f, -0.08f);
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
            getItem.Draw(b);
           
            locateButton?.draw(b);
            clearButton?.draw(b);
            finishedButton?.draw(b);

            //draws hovertext
            if (!string.IsNullOrEmpty(locateButton?.hoverText))
            {
                drawHoverText(b, locateButton.hoverText, Game1.smallFont);
            }
            if (!string.IsNullOrEmpty(clearButton?.hoverText))
            {
                drawHoverText(b, clearButton.hoverText, Game1.smallFont);
            }

            //draws text if there are no chests found
            if(noChestsFound != null)
                Utility.drawTextWithShadow(b, noChestsFound.name, Game1.smallFont, new Vector2(noChestsFound.bounds.X, noChestsFound.bounds.Y), Color.Red);
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
