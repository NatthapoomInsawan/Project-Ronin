using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Ronin
{
    public class Sprite
    {

        protected int spriteWidth;
        protected int spriteHeight;
        public int frame;
        public int scale = 1;
        protected int row;
        protected int totalFrames;
        protected float framePerSec;
        protected float timePerFrame;
        protected float totalElapsed = 0;

        public bool isActive = false;
        public bool isFlip = false;

        public Texture2D texture;
        public Vector2 position;
        public Vector2 scrollingFacter;
        public Vector2 camera;

        public Sprite()
        {

        }

        public Sprite(Texture2D Texture, bool direction, Vector2 pos)
        {
            texture = Texture;
            isFlip = direction;
            position = pos;
            timePerFrame = (float)1 / framePerSec;

        }

        public Sprite(Texture2D Texture, bool direction, Vector2 pos, Vector2 scrollFac)
        {
            texture = Texture;
            isFlip = direction;
            position = pos;
            timePerFrame = (float)1 / framePerSec;
            scrollingFacter = scrollFac;

        }

        public Sprite(Texture2D Texture, bool direction, Vector2 pos, int width, int height, int TotalFrame, int Row, float fps, Vector2 Factor)
        {
            texture = Texture;
            isFlip = direction;
            position = pos;
            spriteWidth = width;
            spriteHeight = height;
            totalFrames = TotalFrame;
            framePerSec = fps;
            timePerFrame = (float)1 / framePerSec;
            row = Row;
            scrollingFacter = Factor;
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateFrames((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if ( scrollingFacter != Vector2.Zero)
            {
                if (isFlip == true)
                    spriteBatch.Draw(texture, (position - camera) * scrollingFacter, new Rectangle(spriteWidth * frame, spriteHeight * row, spriteWidth, spriteHeight), Color.White, 0.0f, new Vector2(0, 0), scale, SpriteEffects.FlipHorizontally, 0.5f);
                else
                    spriteBatch.Draw(texture, (position - camera) * scrollingFacter, new Rectangle(spriteWidth * frame, spriteHeight * row, spriteWidth, spriteHeight), Color.White, 0.0f, new Vector2(0, 0), scale, SpriteEffects.None, 0.5f);
            }
            else
            {
                if (isFlip == true)
                    spriteBatch.Draw(texture, (position - camera), new Rectangle(spriteWidth * frame, spriteHeight * row, spriteWidth, spriteHeight), Color.White, 0.0f, new Vector2(0, 0), scale, SpriteEffects.FlipHorizontally, 0.5f);
                else
                    spriteBatch.Draw(texture, (position - camera), new Rectangle(spriteWidth * frame, spriteHeight * row, spriteWidth, spriteHeight), Color.White, 0.0f, new Vector2(0, 0), scale, SpriteEffects.None, 0.5f);
            }            
            spriteBatch.End();
        }

        protected void UpdateFrames(float elapsed)
        {
            totalElapsed += elapsed;
            if (totalElapsed > timePerFrame)
            {
                frame = (frame + 1) % totalFrames;
                totalElapsed = 0;
            }
        }
    }
}
