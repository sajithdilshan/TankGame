using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using TankGame.Terrain;
using TankGame.Network;
using TankGame;

namespace TankGame
{
    /// <summary>
    /// This is the main type for your game
    /// render the graphics of the game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        GraphicsDevice device;
   
        Client client;
        World gameWorld;
        Tank tank;
        Tank[] enemyTanks;

        Texture2D backgroundTexture, cellbg,whitebg;
        Texture2D tank0, tank1;
        Texture2D water, stone, brick100, brick75, brick50, brick25,coin,lifepack;

        
        int screenWidth;
        int screenHeight;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferMultiSampling = true;
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferHeight =650;
            //width= height-50+10+500;
            graphics.PreferredBackBufferWidth = 1100;
            graphics.PreferMultiSampling = true;
            
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Tanks War 1.0 - EliteCoders";

            //initialize game client
            client = new Client();
            client.join_game();
            Thread t1 = new Thread(new ThreadStart(client.get_updates));
            t1.Start();
            client.send_updates();
            gameWorld = client.getWorld();
            tank = client.getTank();
            enemyTanks = client.getEnemyTanks();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            device = graphics.GraphicsDevice;
            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;


            spriteBatch = new SpriteBatch(GraphicsDevice);
            //loads image content
            backgroundTexture = Content.Load<Texture2D>("Images/bg");
            brick100 = Content.Load<Texture2D>("Images/bricks-100");
            brick75 = Content.Load<Texture2D>("Images/bricks-75");
            brick50 = Content.Load<Texture2D>("Images/bricks-50");
            brick25 = Content.Load<Texture2D>("Images/bricks-25");

            stone = Content.Load<Texture2D>("Images/stone");
            water = Content.Load<Texture2D>("Images/water");
            coin = Content.Load<Texture2D>("Images/coin");
            lifepack = Content.Load<Texture2D>("Images/lifepack");
            cellbg = Content.Load<Texture2D>("Images/cellbg");
            whitebg = Content.Load<Texture2D>("Images/whitebg");

            tank0 = Content.Load<Texture2D>("Images/tank4");
            tank1 = Content.Load<Texture2D>("Images/tank5");
            font = Content.Load<SpriteFont>("Fonts/spritefont");

            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // tanks, gameworld and enemy tank details will be pulled
            gameWorld = client.getWorld();
            tank = client.getTank();
            enemyTanks = client.getEnemyTanks();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            DrawScenery();
            draw_game_name();
            init_gameworld();
            draw_pointtable();
            spriteBatch.End();

            base.Draw(gameTime);
        }




        //draws the background
        private void DrawScenery()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
        }
        //draws the game name
        private void draw_game_name()
        {
            spriteBatch.DrawString(font, "Tanks War - EliteCoders", new Vector2(500, 10), Color.Black);
        }

        //draws the titles of the point table
        private void draw_pointtable()
        {
            double X = Convert.ToDouble((screenHeight - 50) / ConfigData.MAP_SIZE);
            int startx = (int)Math.Floor(X)*ConfigData.MAP_SIZE+50;

            spriteBatch.DrawString(font, "Points Table", new Vector2(startx + 20, 60), Color.Black);
            spriteBatch.DrawString(font, "Player ID", new Vector2(startx+20, 90), Color.Black);
            spriteBatch.DrawString(font, "Points", new Vector2(startx + 130, 90), Color.Black);
            spriteBatch.DrawString(font, "Coins", new Vector2(startx + 220, 90), Color.Black);
            spriteBatch.DrawString(font, "Health", new Vector2(startx + 300, 90), Color.Black);

        }
        //draws the content of the game worls (cells, tanks)
        private void init_gameworld()
        {
            int x = 0;
            int y = 50;
            double X = Convert.ToDouble((screenHeight - 50) / ConfigData.MAP_SIZE);
            int cellHeight = (int)Math.Floor(X);
            int startx = cellHeight * ConfigData.MAP_SIZE + 50;
            //draws the cells
            for (int i = 0; i < ConfigData.MAP_SIZE; i++)
            {
                for (int j = 0; j < ConfigData.MAP_SIZE; j++)
                {
                    Rectangle square = new Rectangle(x,y, cellHeight, cellHeight);
                    Cell cell = gameWorld.getCell(j, i);
                    //draws bricks
                    if (cell.is_brick)
                    {
                        if (cell.brick_level == 0)
                            spriteBatch.Draw(brick100, square, Color.White);
                        else if (cell.brick_level == 1)
                            spriteBatch.Draw(brick75, square, Color.White);
                        else if (cell.brick_level == 2)
                            spriteBatch.Draw(brick50, square, Color.White);
                        else if (cell.brick_level == 3)
                            spriteBatch.Draw(brick25, square, Color.White);
                        else if(cell.brick_level == 4)
                            spriteBatch.Draw(cellbg, square, Color.White);
                    }
                    //draws stones
                    else if (cell.is_stone)
                    {
                        spriteBatch.Draw(stone, square, Color.White);
                       
                    }
                    //draws water
                    else if (cell.is_water)
                    {
                        spriteBatch.Draw(water, square, Color.White);
                    }
                    //draws coin piles
                    else if (cell.is_coin)
                    {
                        spriteBatch.Draw(coin, square, Color.White);
                    }
                    //draws lifepack
                    else if(cell.is_Life)
                    {
                        spriteBatch.Draw(lifepack, square, Color.White);
                    }
                    else
                        spriteBatch.Draw(cellbg, square, Color.White);

                    x = x + cellHeight;
                    
                }
                x = 0;
                y = y + cellHeight;
            }
            //draws the direction and the position of the game tank
            int t0X = 0 + tank.positionX * cellHeight;
            int t0Y = 50 + tank.positionY * cellHeight;
            if (tank.direction == 0)
            {
                Rectangle tk0 = new Rectangle(t0X, t0Y, cellHeight, cellHeight);
                spriteBatch.Draw(tank0, tk0, Color.White);
            }
            else if (tank.direction == 1)
            {
                Rectangle tk0 = new Rectangle(t0X + cellHeight, t0Y, cellHeight, cellHeight);
                spriteBatch.Draw(tank0, tk0, null, Color.White, MathHelper.PiOver2, new Vector2(0, 0), SpriteEffects.None, 0);
            }
            else if (tank.direction == 2)
            {
                Rectangle tk0 = new Rectangle(t0X + cellHeight, t0Y + cellHeight, cellHeight, cellHeight);
                spriteBatch.Draw(tank0, tk0, null, Color.White, MathHelper.Pi, new Vector2(0, 0), SpriteEffects.None, 0);
            }
            else
            {
                Rectangle tk0 = new Rectangle(t0X, t0Y + cellHeight, cellHeight, cellHeight);
                spriteBatch.Draw(tank0, tk0, null, Color.White, -MathHelper.PiOver2, new Vector2(0, 0), SpriteEffects.None, 0);
            }

            //draws points, coins and health of tank to the point table
            Rectangle empty = new Rectangle(startx, 50, 400, 250);
            spriteBatch.Draw(whitebg, empty, Color.White);
            spriteBatch.DrawString(font,"#"+Convert.ToString(tank.player_number), new Vector2(startx + 50, 120), Color.Red);
            spriteBatch.DrawString(font, Convert.ToString(tank.points), new Vector2(startx + 130, 120), Color.Red);
            spriteBatch.DrawString(font,Convert.ToString(tank.coins)+"$", new Vector2(startx + 220, 120), Color.Red);
            spriteBatch.DrawString(font, Convert.ToString(tank.health)+"%", new Vector2(startx + 300, 120), Color.Red);

            // draws the direction and the position of the enemy tanks
            if (enemyTanks != null)
            {
                int h = 150;
                foreach (Tank tk in enemyTanks)
                {
                    

                    if (tk == null || tk.health==0)
                        continue;
                    int teX = 0 + tk.positionX * cellHeight;
                    int teY = 50 + tk.positionY * cellHeight;

                    if (tank.direction == 0)
                    {
                        Rectangle tk0 = new Rectangle(teX, teY, cellHeight, cellHeight);
                        spriteBatch.Draw(tank1, tk0, Color.White);
                    }
                    else if (tank.direction == 1)
                    {
                        Rectangle tk0 = new Rectangle(teX + cellHeight, teY, cellHeight, cellHeight);
                        spriteBatch.Draw(tank1, tk0, null, Color.White, MathHelper.PiOver2, new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                    else if (tank.direction == 2)
                    {
                        Rectangle tk0 = new Rectangle(teX + cellHeight, teY + cellHeight, cellHeight, cellHeight);
                        spriteBatch.Draw(tank1, tk0, null, Color.White, MathHelper.Pi, new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                    else
                    {
                        Rectangle tk0 = new Rectangle(teX, teY + cellHeight, cellHeight, cellHeight);
                        spriteBatch.Draw(tank1, tk0, null, Color.White, -MathHelper.PiOver2, new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                    // draws the points, coins, health of enemy tanks to the points table
                    spriteBatch.DrawString(font, "#" + Convert.ToString(tk.player_number), new Vector2(startx + 50, h), Color.Blue);
                    spriteBatch.DrawString(font, Convert.ToString(tk.points), new Vector2(startx + 130, h), Color.Blue);
                    spriteBatch.DrawString(font, Convert.ToString(tk.coins) + "$", new Vector2(startx + 220, h), Color.Blue);
                    spriteBatch.DrawString(font, Convert.ToString(tk.health)+"%", new Vector2(startx + 300, h), Color.Blue);

                    h = h + 30;
                }            
            }
        }
    }
}
