using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Project_Ronin
{
    public class TitleScene : Scene
    {
        Game1 game;
        Texture2D sceneTexture;
        Sprite titleScene;

        public SoundEffect menuMove;
        public SoundEffect menuSelect;

        KeyboardState keyboardState;
        KeyboardState old_keyboardState;

        public int selectIndex;
        bool firstTime;

        public TitleScene (Game1 game, EventHandler SceneEvent) : base(SceneEvent)
        {
            //BG
            sceneTexture = game.Content.Load<Texture2D>("Assets/Scene/Title_Screen_sprite");
            titleScene = new Sprite(sceneTexture, false, new Vector2(0, 0), 240, 168, 4, 0, 1.4f, new Vector2(1f, 1f));
            selectIndex = 0;

            //Sound
            menuMove = game.Content.Load<SoundEffect>("Assets/Sound/SFX/Menu_move");
            menuSelect = game.Content.Load<SoundEffect>("Assets/Sound/SFX/Menu_select");

            this.game = game;
        }
        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Down) && old_keyboardState.IsKeyUp(Keys.Down))
            {
                if (selectIndex < 3)
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

            if (keyboardState.IsKeyDown(Keys.Z) && old_keyboardState.IsKeyUp(Keys.Z))
            {
                switch (selectIndex)
                {
                    case 0:
                        if (firstTime == false)
                            game.mCurrentScene = game.mStoryScene;
                        else
                            game.mCurrentScene = game.mGameScene;
                        if (game.mGameScene.bgmPlay == true)
                            game.mGameScene.ResumeBGM();
                        else
                        {
                            game.mGameScene.PlayBGM();
                            game.mGameScene.bgmPlay = true;
                        }
                        menuSelect.Play();
                        break;
                    case 1:
                        game.mCurrentScene = game.mTutorialScene;
                        menuSelect.Play();
                        break;
                    case 2:
                        game.mOptionScene.previousScene = game.mTitleScene;
                        game.mCurrentScene = game.mOptionScene;
                        menuSelect.Play();
                        break;
                    case 3:
                        menuSelect.Play();
                        game.SaveData();
                        game.Exit();
                        break;
                }
            }

            old_keyboardState = keyboardState;
            titleScene.Update(gameTime);
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            titleScene.Draw(spriteBatch);
        }
    }
}
