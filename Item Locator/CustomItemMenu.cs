﻿using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Extensions;
namespace Item_Locator
{
    public class CustomItemMenu : IClickableMenu
    {
        public static string SearchedItem = "";
        public static string errorMessageText = "";
        //public static Texture2D? locateButtonTexture;
        public static ClickableTextureComponent? locateButton;
        public static ClickableTextureComponent? clearButton;
        public static ClickableTextureComponent? clearInputButton;
        //History Buttons
        public static List<ClickableTextureComponent> listOfHistoryButtons = new(); //used to hold the buttons of each history item
        public static List<Rectangle> listOfHistoryButtonsRects = new(); //used to detect clicks
        public static List<ClickableComponent> listOfHistoryButtonsText = new(); //used to hold the actual item names
        static int UIWidth = 632;
        static int UIHeight = 500;
        static int UIHistoryWidth = 300;
        static int UIHistoryHeight = 500;
        //Takes user's zoomlevel and uiscale into account to center menu based off user's settings too
        static int xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
        static int yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);
        static int xPosUIHistory = xPos - 275;
        static int yPosUIHistory = yPos;
        ClickableComponent TitleLabel;
        ClickableComponent HistoryLabel;
        ClickableComponent? errorMessage;
        TextBox getItem;
        Rectangle getItemRect;
        Rectangle locateButtonRect;
        Rectangle clearButtonRect;
        Rectangle clearInputButtonRect;
    public CustomItemMenu()
        {
            //re-assign x/y pos to ensure correct scaling of game window
            xPos = (int)((Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale / 2) - (UIWidth / 2));
            yPos = (int)((Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale / 2) - UIHeight);
            xPos = Math.Max(0, Math.Min(xPos, Game1.viewport.Width - UIWidth));
            yPos = Math.Max(0, Math.Min(yPos, Game1.viewport.Height - UIHeight));
            xPosUIHistory = xPos - 275;
            yPosUIHistory = yPos;
            Vector2 spaceSize = Game1.smallFont.MeasureString("   "); //used to artifically justify-center for text in TitleLabel
            TitleLabel = new ClickableComponent(new Rectangle(xPos + (UIWidth / 2) - ((UIWidth - 400) / 2) - (int)spaceSize.X, yPos + 125, UIWidth - 400, 64), "   Item Locator\nEnter Item Name:");
            HistoryLabel = new ClickableComponent(new Rectangle(xPosUIHistory + (int)Game1.smallFont.MeasureString("History").X, yPosUIHistory + 125, UIHistoryWidth - 400, 64), "History:");
            getItem = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), Game1.smallFont, Game1.textColor)
            {
                X = xPos + (UIWidth / 2) - (TitleLabel.bounds.Width / 2) - 35,
                Y = TitleLabel.bounds.Y + TitleLabel.bounds.Height + 30,
                Width = TitleLabel.bounds.Width,
            };
            getItem.Text = SearchedItem;

            locateButton = new ClickableTextureComponent(new Rectangle(xPos + (UIWidth / 2) + (14 * 6), getItem.Y + 75 + (15 * 7 / 2), 14, 15), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(208, 321, 14, 15),6f);
            locateButtonRect = new Rectangle(locateButton.bounds.X, locateButton.bounds.Y, locateButton.bounds.Width * (int)locateButton.scale, locateButton.bounds.Height * (int)locateButton.scale);
            
            clearButton = new ClickableTextureComponent(new Rectangle(xPos + (UIWidth / 2) - (14 * 6 * 2), getItem.Y + 75 + (15 * 7 / 2), 14, 15), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(269, 471, 14, 15), 6f);
            clearButtonRect = new Rectangle(clearButton.bounds.X, clearButton.bounds.Y, clearButton.bounds.Width * (int)clearButton.scale, clearButton.bounds.Height * (int)clearButton.scale);
     
            clearInputButton = new ClickableTextureComponent(new Rectangle(getItem.X + 10 + getItem.Width, getItem.Y, 64, 64), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(192,256,64,64), 0.7f);
            clearInputButtonRect = new Rectangle(clearInputButton.bounds.X, clearInputButton.bounds.Y, (int)(clearInputButton.bounds.Width * clearInputButton.scale), (int)(clearInputButton.bounds.Height * clearInputButton.scale));
            getItem.OnEnterPressed += EnterPressed;

            //create 5 history buttons
            updateHistoryList();


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
                    getItem.Text = "";
                    SearchedItem = "";
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
                
                scaleTransition(locateButton, 5.7f, -0.08f);
                scaleTransition(locateButton, 6f, 0.08f);

                ClickLocate(true);
                
            }
            if(clearButtonRect.Contains(x,y))
            {
                Game1.playSound("select");
                ModEntry.paths.Clear(); // clear all paths
                ModEntry.shouldDraw = false; 
                Game1.activeClickableMenu = null; //close menu
            }
            if(clearInputButtonRect.Contains(x,y))
            {
                Game1.playSound("select");
                scaleTransition(clearInputButton, 0.67f, -0.02f);
                scaleTransition(clearInputButton, 0.7f, 0.02f);
                getItem.Text = "";
                SearchedItem = "";
            }

            for(int i = 0; i < listOfHistoryButtons.Count; i++)
            {
                if (listOfHistoryButtonsRects[i].Contains(x, y))
                {
                    SearchedItem = listOfHistoryButtonsText[i].name;
                    ClickLocate(false);
                }
            }
        }
        public void ClickLocate(bool isHistory)
        {
            if (SearchedItem is not null && Game1.activeClickableMenu is CustomItemMenu)
            {
                Game1.playSound("select");
                Path_Finding.GetPaths(); //finds and draw paths
                List<Vector2> chestlocs = FindContainers.get_container_locs(Game1.player.currentLocation, SearchedItem);
                if (Path_Finding.invalidPlayerTile)
                {
                    errorMessageText = "Please stand in a valid tile";
                }
                else if (ModEntry.paths.Count == 0 && chestlocs.Count == 0)
                {
                    errorMessageText = "No paths or containers found :(";
                }
                else if (ModEntry.paths.Count == 0 && chestlocs.Count > 0)
                {
                    errorMessageText = $"No paths found, but {chestlocs.Count} containers found";
                }
                else
                {
                    Game1.activeClickableMenu = null; //close menu
                    errorMessageText = "";
                }
                //make text to display on user's screen with the error message
                errorMessage = new ClickableComponent(new Rectangle(getItem.X, getItem.Y + 75, 30, 30), errorMessageText);
                if(isHistory) //checks to see if the user located the item from the history window, if they did not, then update history
                {
                    changeLocateHistory(ModEntry.locateHistory, SearchedItem);
                }
                updateHistoryList();

            }
        }
        /// <summary>
        /// allows visual hover changes such as animation or text
        /// </summary>
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (locateButton is null || clearButton is null || clearInputButton is null)
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

            if(clearInputButtonRect.Contains(x,y))
            {
                clearInputButton.hoverText = "Clear Input";
                scaleTransition(clearInputButton, 0.73f, 0.08f);
            }
            else
            {
                clearInputButton.hoverText = "";
                scaleTransition(clearInputButton, 0.7f, -0.08f);
            }
            for (int i = 0; i < listOfHistoryButtons.Count; i++){
                if (listOfHistoryButtonsRects[i].Contains(x, y))
                {
                    scaleTransition(listOfHistoryButtons[i], 2.3f, 0.04f);
                }
                else
                {
                    scaleTransition(listOfHistoryButtons[i], 2f, -0.04f);
                }
            }


        

        }
        /// <summary>
        /// Draws the menu and menu components onto screen
        /// </summary>
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPos, yPos, UIWidth, UIHeight, false, true);
            Game1.drawDialogueBox(xPos - 275, yPos, 300, UIHeight, false, true);
            Utility.drawTextWithShadow(b, TitleLabel.name, Game1.dialogueFont, new Vector2(TitleLabel.bounds.X, TitleLabel.bounds.Y), Color.Black);
            Utility.drawTextWithShadow(b, HistoryLabel.name, Game1.dialogueFont, new Vector2(HistoryLabel.bounds.X, HistoryLabel.bounds.Y), Color.Black);

            getItem.Draw(b);
           
            locateButton?.draw(b);
            clearButton?.draw(b);
            clearInputButton?.draw(b);

            for(int i = 0; i < listOfHistoryButtons.Count; i++) //draw the history buttons and item names
            {
                ClickableTextureComponent button = listOfHistoryButtons[i];
                button.draw(b);
                Utility.drawTextWithShadow(b, listOfHistoryButtonsText[i].name, Game1.smallFont, new Vector2(listOfHistoryButtonsText[i].bounds.X, listOfHistoryButtonsText[i].bounds.Y), Color.Black);

            }

            //draws text if there are no chests found
            if (errorMessage != null)
            {
                Vector2 textSize = Game1.smallFont.MeasureString(errorMessageText);
                Utility.drawTextWithShadow(b, errorMessage.name, Game1.smallFont, new Vector2(xPos + (UIWidth / 2) - (textSize.X / 2), errorMessage.bounds.Y), Color.Red);
            }

            //draws hovertext
            if (!string.IsNullOrEmpty(locateButton?.hoverText))
            {
                drawHoverText(b, locateButton.hoverText, Game1.smallFont);
            }
            if (!string.IsNullOrEmpty(clearButton?.hoverText))
            {
                drawHoverText(b, clearButton.hoverText, Game1.smallFont);
            }
            if (!string.IsNullOrEmpty(clearInputButton?.hoverText))
            {
                drawHoverText(b, clearInputButton.hoverText, Game1.smallFont);
            }


                
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

            xPosUIHistory = xPos - 275;
            yPosUIHistory = yPos;
        }
        /// <summary>
        /// Mainly used to update config file and the History list and make sure there's only 5 items
        /// </summary>
        private void changeLocateHistory(List<string> locHist, string item)
        {
            locHist.Insert(0, item);
            while (locHist.Count > 5)
            {
                locHist.RemoveAt(locHist.Count - 1);
            }
            ModEntry.updateLocateHistory = true; //when true, it will be caught in ModEntry.RenderedWorld and is used to save the location history to config file.
        }
        /// <summary>
        /// Used to reset the history lists everytime the player clicks locate so they can see the change in history in real time
        /// </summary>
        private void updateHistoryList()
        {
            listOfHistoryButtons.Clear(); //clear to prevent duplicates when reopening menu
            listOfHistoryButtonsRects.Clear();
            listOfHistoryButtonsText.Clear();
            ClickableTextureComponent temp;
            for (int i = 0; i < 5; i++)
            {
                temp = new ClickableTextureComponent(new Rectangle(xPosUIHistory + (16 * 2) + 15, HistoryLabel.bounds.Y + HistoryLabel.bounds.Height + (i * 50), 16, 16), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(274, 284, 16, 16), 2f);
                Rectangle irect = new Rectangle(temp.bounds.X, temp.bounds.Y, temp.bounds.Width * (int)temp.scale, temp.bounds.Height * (int)temp.scale);
                ClickableComponent itext = new ClickableComponent(new Rectangle(HistoryLabel.bounds.X, HistoryLabel.bounds.Y + HistoryLabel.bounds.Height + (i * 50), HistoryLabel.bounds.Width, HistoryLabel.bounds.Height), ModEntry.locateHistory[i]);
                listOfHistoryButtonsRects.Add(irect);
                listOfHistoryButtons.Add(temp);
                listOfHistoryButtonsText.Add(itext);
            }
        }
    }
}
