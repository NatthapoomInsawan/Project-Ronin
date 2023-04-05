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
    public class Ronin : Sprite
    {

        public string state;
        public int hp;
        public int mhp;
        public float sp;
        public float msp;
        protected float parryElapsed;
        protected float spDrainElapsed;
        protected float stunElapsed;
        protected float attackElapsed;
        protected float blockDelayElapsed;
        protected float hurtDelayElapsed;
        protected float animateElapsed;
        float dashElapsed;
        float dashDelayElapsed;

        protected bool isDead;
        protected bool isBlockCooldown;
        bool isDashCooldown;
        public bool isBoss;

        Vector2 speed = new Vector2( 0.5f, 0);

        public Rectangle attackRectangle;
        public Rectangle parryRectangle;

        KeyboardState keyboardState;
        KeyboardState old_keyboardState;

        public Texture2D barTexture;
        public Texture2D bossBarTexture;

        public SoundEffect attackSound;
        public float volume;

        public Ronin()
        {

        }

        public Ronin(Texture2D Texture, bool direction, Vector2 pos)
            : base (Texture,  direction,  pos)
        {
            spriteWidth = 43;
            spriteHeight = 45;
            totalFrames = 4;
            framePerSec = 4.3f;
            timePerFrame = (float)1 / framePerSec;
            totalElapsed = 0;
            mhp = 100;
            hp = mhp;
            msp = 100;

            texture = Texture;
            isFlip = direction;
            state = "idle";
            position = pos;
            camera = Vector2.Zero;
            scrollingFacter = new Vector2(1.0f, 1); ;
            timePerFrame = (float)1 / framePerSec;
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (hp > 0 && state == "dead")
            {
                hp = mhp;
                state = "idle";
                isDead = false;
                row = 0;
                frame = 0;
            }
            if (hp <= 0)
            {
                hp = 0;
                state = "dead";
            }
            if (sp >= msp)
            {
                state = "stun";
                sp = 0;
            }

            if (isDead != true)
            {
                //move
                if (keyboardState.IsKeyDown(Keys.Left) && position.X > -4 && state != "attack" && state != "block" && state != "hurt" && state != "stun" && state != "dash")
                {
                    isFlip = false;
                    if (position.X - camera.X <= 48 && camera.X > 0)
                    {
                        camera -= speed;
                    }
                    position -= speed;
                    UpdateFrames((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else if (old_keyboardState.IsKeyDown(Keys.Left) && keyboardState.IsKeyUp(Keys.Left) && state != "attack" && state != "hurt" && state != "stun" && state != "dash")
                {
                    frame = 0;
                }

                if (keyboardState.IsKeyDown(Keys.Right) && position.X < 500 - 88 && state != "attack" && state != "block" && state != "hurt" && state != "stun" && state != "dash")
                {
                    isFlip = true;
                    if (position.X - camera.X >= 120 && camera.X < 210)
                    {
                        camera += speed;
                    }
                    position += speed;
                    UpdateFrames((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else if (old_keyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyUp(Keys.Right) && state != "attack" && state != "hurt" && state != "stun" && state != "dash")
                {
                    frame = 0;
                }

                //Dash
                if(keyboardState.IsKeyDown(Keys.Space) && old_keyboardState.IsKeyUp(Keys.Space) && state != "attack" && state != "block" && state != "stun" && state != "hurt"  && state != "dash" && isDashCooldown != true)
                {
                    state = "dash";
                    frame = 0;
                    row = 6;
                    isDashCooldown = true;
                }

                //Attack
                if (keyboardState.IsKeyDown(Keys.Z) && old_keyboardState.IsKeyUp(Keys.Z) && state != "attack" && state != "block" && state != "stun" && state != "dash")
                {
                    state = "attack";
                    frame = 0;
                    row = 1;
                    attackSound.Play(volume: volume, pitch: 0.0f, pan: 0.0f);
                }

                //parry
                if(keyboardState.IsKeyDown(Keys.X) && old_keyboardState.IsKeyUp(Keys.X) && state != "attack" && state != "hurt" && state != "dead" && state != "stun" && isBlockCooldown != true && sp < 100)
                {
                    if (isFlip == true)
                    {
                        parryRectangle = new Rectangle((int)position.X + 28, (int)position.Y + 15, 14, 32);
                    }
                    else
                    {
                        parryRectangle = new Rectangle((int)position.X, (int)position.Y + 15, 14, 32);
                    }
                    Console.WriteLine("parry!");
                }
                //block
                if (keyboardState.IsKeyDown(Keys.X) && state != "attack" && state != "hurt" && state != "dead" && state != "stun" && isBlockCooldown != true && sp < 100)
                {
                    state = "block";
                    frame = 0;
                    totalFrames = 1;
                    row = 4;
                }
                else if (old_keyboardState.IsKeyDown(Keys.X) && keyboardState.IsKeyUp(Keys.X) && state != "attack" && state != "stun")
                {
                    state = "idle";
                    frame = 0;
                    row = 0;
                    totalFrames = 4;
                    isBlockCooldown = true;
                }

                if (state == "hurt")
                {
                    row = 3;
                    frame = 0;
                    totalFrames = 1;
                }

                if (state == "dead")
                {
                    row = 2;
                    frame = 0;
                    totalFrames = 4;
                    isDead = true;
                }

                if (state == "stun")
                {
                    row = 5;
                    frame = 0;
                    totalFrames = 1;
                }

                HurtDelayed((float)gameTime.ElapsedGameTime.TotalSeconds);
                BlockDelayed((float)gameTime.ElapsedGameTime.TotalSeconds);
                DashDelayed((float)gameTime.ElapsedGameTime.TotalSeconds);
                StaggerDrain((float)gameTime.ElapsedGameTime.TotalSeconds);
                Stun((float)gameTime.ElapsedGameTime.TotalSeconds);
                Parry((float)gameTime.ElapsedGameTime.TotalSeconds);
                Attack((float)gameTime.ElapsedGameTime.TotalSeconds);
                Dash((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                Animate((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            old_keyboardState = keyboardState;

        }

        protected virtual void Dash(float elapsed)
        {

            if (state == "dash")
            {
                Console.WriteLine("dashing");
                dashElapsed += elapsed;
                if (dashElapsed > 0.05)
                {
                    frame = (frame + 1) % totalFrames;
                    dashElapsed = 0;
                    if (isFlip == true && position.X + 10 < 412)
                    {
                        if (position.X - camera.X >= 120 && camera.X < 202)
                        {
                            camera += new Vector2(8, 0);
                        }
                        position += new Vector2(10, 0);
                    }
                    else if (isFlip != true && position.X - 10 > -4)
                    {
                        
                        if (position.X - camera.X <= 48 && camera.X > 8)
                        {
                            camera += new Vector2(-8, 0);
                        }
                        position += new Vector2(-10, 0);
                    }
                    if (frame == 3)
                    {
                        frame = 0;
                        row = 0;
                        state = "idle";
                    }
                }
            }

        }

        protected virtual void Attack(float elapsed)
        {
            if (attackRectangle != Rectangle.Empty)
            {
                attackRectangle = Rectangle.Empty;
            }

            if (state == "attack")
            {
                attackElapsed += elapsed;
                totalFrames = 4;
                row = 1;

                if (attackElapsed > timePerFrame)
                {
                    frame = (frame + 1) % totalFrames;
                    attackElapsed = 0;
                    if (frame == 2)
                    {
                        if (isFlip == true)
                        {
                            attackRectangle = new Rectangle((int)position.X + 28, (int)position.Y + 15, 14, 32);
                        }
                        else
                        {
                            attackRectangle = new Rectangle((int)position.X, (int)position.Y + 15, 14, 32);
                        }
                    }
                    if (frame == 3)
                    {
                        attackRectangle = Rectangle.Empty;
                        frame = 0;
                        row = 0;
                        state = "idle";
                    }
                }
            }
            
        }

        protected virtual void HurtDelayed(float elapsed)
        {
            if (state == "hurt")
            {
                if (stunElapsed != 0)
                    stunElapsed = 0;
                hurtDelayElapsed += elapsed;
                if (hurtDelayElapsed > 0.5)
                {
                    if (hp > 0)
                    {
                        row = 0;
                        frame = 0;
                        totalFrames = 4;
                        state = "idle";
                        hurtDelayElapsed = 0;
                    }
                    else
                    {
                        state = "dead";
                    }
                }
            }
        }

        protected void BlockDelayed(float elapsed)
        {
            blockDelayElapsed += elapsed;
            if (blockDelayElapsed > 0.30)
            {
                blockDelayElapsed = 0;
                isBlockCooldown = false;
            }
        }

        protected void DashDelayed(float elapsed)
        {
            dashDelayElapsed += elapsed;
            if (dashDelayElapsed > 2.5)
            {
                dashDelayElapsed = 0;
                isDashCooldown = false;
            }
        }

        protected void StaggerDrain(float elapsed)
        {
            spDrainElapsed += elapsed;
            if (spDrainElapsed > 2 && sp > 0 && state != "hurt")
            {
                if (hp > 50 * mhp / 100)
                {
                    sp -= 0.25f;
                }
                else if (hp >= 30 * mhp / 100)
                {
                    sp -= 0.15f;
                }
                else if (hp >= 10 * mhp / 100)
                {
                    sp -= 0.1f;
                }
                else
                {
                    sp -= 0.06f;
                }
            }

        }

        protected virtual void Stun(float elapsed)
        {
            if (state == "stun")
            {
                stunElapsed += elapsed;
                if (stunElapsed > 1)
                {
                    stunElapsed = 0;
                    if (hp > 0)
                    {
                        row = 0;
                        frame = 0;
                        totalFrames = 4;
                        state = "idle";
                    }
                    else
                        state = "dead";
                }
            }
        }

        protected virtual void Parry(float elapsed)
        {
            parryElapsed += elapsed;

            if (parryElapsed > 0.25)
            {
                if (parryRectangle != Rectangle.Empty)
                {
                    parryRectangle = Rectangle.Empty;
                    Console.WriteLine("parryDisposed!");
                }
                parryElapsed = 0;
            }
        }
        protected virtual void Animate(float elapsed)
        {
            animateElapsed += elapsed;
            if (animateElapsed > 0.2)
            {
                if (isDead == true && frame != 3)
                    frame = (frame + 1) % totalFrames;
                else
                {
                    state = "dead";
                    frame = 3;
                }
                animateElapsed = 0;
            }
        }

        public void DrawBar(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (isBoss != true)
            {
                if (isFlip == true && isDead != true)
                {
                    //bound
                    spriteBatch.Draw(barTexture, ((new Vector2(position.X + 7, position.Y)) - camera), new Rectangle(20 * 0, 2 * 0, 20, 2), new Color(186, 104, 108));
                    spriteBatch.Draw(barTexture, ((new Vector2(position.X + 7, position.Y + 2)) - camera), new Rectangle(20 * 0, 2 * 0, 20, 2), new Color(179, 144, 90));
                    //hp
                    spriteBatch.Draw(barTexture, ((new Vector2(position.X + 7, position.Y)) - camera), new Rectangle(20 * 0, 2 * 1, (int)Math.Ceiling((double)barTexture.Width * hp / mhp), 2), new Color(186, 104, 108));
                    //sp
                    spriteBatch.Draw(barTexture, ((new Vector2(position.X + 7, position.Y + 2)) - camera), new Rectangle(20 * 0, 2 * 1, (int)Math.Ceiling((double)barTexture.Width * (sp / msp)), 2), new Color(179, 144, 90));
                }
                else if (isDead != true)
                {
                    //bound
                    spriteBatch.Draw(barTexture, ((new Vector2(position.X + 15, position.Y)) - camera), new Rectangle(20 * 0, 2 * 0, 20, 2), new Color(186, 104, 108));
                    spriteBatch.Draw(barTexture, ((new Vector2(position.X + 15, position.Y + 2)) - camera), new Rectangle(20 * 0, 2 * 0, 20, 2), new Color(179, 144, 90));
                    //hp
                    spriteBatch.Draw(barTexture, ((new Vector2(position.X + 15, position.Y)) - camera), new Rectangle(20 * 0, 2 * 1, (int)Math.Ceiling((double)barTexture.Width * hp / mhp), 2), new Color(186, 104, 108));
                    //sp
                    spriteBatch.Draw(barTexture, ((new Vector2(position.X + 15, position.Y + 2)) - camera), new Rectangle(20 * 0, 2 * 1, (int)Math.Ceiling((double)barTexture.Width * (sp / msp)), 2), new Color(179, 144, 90));
                }
            }
            else if (isBoss == true)
            {
                //bound
                spriteBatch.Draw(bossBarTexture, new Vector2( 45, 30), new Rectangle(150 * 0, 4 * 0, 150, 4), Color.White);
                spriteBatch.Draw(bossBarTexture, new Vector2( 45, 34), new Rectangle(150 * 0, 4 * 0, 150, 4), Color.White);
                //hp
                spriteBatch.Draw(bossBarTexture, new Vector2(45, 30), new Rectangle(150 * 0, 4 * 1, (int)Math.Ceiling((double)bossBarTexture.Width * hp / mhp), 4), new Color(186, 104, 108));
                spriteBatch.Draw(bossBarTexture, new Vector2(45, 34), new Rectangle(150 * 0, 4 * 1, (int)Math.Ceiling((double)bossBarTexture.Width * (sp / msp)), 4), new Color(179, 144, 90));
            }
            
            spriteBatch.End();
        }
    }
}
