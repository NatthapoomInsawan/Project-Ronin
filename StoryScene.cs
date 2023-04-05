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
    public class StoryScene : Scene
    {
        Game1 game;

        Texture2D[] cutScene = new Texture2D[6];

        float elapsed;
        float timer;
        float percent = 0;

        int scene = 1;

        bool start;

        KeyboardState keyboardState;
        KeyboardState old_keyboardState;

        public StoryScene(Game1 game, EventHandler SceneEvent) : base(SceneEvent)
        {
            //bar texture
            cutScene[0] = game.Content.Load<Texture2D>("Assets/Scene/Story/CutScene1");
            cutScene[1] = game.Content.Load<Texture2D>("Assets/Scene/Story/CutScene2");
            cutScene[2] = game.Content.Load<Texture2D>("Assets/Scene/Story/CutScene3");
            cutScene[3] = game.Content.Load<Texture2D>("Assets/Scene/Story/CutScene4");
            cutScene[4] = game.Content.Load<Texture2D>("Assets/Scene/Story/CutScene5");
            cutScene[5] = game.Content.Load<Texture2D>("Assets/Scene/Story/CutScene6");

            start = true;
            this.game = game;
        }
        public override void Update(GameTime gameTime)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            keyboardState = Keyboard.GetState();

            if (elapsed > 0.01 && percent < 1 && start == true)
            {
                elapsed = 0;
                percent += 0.01f;
                if (percent >= 1)
                    start = false;
            }
            else if (elapsed > 0.01 && percent > 0 && start == false && timer >= 4.5)
            {
                elapsed = 0;
                percent -= 0.01f;
                if (percent <= 0)
                {
                    scene++;
                    start = true;
                    timer = 0;
                }
            }

            if(timer >= 3 && scene == 6)
            {
                game.mCurrentScene = game.mGameScene;
            }

            if (keyboardState.IsKeyDown(Keys.X) && old_keyboardState.IsKeyUp(Keys.X))
            {
                elapsed = 0;
                start = true;
                timer = 0;
                game.mCurrentScene = game.mGameScene;
            }

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            switch (scene)
            {
                case 1:
                    spriteBatch.Draw(cutScene[0], new Rectangle(0, 0, 1200, 840), Color.White * percent);
                    spriteBatch.DrawString(game.font, "Nowadays, the land was in turmoil from the Moon King,", new Vector2(30, 30), Color.GhostWhite*percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "the tyrant ruler.", new Vector2(30, 70), Color.GhostWhite * percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    break;
                case 2:
                    spriteBatch.Draw(cutScene[1], new Rectangle(0, 0, 1200, 840), Color.White * percent);
                    spriteBatch.DrawString(game.font, "But this chaos did not keep silent. Someone picked up a sword ", new Vector2(30, 30), Color.GhostWhite * percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "to fight the tyrant.", new Vector2(30, 70), Color.GhostWhite * percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    break;
                case 3:
                    spriteBatch.Draw(cutScene[2], new Rectangle(0, 0, 1200, 840), Color.White * percent);
                    spriteBatch.DrawString(game.font, "With the difference of power, the rebel was outnumbered", new Vector2(30, 30), Color.GhostWhite * percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "by Moonking minions and overwhelmed by their skills.", new Vector2(30, 70), Color.GhostWhite * percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    break;
                case 4:
                    spriteBatch.Draw(cutScene[3], new Rectangle(0, 0, 1200, 840), Color.White * percent);
                    spriteBatch.DrawString(game.font, "Eventually, this liberation attempt did not succeed.", new Vector2(30, 30), Color.GhostWhite * percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    break;
                case 5:
                    spriteBatch.Draw(cutScene[4], new Rectangle(0, 0, 1200, 840), Color.White * percent);
                    spriteBatch.DrawString(game.font, "Only Ronin was able to escape. ", new Vector2(30, 30), Color.GhostWhite * percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    break;
                case 6:
                    spriteBatch.Draw(cutScene[5], new Rectangle(0, 0, 1200, 840), Color.White * percent);
                    spriteBatch.DrawString(game.font, "Even though the sword was broken, his fighting spirit ", new Vector2(30, 30), Color.GhostWhite * percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    spriteBatch.DrawString(game.font, "still lives on. He will fight even if he can't win.", new Vector2(30, 70), Color.GhostWhite * percent, 0.0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.5f);
                    break;

            }
            spriteBatch.End();
        }
    }
}
