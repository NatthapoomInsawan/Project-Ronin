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
    public class TutorialScene : Scene
    {
        Game1 game;
        Texture2D roninTexture;
        Texture2D rectangleBG;
        Texture2D[] tutorial = new Texture2D[4];
        Sprite[] roninFigure = new Sprite[3];

        SoundEffect menuMove;
        SoundEffect menuSelect;

        KeyboardState keyboardState;
        KeyboardState old_keyboardState;

        int selectIndex;
        public TutorialScene (Game1 game, EventHandler SceneEvent) : base(SceneEvent)
        {
            //BG
            rectangleBG = new Texture2D(game.GraphicsDevice, 1, 1);
            rectangleBG.SetData(new Color[] { new Color(197, 205, 207) });
            tutorial[0] = game.Content.Load<Texture2D>("Assets/Scene/Tutorial/Stagger");
            tutorial[1] = game.Content.Load<Texture2D>("Assets/Scene/Tutorial/Parry");
            tutorial[2] = game.Content.Load<Texture2D>("Assets/Scene/Tutorial/Dash");
            tutorial[3] = game.Content.Load<Texture2D>("Assets/Scene/Tutorial/Warning");

            roninTexture = game.Content.Load<Texture2D>("Assets/Character/Ronin/Ronin_sprite");
            roninFigure[0] = new Sprite(roninTexture, false, new Vector2(260, 102), 43, 45, 4, 0, 3f, new Vector2(1f, 1f));
            roninFigure[1] = new Sprite(roninTexture, false, new Vector2(260 + 250, 102), 43, 45, 4, 1, 3f, new Vector2(1f, 1f));
            roninFigure[2] = new Sprite(roninTexture, false, new Vector2(260 + 500, 102), 43, 45, 1, 4, 3f, new Vector2(1f, 1f));

            for (int i = 0; i < 3; i++)
            {
                roninFigure[i].scale = 5;
            }

            menuMove = game.mTitleScene.menuMove;
            menuSelect = game.mTitleScene.menuSelect;

            this.game = game;
        }
        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Down) && old_keyboardState.IsKeyUp(Keys.Down))
            {
                if (selectIndex < 4)
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

            if (keyboardState.IsKeyDown(Keys.X) && old_keyboardState.IsKeyUp(Keys.X))
            {
                game.mCurrentScene = game.mTitleScene;
                selectIndex = 0;
                menuSelect.Play();
            }

            old_keyboardState = keyboardState;

            for(int i = 0; i<3; i++)
            {
                roninFigure[i].Update(gameTime);
            }

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.DrawString(game.font, "TUTORIAL", new Vector2( 20, 10), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(game.font, "Press X back to title menu", new Vector2(20, 750), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);

            if (selectIndex != 0)
                spriteBatch.DrawString(game.font, "Control", new Vector2( 30, 72), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
            if (selectIndex != 1)
                spriteBatch.DrawString(game.font, "Stagger", new Vector2( 30, 72 * 2), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
            if (selectIndex != 2)
                spriteBatch.DrawString(game.font, "Parry", new Vector2( 30, 72 * 3), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
            if (selectIndex != 3)
                spriteBatch.DrawString(game.font, "Dash", new Vector2( 30, 72 * 4), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
            if (selectIndex != 4)
                spriteBatch.DrawString(game.font, "Warning", new Vector2(30, 72 * 5), Color.GhostWhite, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);

            switch (selectIndex)
            {
                case 0:
                    spriteBatch.DrawString(game.font, "Control", new Vector2(30, 72), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.fontx32, "Control", new Vector2(240, 410), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "You can press the       and                      to walk in that direction,", new Vector2(240, 410 +50), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "left", new Vector2(240 + 245, 410 + 50), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "right arrow key", new Vector2(240 + 358, 410 + 50), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "press the    key to attack, press the    key to block and space bar", new Vector2(240, 410 + 100), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "Z", new Vector2(240 + 131, 410 + 100), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "X", new Vector2(240 + 481, 410 + 100), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "for dashing,", new Vector2(240, 410 + 150), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.Draw(rectangleBG, new Rectangle( 240, 82, 800, 300), Color.White);
                    break;
                case 1:
                    spriteBatch.DrawString(game.font, "Stagger", new Vector2(30, 72 * 2), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.fontx32, "Stagger", new Vector2(240, 410), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "Every creature have a                  as yellow bar under the red HP bar.", new Vector2(240, 410 + 50), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "stagger bar", new Vector2(240 + 293, 410 + 50), new Color(179, 144, 90), 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "When you are blocking, you take no damage but increase the stagger bar.", new Vector2(240, 410 + 100), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "If the stagger bar reaches max capacity, the player or enemy becomes", new Vector2(240, 410 + 150), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "vulnerable.", new Vector2(240, 410 + 200), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.Draw(tutorial[0], new Rectangle(240, 82, 800, 300), Color.White);
                    break;
                case 2:
                    spriteBatch.DrawString(game.font, "Parry", new Vector2(30, 72 * 3), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.fontx32, "Parry", new Vector2(240, 410), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "When you block the incoming attack in time, you take no damage nor", new Vector2(240, 410 + 50), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "increase the stagger bar but increase the enemy's stagger bar.", new Vector2(240, 410 + 100), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.Draw(tutorial[1], new Rectangle(240, 82, 800, 300), Color.White);
                    break;
                case 3:
                    spriteBatch.DrawString(game.font, "Dash", new Vector2(30, 72 * 4), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.fontx32, "Dash", new Vector2(240, 410), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "When you dash, you push yourself to the facing direction.", new Vector2(240, 410 + 50), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "The attacker can not attack while you are dashing.", new Vector2(240, 410 + 100), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.Draw(tutorial[2], new Rectangle(240, 82, 800, 300), Color.White);
                    break;
                case 4:
                    spriteBatch.DrawString(game.font, "Warning", new Vector2(30, 72 * 5), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.fontx32, "Warning", new Vector2(240, 410), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "The warning sign on the enemy head warns that their next attack", new Vector2(240, 410 + 50), Color.GhostWhite, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "can't be blocked or parry.", new Vector2(240, 410 + 100), new Color(212, 78, 83), 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.5f);
                    spriteBatch.Draw(tutorial[3], new Rectangle(240, 82, 800, 300), Color.White);
                    break;
            }
            spriteBatch.End();

            //Draw figure
            if (selectIndex == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    roninFigure[i].Draw(spriteBatch);
                }
            }

        }
    }
}
