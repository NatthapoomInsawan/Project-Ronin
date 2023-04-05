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
    public class StudioScene : Scene
    {
        Game1 game;

        Texture2D studioTexture;

        float elapsed;
        float timer;
        float percent = 0;

        bool start;

        public StudioScene(Game1 game, EventHandler SceneEvent) : base(SceneEvent)
        {
            //bar texture
            studioTexture = game.Content.Load<Texture2D>("Assets/HUD/StudioLogo");

            start = true;
            this.game = game;
        }
        public override void Update(GameTime gameTime)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsed > 0.01 && percent < 1 && start == true)
            {
                elapsed = 0;
                percent += 0.01f;
                if (percent >= 1)
                    start = false;
            }
            else if (elapsed > 0.01 && percent > 0 && start == false && timer >= 3.5)
            {
                elapsed = 0;
                percent -= 0.01f;
            }

            if (timer > 6)
            {
                game.mCurrentScene = game.mTitleScene;
            }

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(studioTexture, new Rectangle(  0, 0, 1200, 840), Color.White * percent);
            spriteBatch.End();
        }
    }
}
