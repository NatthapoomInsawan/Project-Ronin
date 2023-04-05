using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Project_Ronin
{
    public class Enemy : Ronin
    {

        public string playerState;
        public string type;

        int reach;
        int reload = 0;
        public int charge = 0;

        float blockElapsed;
        float delayElapsed;
        float reloadElapsed;
        float hurtDelay;
        float attackCooldown;

        bool isATKCooldown;
        bool isReload;
        bool canBlock;
        bool isBlock;
        public bool warning;

        public Vector2 playerPosition;
        Vector2 speed = new Vector2(0.4f, 0);

        Random random = new Random();


        public Enemy(Texture2D Texture, bool direction, Vector2 pos, Vector2 scrollFac)
        {
            spriteWidth = 43;
            spriteHeight = 45;
            totalFrames = 4;
            framePerSec = 4.3f;
            timePerFrame = (float)1 / framePerSec;
            totalElapsed = 0;

            texture = Texture;
            isFlip = direction;
            state = "idle";
            position = pos;
            camera = Vector2.Zero;
            scrollingFacter = new Vector2(1.0f, 1); ;
            timePerFrame = (float)1 / framePerSec;
            scrollingFacter = scrollFac;

        }

        public override void Update(GameTime gameTime)
        {
            if(hp <= 0)
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
                EnemyAI(playerPosition);

                if (state == "walk")
                {
                    UpdateFrames((float)gameTime.ElapsedGameTime.TotalSeconds);
                }


                if (isATKCooldown == true)
                {
                    Delayed((float)gameTime.ElapsedGameTime.TotalSeconds);
                }

                if (isReload == true && state != "hurt")
                {
                    totalFrames = 8;
                    row = 5;
                    Reload((float)gameTime.ElapsedGameTime.TotalSeconds);
                }

                if (state == "hurt")
                {
                    row = 3;
                    frame = 0;
                    totalFrames = 1;
                    reload = 0;
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
                    row = 4;
                    frame = 0;
                    totalFrames = 1;
                    charge = 0;
                    warning = false;
                }

                Block((float)gameTime.ElapsedGameTime.TotalSeconds);
                HurtDelayed((float)gameTime.ElapsedGameTime.TotalSeconds);
                StaggerDrain((float)gameTime.ElapsedGameTime.TotalSeconds);
                Stun((float)gameTime.ElapsedGameTime.TotalSeconds);
                Attack((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
                Animate((float)gameTime.ElapsedGameTime.TotalSeconds);

        }

        private void EnemyAI(Vector2 playerPos)
        {
            if (state != "dead" && playerState != "dead")
            {
                //turning
                if ((playerPos.X - position.X) >= 0 && state != "attack" && state != "stun" && state != "reload" && isReload != true)
                {
                    isFlip = true;
                }
                else if ((playerPos.X - position.X) < 0 && state != "attack" && state != "stun" && state != "reload" && isReload != true)
                {
                    isFlip = false;
                }

                //Searching
                if (isFlip == true && playerPosition.X > position.X + reach && state != "attack" && state != "hurt" && state != "stun" && state != "reload" && state != "block" && isReload != true)
                {
                    state = "walk";
                    position += speed;
                }
                else if (isFlip == false && playerPosition.X < position.X - reach && state != "attack" && state != "hurt" && state != "stun" && state != "reload" && state != "block" && isReload != true)
                {
                    state = "walk";
                    position -= speed;
                }
                else if (state != "attack" && state != "hurt" && state != "stun" && state != "reload" && state != "block" && isReload != true && isBlock != true)
                {
                    state = "idle";
                    frame = 0;
                }

                //block
                if (canBlock == true && (playerPos.X - position.X) <= reach && playerState == "attack" && state != "attack" && state != "walk" && state != "hurt" && state != "stun" && state != "reload" && isBlock != true && sp < msp)
                {
                    switch (random.Next(1, 11))
                    {
                        case 1:
                            state = "block";
                            row = 5;
                            frame = 0;
                            isBlock = true;
                            break;
                    }
                }

                //attack
                if ((playerPos.X - position.X) <= reach && state != "attack" && state != "walk" && state != "hurt" && state != "stun" && state != "reload" && state != "block" && isATKCooldown == false && isReload == false && isBlock == false)
                {
                    state = "attack";
                    if (type == "enbu" && charge < 6)
                    {
                        charge++;
                    }

                    if (charge >= 0 && charge < 6)
                    {
                        row = 1;
                    }
                    else if (charge == 6)
                    {
                        row = 6;
                    }

                    frame = 0;
                    attackSound.Play(volume: volume, pitch: 0.0f, pan: 0.0f);
                }
            }

        }

        public void SetEnemyType(string enemyType)
        {
            type = enemyType;
            if (type == "samurai")
            {
                reach = 23;
                attackCooldown = 0.5f;
                hurtDelay = 0.5f;
                msp = 60;
                mhp = 100;
                hp = mhp;
            }
            else if (type == "ashigaru")
            {
                reach = 80;
                attackCooldown = 0.5f;
                hurtDelay = 0.5f;
                msp = 60;
                mhp = 60;
                hp = mhp;
            }
            else if (type == "matsu")
            {
                reach = 23;
                attackCooldown = 0.5f;
                hurtDelay = 0.5f;
                msp = 100;
                mhp = 300;
                hp = mhp;
                isBoss = true;
                canBlock = true;
            }
            else if (type == "roland")
            {
                reach = 23;
                attackCooldown = 0.5f;
                hurtDelay = 0.5f;
                msp = 150;
                mhp = 60;
                hp = mhp;
                isBoss = true;
                canBlock = true;
            }
            else if (type == "enbu")
            {
                reach = 23;
                attackCooldown = 1f;
                hurtDelay = 0.5f;
                msp = 100;
                mhp = 550;
                hp = mhp;
                isBoss = true;
                canBlock = true;
            }
        }

        protected override void Attack(float elapsed)
        {
            if (attackRectangle != Rectangle.Empty)
            {
                attackRectangle = Rectangle.Empty;
            }
            if (state == "attack")
            {
                attackElapsed += elapsed;

                if (attackElapsed > timePerFrame)
                {
                    frame = (frame + 1) % totalFrames;
                    attackElapsed = 0;
                    if (frame == 2)
                    {
                        if (isFlip == true)
                        {
                            if (type == "ashigaru")
                            {
                                attackRectangle = new Rectangle((int)playerPosition.X + 12, (int)playerPosition.Y + 14, 15, 32);
                            }
                            else
                                attackRectangle = new Rectangle((int)position.X + 28, (int)position.Y + 15, 14, 32);
                        }
                        else
                        {
                            if (type == "ashigaru")
                            {
                                attackRectangle = new Rectangle((int)playerPosition.X + 12, (int)playerPosition.Y + 14, 15, 32);
                            }
                            else
                                attackRectangle = new Rectangle((int)position.X, (int)position.Y + 15, 14, 32);
                        }
                    }
                    if (frame == 3)
                    {
                        if (charge == 6)
                        {
                            charge = 0;
                            warning = false;
                        }
                        if (charge == 5)
                            warning = true;
                        row = 0;
                        frame = 0;
                        state = "idle";
                        if (type == "ashigaru")
                        {
                            row = 5;
                            frame = 0;
                            isReload = true;
                            state = "reload";
                        }
                        else
                            isATKCooldown = true;
                    }
                }
            }
            
        }

        protected void Reload(float elapsed)
        {
            reloadElapsed += elapsed;
            if (reload == 0)
            {
                frame = reload;
            }
            if (reloadElapsed > 0.6)
            {
                frame = (frame + 1) % totalFrames;
                reloadElapsed = 0;
                reload++;
                if (frame == 7 && reload == 7)
                {
                    reload = 0;
                    row = 0;
                    frame = 0;
                    totalFrames = 4;
                    state = "idle";
                    isATKCooldown = true;
                    isReload = false;
                }
            }
        }

        protected void Delayed(float elapsed)
        {
            delayElapsed += elapsed;
            if (delayElapsed > attackCooldown)
            {
                isATKCooldown = false;
                delayElapsed = 0;
                Console.WriteLine("Delayed!");
            }
        }

        protected void Block(float elapsed)
        {

            if (state == "stun" && isBlock != false)
            {
                isBlock = false;
            }

            if (state == "block" && playerState != "attack")
            {
                isBlock = false;
                blockElapsed = 0;
                state = "idle";
                row = 0;
                frame = 0;
            }
            else if (state == "block")
            {
                blockElapsed += elapsed;
                if (blockElapsed > 3)
                {
                    isBlock = false;
                    blockElapsed = 0;
                    state = "idle";
                    row = 0;
                    frame = 0;
                }
            }
        }

    }
}
