using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Extensions;
using System.Collections.Generic;
namespace Item_Locator
{
    public class CustomItemMenu : IClickableMenu
    {
        // --- Autocomplete state (mouse + scroll only) ---
        List<string> _acItems = new();
        int _acScroll = 0;           // index of first visible row
        int _acHoverRow = -1;        // 0..visibleRows-1 (relative to _acScroll)
        int _acVisibleRows = 7;      // rows in dropdown
        int _acRowPad = 6;           // vertical padding per row
        string _acLastQuery = "";
        bool _acEnabled = true;
        bool AC_ShouldShow => _acEnabled && getItem != null && getItem.Selected && !string.IsNullOrEmpty(getItem.Text) && _acItems.Count > 0;

        // set of all item names, this variable is updated upon loading the game.
        public static List<string>? itemNames;
        public static string SearchedItem = "";
        public static string errorMessageText = "";
        // public static Texture2D? locateButtonTexture;
        public static ClickableTextureComponent? locateButton;
        public static ClickableTextureComponent? clearButton;
        public static float locateButtonScale = 3f;
        public static float clearButtonScale = 3f;
        // History Buttons
        public static List<ClickableTextureComponent> listOfHistoryButtons = new(); //used to hold the buttons of each history item
        public static List<Rectangle> listOfHistoryButtonsRects = new(); //used to detect clicks
        public static List<ClickableComponent> listOfHistoryButtonsText = new(); //used to hold the actual item names
        static int UIWidth = 500;
        static int UIHeight = 275;
        static int UIHistoryWidth = 300;
        static int UIHistoryHeight = 500;
        // Takes user's zoomlevel and uiscale into account to center menu based off user's settings too
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
    public CustomItemMenu()
        {
            int viewW = (int)(Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale);
            int viewH = (int)(Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale);

            int totalW = UIHistoryWidth + UIWidth;

            // left edge of the combined block
            int left = Math.Max(0, (viewW - totalW) / 2);

            // set panel Xs
            xPosUIHistory = left;
            xPos = left + UIHistoryWidth - 32;

            // keep your existing vertical behavior (or center if you prefer)
            yPos = Math.Max(8, Math.Min((int)(viewH * 0.05f), viewH - UIHeight - 8)); // ~18% from top
            yPosUIHistory = yPos;

            TitleLabel = new ClickableComponent(new Rectangle(xPos + (UIWidth / 2) - (UIWidth - 400 - 10), yPos + 108, UIWidth - 400, 64), "Item Locator");
            HistoryLabel = new ClickableComponent(new Rectangle(xPosUIHistory + (int)Game1.smallFont.MeasureString("History").X, yPosUIHistory + 125, UIHistoryWidth - 400, 64), "History:");
            getItem = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), Game1.smallFont, Game1.textColor)
            {
                X = xPos + (UIWidth / 2) - (UIWidth / 2) + 36,
                Y = yPos + (UIHeight - 115),
                Width = 300,
            };
            getItem.Text = SearchedItem;

            locateButton = new ClickableTextureComponent(new Rectangle(getItem.X + 16 + getItem.Width, getItem.Y, 14, 15), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(208, 321, 14, 15), locateButtonScale);
            locateButtonRect = new Rectangle(locateButton.bounds.X, locateButton.bounds.Y, locateButton.bounds.Width * (int)locateButton.scale, locateButton.bounds.Height * (int)locateButton.scale);
            
            clearButton = new ClickableTextureComponent(new Rectangle(getItem.X + 75 + getItem.Width, getItem.Y, 14, 15), Game1.content.Load<Texture2D>("LooseSprites\\Cursors"), new Rectangle(269, 471, 14, 15), clearButtonScale);
            clearButtonRect = new Rectangle(clearButton.bounds.X, clearButton.bounds.Y, clearButton.bounds.Width * (int)clearButton.scale, clearButton.bounds.Height * (int)clearButton.scale);
     
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
                if (itemNames != null)
                {
                    Tuple<int, int> WordResults = ItemListHelper.GetRange(itemNames, getItem.Text);
                    AC_RecomputeIfNeeded();
                    foreach (string name in itemNames.GetRange(WordResults.Item1, WordResults.Item2 - WordResults.Item1))
                    {
                        Console.WriteLine(name);
                    }
                    Console.WriteLine("======================");
                }
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
            // detect dropdown click FIRST so outside clicks can dismiss later
            if (AC_ShouldShow)
            {
                var dd = AC_GetDropdownRect();
                if (dd.Contains(x, y))
                {
                    int idx = AC_PixelToIndex(y, dd);
                    if (idx >= 0 && idx < _acItems.Count)
                    {
                        AC_Choose(_acItems[idx]);
                    }
                    return; // handled
                }
            }

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
                _acItems.Clear();
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
            if (locateButton is null || clearButton is null)
                return;

            if(locateButtonRect.Contains(x,y))
            {
                locateButton.hoverText = "Locate Item";
                scaleTransition(locateButton, locateButtonScale + 0.3f, 0.08f); 
            }
            else
            {
                locateButton.hoverText = "";
                scaleTransition(locateButton, locateButtonScale, -0.08f); //6f is the original scale of the locateButton
            }

            if(clearButtonRect.Contains(x,y))
            {
                clearButton.hoverText = "Clear All Paths";
                scaleTransition(clearButton, clearButtonScale + 0.3f, 0.08f);
            }
            else
            {
                clearButton.hoverText = "";
                scaleTransition(clearButton, clearButtonScale, -0.08f);
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
            if (AC_ShouldShow)
            {
                var dd = AC_GetDropdownRect();
                _acHoverRow = dd.Contains(x, y) ? AC_PixelToRow(y, dd) : -1;
            }

        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (AC_ShouldShow && _acItems.Count > _acVisibleRows)
            {
                int delta = Math.Sign(direction); // +120 => +1, -120 => -1
                _acScroll = Math.Clamp(_acScroll - delta, 0, Math.Max(0, _acItems.Count - _acVisibleRows));
            }
        }

        /// <summary>
        /// Draws the menu and menu components onto screen
        /// </summary>
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPos, yPos, UIWidth, UIHeight, false, true);
            Game1.drawDialogueBox(xPosUIHistory, yPosUIHistory, UIHistoryWidth, UIHistoryHeight, false, true);
            var font = Game1.dialogueFont;
            Vector2 size = font.MeasureString(TitleLabel.name);
            // dialog has ~32px border on each side → use inner content width
            float innerLeft = xPos + 32;
            float innerWidth = UIWidth - 64;
            Utility.drawTextWithShadow(b, TitleLabel.name, Game1.dialogueFont, new Vector2(innerLeft + (innerWidth - size.X) / 2f, TitleLabel.bounds.Y), Color.Black);
            Utility.drawTextWithShadow(b, HistoryLabel.name, Game1.dialogueFont, new Vector2(HistoryLabel.bounds.X, HistoryLabel.bounds.Y), Color.Black);

            getItem.Draw(b);

            locateButton?.draw(b);
            clearButton?.draw(b);

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
                Utility.drawTextWithShadow(b, errorMessage.name, Game1.smallFont, new Vector2(xPos + (UIWidth / 2) - (textSize.X / 2), locateButton.bounds.Y + 48), Color.Red);
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


            AC_Draw(b);

            drawMouse(b);
        }

        /// <summary>
        /// Automatically fix window's size and position based off player's window dimensions
        /// </summary>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            int viewW = (int)(Game1.viewport.Width * Game1.options.zoomLevel / Game1.options.uiScale);
            int viewH = (int)(Game1.viewport.Height * Game1.options.zoomLevel / Game1.options.uiScale);

            int totalW = UIHistoryWidth + UIWidth;

            int left = Math.Max(0, (viewW - totalW) / 2);

            xPosUIHistory = left;
            xPos = left + UIHistoryWidth - 32;

            yPos = Math.Max(8, Math.Min((int)(viewH * 0.05f), viewH - UIHeight - 8)); // ~18% from top
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
            getItem.Text = "";
            SearchedItem = "";
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

        Rectangle AC_GetDropdownRect()
        {
            var font = Game1.smallFont;
            int rowHeight = font.LineSpacing + _acRowPad * 2;
            int rows = Math.Min(_acVisibleRows, _acItems.Count);
            int height = rows * rowHeight + 8;
            int width = 420;

            int x = Math.Clamp(getItem.X, 8, Game1.uiViewport.Width - width - 16);
            int belowY = getItem.Y + getItem.Height + 4;
            int aboveY = getItem.Y - height - 4;
            int y = (belowY + height <= Game1.uiViewport.Height - 8) ? belowY : Math.Max(8, aboveY);

            return new Rectangle(x, y, width, height);
        }
        int AC_PixelToRow(int y, Rectangle dd)
        {
            var font = Game1.smallFont;
            int rowHeight = font.LineSpacing + _acRowPad * 2;
            int innerY = y - (dd.Y + 8);
            return innerY < 0 ? -1 : innerY / rowHeight;
        }

        int AC_PixelToIndex(int y, Rectangle dd)
        {
            int row = AC_PixelToRow(y, dd);
            return (row < 0) ? -1 : _acScroll + row;
        }

        void AC_RecomputeIfNeeded()
        {
            string q = getItem?.Text ?? "";
            if (q == _acLastQuery) return;
            _acLastQuery = q;

            _acItems.Clear();
            _acScroll = 0;
            _acHoverRow = -1;

            if (string.IsNullOrWhiteSpace(q) || itemNames == null || itemNames.Count == 0)
                return;

            var slice = ItemListHelper.GetRange(itemNames, q);
            int start = Math.Max(0, slice.Item1);
            int count = Math.Max(0, slice.Item2 - slice.Item1);
            int cap = 200;
            count = Math.Min(count, cap);

            if (count > 0)
                _acItems.AddRange(itemNames.GetRange(start, count));
        }

        void AC_Draw(SpriteBatch b)
        {
            if (!AC_ShouldShow) return;

            var dd = AC_GetDropdownRect();
            var font = Game1.smallFont;
            int rowHeight = font.LineSpacing + _acRowPad * 2;

            IClickableMenu.drawTextureBox(
                b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                dd.X, dd.Y, dd.Width, dd.Height,
                Color.White, 1f, false);

            int xText = dd.X + 12;
            int yStart = dd.Y + 8;

            int end = Math.Min(_acItems.Count, _acScroll + _acVisibleRows);
            for (int i = _acScroll, row = 0; i < end; i++, row++)
            {
                int rowY = yStart + row * rowHeight;

                if (row == _acHoverRow)
                    b.Draw(Game1.staminaRect, new Rectangle(dd.X + 4, rowY, dd.Width - 8, rowHeight), Color.Black * 0.15f);

                Utility.drawTextWithShadow(b, _acItems[i], font, new Vector2(xText, rowY + _acRowPad), Game1.textColor);
            }

            if (_acItems.Count > _acVisibleRows)
            {
                float pct = _acScroll / (float)Math.Max(1, _acItems.Count - _acVisibleRows);
                int barH = Math.Max(12, (int)(dd.Height * (_acVisibleRows / (float)_acItems.Count)));
                int barY = dd.Y + 4 + (int)((dd.Height - 8 - barH) * pct);
                b.Draw(Game1.staminaRect, new Rectangle(dd.Right - 6, barY, 2, barH), Color.White * 0.5f);
            }
        }
        void AC_Choose(string value)
        {
            Game1.playSound("smallSelect");
            getItem.Text = value;     
            SearchedItem = value;
            getItem.Selected = true;

            _acItems.Clear();
            _acHoverRow = -1;
            _acScroll = 0;
        }
    }



}
