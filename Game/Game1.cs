using GameHND.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using static System.Formats.Asn1.AsnWriter;

namespace GameHND
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Declare jump-related fields
        private bool jumping = false;
        private float jumpSpeed = 400f; // The initial speed of the jump
        private float jumpTime = 0f;   // The amount of time the sprite has been jumping
        private float jumpDuration = 0.5f; // The duration of the jump in seconds
        private float jumpHeight = 100f; // The height of the jump in pixels

        //adding background layer
        Texture2D backgroundTexture;

        Texture2D blankTexture;



        int screenWidth = 640;
        int screenHeight = 480;


        Sprite player;
        Sprite coin;
        Random rnd = new Random();


        Sprite[] wallArray = new Sprite[10];

        Sprite[] enemyArray = new Sprite[2];


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.ApplyChanges();

            // Initialize variables for the camera position and the screen size
            screenWidth = _graphics.GraphicsDevice.Viewport.Width;
            screenHeight = _graphics.GraphicsDevice.Viewport.Height;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //debugging hitboxes code
            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });

            // TODO: use this.Content to load your game content here

            // Load the player sprite
            player = new Sprite(400, 128, Content.Load<Texture2D>("sprite_01"));
            // Set the player's speed to 64 pixels per second in both the X and Y directions
            player.speed = new Vector2(124, 124);

            // Load the coin sprite and position it at (32, 100) pixels
            coin = new Sprite(32, 100, Content.Load<Texture2D>("example_coin_32x32"));

            //// Load two enemy sprites and set their positions, speeds and directions
            //enemyArray[0] = new Sprite(500, 132, Content.Load<Texture2D>("trashman"));
            //enemyArray[1] = new Sprite(450, 32, Content.Load<Texture2D>("nightman"));

            // Load two enemy sprites and set their positions, speeds and directions
            Rectangle trashmanSource = new Rectangle(0, 0, 32, 32); // Set the area of the sprite sheet to display for trashman
            enemyArray[0] = new Sprite(500, 132, Content.Load<Texture2D>("trashman-still"), trashmanSource); // Create a new enemy sprite with the trashman texture and source rectangle

            Rectangle nightmanSource = new Rectangle(64, 0, 32, 32); // Set the area of the sprite sheet to display for nightman
            enemyArray[1] = new Sprite(450, 32, Content.Load<Texture2D>("nightman-still"), nightmanSource);

            enemyArray[0].direction = new Vector2(-1, 0);
            enemyArray[1].direction = new Vector2(0, -1);

            // Load nine wall sprites and set their positions
            wallArray[0] = new Sprite(0, 448, Content.Load<Texture2D>("floor_32_128"));
            wallArray[1] = new Sprite(128, 448, Content.Load<Texture2D>("floor_32_128"));
            wallArray[2] = new Sprite(256, 448, Content.Load<Texture2D>("floor_32_128"));
            wallArray[3] = new Sprite(384, 448, Content.Load<Texture2D>("floor_32_128"));
            wallArray[4] = new Sprite(512, 448, Content.Load<Texture2D>("floor_32_128"));

            wallArray[5] = new Sprite(100, 100, Content.Load<Texture2D>("example_wall_32x32"));
            wallArray[6] = new Sprite(132, 100, Content.Load<Texture2D>("example_wall_32x32"));
            wallArray[7] = new Sprite(164, 100, Content.Load<Texture2D>("example_wall_32x32"));
            wallArray[8] = new Sprite(196, 100, Content.Load<Texture2D>("example_wall_32x32"));
            wallArray[9] = new Sprite(196, 132, Content.Load<Texture2D>("example_wall_32x32"));

            // Load the background texture
            backgroundTexture = Content.Load<Texture2D>("1_2");

            // Declare variables for the camera position and the screen size
            Vector2 cameraPosition = Vector2.Zero;
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            // Load a font that can be used for rendering text messages to the screen
            //spriteFontMessage = Content.Load<SpriteFont>("spriteFont001");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //apply gravity to enemies
            foreach (Sprite enemy in enemyArray)
            {
                enemy.ApplyGravity(gameTime);
            }


            //declare player position
            Vector2 playerPosition = player.position;

            //enemy speed
            float enemySpeed = 1f;

            //find the player and move towards the player
            for (int i = 0; i < enemyArray.Length; i++)
            {
                Vector2 enemyPosition = enemyArray[i].position;

                // Calculate the distance to the player
                float distanceToPlayer = Vector2.Distance(enemyPosition, playerPosition);

                // If the enemy is within a certain distance of the player, move towards the player
                if (distanceToPlayer < 20000f)
                {
                    // Calculate the direction to move in
                    Vector2 direction = playerPosition - enemyPosition;
                    direction.Normalize();

                    // Update the enemy position
                    enemyPosition += direction * enemySpeed;
                    enemyArray[i].position = enemyPosition;
                }
            }


            // TODO: Add your update logic here
            KeyboardState kb = Keyboard.GetState();

            // Select which key is pressed
            if (kb.IsKeyDown(Keys.Left))
            {
                player.direction.X = -1;
                player.image = Content.Load<Texture2D>("sprite_09");
                
            }
            if (kb.IsKeyDown(Keys.Right))
            {
                player.direction.X = +1;
                player.image = Content.Load<Texture2D>("sprite_02");
                
            }
            if (kb.IsKeyDown(Keys.Up))
            {
                player.direction.Y = -1;
                player.image = Content.Load<Texture2D>("sprite_02");
            }
            //if (kb.IsKeyDown(Keys.Down))
            //{
            //    player.direction.Y = +1;
            //    player.image = Content.Load<Texture2D>("sprite_04");
            //}
            if (kb.IsKeyDown(Keys.Space) && !player.kicking)
            {
                // Set the kicking flag to true and record the start time
                player.kicking = true;
                player.kickTime = 0f;

                // Set the direction of the kick based on the player's current sprite
                if (player.image == Content.Load<Texture2D>("sprite_01") || player.image == Content.Load<Texture2D>("sprite_02"))
                {
                    player.direction.X = 1;
                }
                else if (player.image == Content.Load<Texture2D>("sprite_08") || player.image == Content.Load<Texture2D>("sprite_09"))
                {
                    player.direction.X = -1;
                }
            }

            // Update the kick animation if necessary
            if (player.kicking)
            {
                // Add the elapsed time since the last call to Update() to the total elapsed time
                player.kickTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // If the kick has been playing for 1 second, reset to the idle state
                if (player.kickTime >= 0.5f)
                {
                    player.kicking = false;

                    // Set the player's image back to the idle state based on their direction
                    if (player.direction.X == -1)
                    {
                        player.image = Content.Load<Texture2D>("sprite_08");
                    }
                    else if (player.direction.X == 1)
                    {
                        player.image = Content.Load<Texture2D>("sprite_01");
                    }
                }
                else
                {
                    // Otherwise, update the kick animation
                    if (player.direction.X == -1)
                    {
                        player.image = Content.Load<Texture2D>("sprite_kick_left");
                    }
                    else if (player.direction.X == 1)
                    {
                        player.image = Content.Load<Texture2D>("sprite_kick_right");
                    }
                }
            }

            // Apply gravity to the player
            player.ApplyGravity(gameTime);

            // Update player position with wall detection
            player.UpdateWithWallDetection(gameTime, wallArray);

            base.Update(gameTime);

            // Reset the player's direction if no keys are pressed
            if (kb.GetPressedKeys().Length == 0)
            {
                player.direction = Vector2.Zero;
            }

            player.UpdateWithWallDetection(gameTime, wallArray);

            if (player.Collision(coin))
            {
                coin.position.X = rnd.Next(32, 200);
                coin.position.Y = rnd.Next(32, 200);

            }

            for (int i = 0; i < enemyArray.Length; i++)
            {
                if (player.Collision(enemyArray[i]))
                {
                    player.position.X = 32;
                    player.position.Y = 32;
                }
            }

            player.direction.X = 0;
            player.direction.Y = 0;

      


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Draw the background texture at the bottom of the screen
            int screenHeight = GraphicsDevice.Viewport.Height;
            int textureHeight = backgroundTexture.Height;
            _spriteBatch.Draw(backgroundTexture, new Vector2(25, screenHeight - textureHeight), Color.White);

            player.Draw(_spriteBatch, player.sourceRectangle, 1f);

            // Draw the hitboxes of the sprites
            foreach (var sprite in enemyArray)
            {
                _spriteBatch.Draw(blankTexture, sprite.position, sprite.sourceRectangle, Color.Red * 0.5f);
            }


            coin.Draw(_spriteBatch, null, 1f);

            for (int i = 0; i < wallArray.Length; i++)
            {
                wallArray[i].Draw(_spriteBatch, null, 1f);
            }

            for (int i = 0; i < enemyArray.Length; i++)
            {
                enemyArray[i].Draw(_spriteBatch, player.sourceRectangle, 1f);
            }

            string s = "player.position = (" + Convert.ToString((int)player.position.X) + "," + Convert.ToString((int)player.position.Y) + ")";
            //_spriteBatch.DrawString(spriteFontMessage, s, new Vector2(0, 0), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}