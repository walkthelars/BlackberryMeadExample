using BlackberryMead.Input.Typography;
using BlackberryMead.Input.UI;
using BlackberryMead.Serialization;
using BlackberryMead.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using BlackberryMead;
using BlackberryMead.Input;

namespace Example
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;

        /// <summary>
        /// User Interface of the application.
        /// </summary>
        private UserInterface UI;

        /// <summary>
        /// Input manager to manage player input.
        /// </summary>
        private InputManager input;

        /// <summary>
        /// Actions avaliable to the player.
        /// </summary>
        private Dictionary<string, Keybind> actions;

        public Game1()
        {
            // Default for Monogame.
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 600;

            // Initialize the InputManager.
            input = new InputManager();

            // Define an action for the player clicking the mouse.
            actions = new Dictionary<string, Keybind>()
            {
                { "Select", new Keybind(MouseButton.Left, Keybind.ActionType.Press) }
            };
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Debug.Print("Loading fonts.");

            // Get all folders in Fonts directory
            string[] fontFolders = Directory.GetDirectories(@"Content\Fonts").
                Select(Path.GetFileName).ToArray()!;

            // Load each font
            foreach (string fontFolder in fontFolders)
            {
                string folderPath = @"Content\Fonts\" + fontFolder + "\\";

                // Load image
                Texture2D fontSprite = Content.Load<Texture2D>("Fonts\\" + fontFolder + "\\" + fontFolder + "_texture");

                // Load all Json files
                List<string> fontNames = Directory.GetFiles(folderPath, "*.json").Distinct().ToList();
                fontNames.Remove(fontFolder + "_texture");
                foreach (string fontName in fontNames)
                {
                    // Try and deserialize to a font
                    try
                    {
                        Font font = Json.DeserializePath<Font>(fontName, UserInterface.DefaultDeserializationOptions);
                        font.Texture = fontSprite;
                        Font.FontDict.Add(FileHelper.GetFileName(fontName), font);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message);
                    }
                }
            }

            Dictionary<string, Window> windows =
                Json.DeserializePath<Dictionary<string, Window>>(@"Content\UI.json",
                UserInterface.DefaultDeserializationOptions);

            // Create the UI.
            UI = new UserInterface(GraphicsDevice, Content, windows, new Size(1600, 1000), 0.7f);
            UI.OpenWindow("MainMenu");

            // Show the borders on each UI element to confirm it's working.
            UI.ShowBorders = true;

            // Pair buttons
            if (UI.GetChild<Button>("ZoomInButton", out var zoomInButton))
            {
                zoomInButton.OnClick += new EventHandler((sender, e) =>
                {
                    UI.Rescale((float)Math.Round(UI.RenderScale + 0.05f, 2));
                });
            }
            if (UI.GetChild<Button>("ZoomOutButton", out var zoomOutButton))
            {
                zoomOutButton.OnClick += new EventHandler((sender, e) =>
                {
                    if (UI.RenderScale > UI.MaxZoomOutLevel)
                    {
                        UI.Rescale((float)Math.Round(UI.RenderScale - 0.05f, 2));
                    }
                });
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Input from the player for the current update tick.
            // Pass in the avaliable actions to the player.
            InputState inputState = input.GetState(actions);

            if (inputState.IsActionTriggered("Select"))
            {

            }

            // Update the UI
            UI.UpdateAndRender(inputState, spriteBatch);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            UI.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
