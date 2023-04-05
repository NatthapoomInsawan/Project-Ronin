using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Project_Ronin
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public SpriteFont font;
        public SpriteFont fontx32;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public GameScene mGameScene;
        public TitleScene mTitleScene;
        public TutorialScene mTutorialScene;
        public OptionScene mOptionScene;
        public StudioScene mStudioScene;
        public StoryScene mStoryScene;
        public Scene mCurrentScene;

        RenderTarget2D _nativeRenderTarget;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 240 * 5;
            graphics.PreferredBackBufferHeight = 168 * 5;
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

            //Load
            string filePath = Path.Combine(@"Content\data.bin");
            //Check if file exist
            if (File.Exists(filePath) != true)
            {
                FileStream fileStreamWriter = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                BinaryWriter binaryWriter = new BinaryWriter(fileStreamWriter);
                binaryWriter.Write((Int32)1);
                binaryWriter.Write((decimal)0.5M);
                binaryWriter.Write((decimal)0.5M);
                binaryWriter.Flush();
                binaryWriter.Close();
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _nativeRenderTarget = new RenderTarget2D(GraphicsDevice, 240, 168);

            //Load font
            font = Content.Load<SpriteFont>("Assets/Font/SpriteFont");
            fontx32 = Content.Load<SpriteFont>("Assets/Font/SpriteFontx32");

            // TODO: use this.Content to load your game content here
            mStudioScene = new StudioScene(this, new EventHandler(GameSceneEvent));
            mTitleScene = new TitleScene(this, new EventHandler(GameSceneEvent));
            mOptionScene = new OptionScene(this, new EventHandler(GameSceneEvent));
            mGameScene = new GameScene(this, new EventHandler(GameSceneEvent));
            mTutorialScene = new TutorialScene(this, new EventHandler(GameSceneEvent));
            mStoryScene = new StoryScene(this, new EventHandler(GameSceneEvent));
            LoadData();
            mCurrentScene = mStudioScene;


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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

            // TODO: Add your update logic here
            mCurrentScene.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_nativeRenderTarget);
            if(mCurrentScene == mTutorialScene || mCurrentScene == mOptionScene || mCurrentScene == mStudioScene || mCurrentScene == mStoryScene)
            {
                GraphicsDevice.SetRenderTarget(null);
            }
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            mCurrentScene.Draw(spriteBatch);

            //Upscaling
            if (mCurrentScene != mTutorialScene && mCurrentScene != mOptionScene && mCurrentScene != mStudioScene && mCurrentScene != mStoryScene)
            {
                GraphicsDevice.SetRenderTarget(null);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                spriteBatch.Draw(_nativeRenderTarget, new Rectangle(0, 0, 5 * 240, 5 * 168), Color.White);
                spriteBatch.End();
            }

            //Text in Game
            TextInGame();

            //Text in Title
            TextInTitle();

            base.Draw(gameTime);

        }

        public void GameSceneEvent(object obj, EventArgs e)
        {
            mCurrentScene = (Scene)obj;
        }

        private void TextInGame()
        {
            if (mCurrentScene == mGameScene)
            {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                spriteBatch.DrawString(font, "WAVE: " + mGameScene.wave.ToString(), new Vector2(0, 0), Color.Black, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, "Enemy Left ", new Vector2(465, 0), Color.Black, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, mGameScene.enemyLeft.ToString(), new Vector2(585, 62), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                if (mGameScene.menuToggle != true)
                    spriteBatch.DrawString(font, "Esc Menu", new Vector2(960, 0), Color.Black, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);

                //Draw Win
                if (mGameScene.enemyLeft == 0 && mGameScene.ronin.hp != 0)
                {
                    spriteBatch.DrawString(font, "WAVE CLEAR", new Vector2(335, 230), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 2, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(font, "Press Z to next wave, Esc to title screen", new Vector2(215, 380), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                }
                //Draw Death
                if (mGameScene.ronin.hp <= 0)
                {
                    spriteBatch.DrawString(font, "GAME OVER", new Vector2(355, 230), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 2, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(font, "Press Z to reset, Esc to title screen", new Vector2(215, 380), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                }

                //MenuText
                if (mGameScene.menuToggle == true)
                {
                    spriteBatch.DrawString(font, "Esc Close", new Vector2(960, 0), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    if (mGameScene.menuIndex != 0)
                    spriteBatch.DrawString(font, "Back to title", new Vector2(920, 75), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    if (mGameScene.menuIndex != 1)
                        spriteBatch.DrawString(font, "Option", new Vector2(920, 75*2), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    if (mGameScene.menuIndex != 2)
                        spriteBatch.DrawString(font, "Quit game", new Vector2(920, 75 * 3), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);


                    switch (mGameScene.menuIndex)
                    {
                        case 0:
                            spriteBatch.DrawString(font, "Back to title", new Vector2(920, 75), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                            break;
                        case 1:
                            spriteBatch.DrawString(font, "Option", new Vector2(920, 75 * 2), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                            break;
                        case 2:
                            spriteBatch.DrawString(font, "Quit game", new Vector2(920, 75 * 3), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                            break;
                    }

                }

                //DrawName
                if (mGameScene.enemy[0].type == "matsu")
                    spriteBatch.DrawString(font, "Matsu the Devil mask", new Vector2( 225, 115), Color.White, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                if (mGameScene.enemy[0].type == "roland")
                    spriteBatch.DrawString(font, "Roland the Dragon slayer", new Vector2(225, 115), Color.White, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                if (mGameScene.enemy[0].type == "enbu")
                    spriteBatch.DrawString(font, "Enbu the Oni", new Vector2(225, 115), Color.White, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);

                spriteBatch.End();
            }
        }

        private void TextInTitle()
        {
            if (mCurrentScene == mTitleScene)
            {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                spriteBatch.DrawString(font, "START", new Vector2(350 - 60, 300), Color.Black, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, "TUTORIAL", new Vector2(350 - 60, 362), Color.Black, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, "OPTION", new Vector2(350 - 60, 422), Color.Black, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, "EXIT", new Vector2(350 - 60, 484), Color.Black, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);

                spriteBatch.DrawString(fontx32, "Highest Progress", new Vector2(850 - 65, 318), Color.Black, 0.0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(fontx32, "Wave " + mGameScene.progress.ToString(), new Vector2( 865, 318+42), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.5f);

                switch (mTitleScene.selectIndex)
                {
                    case 0:
                        spriteBatch.DrawString(font, "START", new Vector2(350 - 60, 300), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "TUTORIAL", new Vector2(350 - 60, 362), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "OPTION", new Vector2(350 - 60, 422), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                        break;
                    case 3:
                        spriteBatch.DrawString(font, "EXIT", new Vector2(350 - 60, 484), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                        break;
                }

                spriteBatch.End();
            }
        }

        private void LoadData()
        {
            string filePath = Path.Combine(@"Content\data.bin");

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            mGameScene.progress = (int)binaryReader.ReadInt32();
            mOptionScene.bgmVolume = (decimal)binaryReader.ReadDecimal();
            mOptionScene.sfxVolume = (decimal)binaryReader.ReadDecimal();
            fileStream.Close();
        }

        public void SaveData()
        {
            string filePath = Path.Combine(@"Content\data.bin");

            FileStream fileStreamWriter = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter binaryWriter = new BinaryWriter(fileStreamWriter);
            binaryWriter.Write((Int32)mGameScene.progress);
            binaryWriter.Write((decimal)mOptionScene.bgmVolume);
            binaryWriter.Write((decimal)mOptionScene.sfxVolume);
            binaryWriter.Flush();
            binaryWriter.Close();
        }


    }
}
