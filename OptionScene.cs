using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Project_Ronin
{
    public class OptionScene : Scene
    {
        Game1 game;

        public Scene previousScene;

        public SoundEffect menuMove;
        public SoundEffect menuSelect;
        Texture2D barTexture;

        KeyboardState keyboardState;
        KeyboardState old_keyboardState;

        public int selectIndex;
        int sfxPercent;
        int bgmPercent;

        public decimal sfxVolume = 0.5M;
        public decimal bgmVolume = 0.5M;

        public OptionScene (Game1 game, EventHandler SceneEvent) : base(SceneEvent)
        {
            selectIndex = 0;
            //bar texture
            barTexture = game.Content.Load<Texture2D>("Assets/HUD/Bar");

            //sound
            menuMove = game.mTitleScene.menuMove;
            menuSelect = game.mTitleScene.menuSelect;
            this.game = game;
        }
        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Down) && old_keyboardState.IsKeyUp(Keys.Down))
            {
                if (selectIndex < 1)
                {
                    selectIndex++;
                    menuMove.Play();
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Up) && old_keyboardState.IsKeyUp(Keys.Up))
            {
                if (selectIndex > 0)
                {
                    selectIndex--;
                    menuMove.Play();
                }
            }

            if (keyboardState.IsKeyDown(Keys.Left) )
            {
                switch (selectIndex)
                {
                    case 0:
                        if (bgmVolume != 0 && bgmVolume - 0.005M >= 0)
                            bgmVolume -= 0.005M;
                        menuSelect.Play(volume: (float)bgmVolume, pitch: 0.0f, pan: 0.0f);
                        break;
                    case 1:
                        if (sfxVolume != 0 && sfxVolume - 0.005M >= 0)
                            sfxVolume -= 0.005M;
                        menuSelect.Play(volume: (float)sfxVolume, pitch: 0.0f, pan: 0.0f);
                        break;
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                switch (selectIndex)
                {
                    case 0:
                        if (bgmVolume + 0.005M <= 0.5M)
                            bgmVolume += 0.005M;
                        menuSelect.Play(volume: (float)bgmVolume, pitch: 0.0f, pan: 0.0f);
                        break;
                    case 1:
                        if (sfxVolume <= 0.5M && sfxVolume + 0.005M <= 0.5M)
                            sfxVolume += 0.005M;
                        menuSelect.Play(volume: (float)sfxVolume, pitch: 0.0f, pan: 0.0f);
                        break;
                }
            }

            if (keyboardState.IsKeyDown(Keys.X) && old_keyboardState.IsKeyUp(Keys.X))
            {
                selectIndex = 0;
                menuSelect.Play();
                game.SaveData();
                game.mCurrentScene = previousScene;
                if (previousScene == game.mGameScene)
                {
                    game.mGameScene.ResumeBGM();
                }
            }

            old_keyboardState = keyboardState;

            sfxPercent = (int)Math.Ceiling(100 * sfxVolume / 0.5M);
            bgmPercent = (int)Math.Ceiling(100 * bgmVolume / 0.5M);

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.DrawString(game.font, "OPTION", new Vector2(20, 10), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(game.font, "Press X back to previous scene", new Vector2(20, 750), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);

            if (selectIndex != 0)
                spriteBatch.DrawString(game.font, "Music", new Vector2(30, 72), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
            if (selectIndex != 1)
                spriteBatch.DrawString(game.font, "Sound Effects", new Vector2(30, 72 * 2), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);

            spriteBatch.Draw(barTexture, new Vector2(30 + 440, 80), new Rectangle(20 * 0, 2 * 0, 20, 2), Color.White, 0.0f, new Vector2(0, 0), 20, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(barTexture, new Vector2(30 + 440, 80), new Rectangle(20 * 0, 2 * 1, (int)Math.Ceiling((double)barTexture.Width * bgmPercent / 100), 2), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 20, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(game.font, bgmPercent.ToString() + " %", new Vector2(950, 72), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);

            spriteBatch.Draw(barTexture, new Vector2(30 + 440, 80 + 75), new Rectangle(20 * 0, 2 * 0, 20, 2), Color.White, 0.0f, new Vector2(0, 0), 20, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(barTexture, new Vector2(30 + 440, 80 + 75), new Rectangle(20 * 0, 2 * 1, (int)Math.Ceiling((decimal)barTexture.Width * sfxPercent / 100), 2), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 20, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(game.font, sfxPercent.ToString() + " %", new Vector2(950, 72 + 75), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);

            switch (selectIndex)
            {
                case 0:
                    spriteBatch.DrawString(game.font, "Music", new Vector2(30, 72), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    break;
                case 1:
                    spriteBatch.DrawString(game.font, "Sound Effects", new Vector2(30, 72 * 2), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    break;
            }

            spriteBatch.End();
        }
    }
}
