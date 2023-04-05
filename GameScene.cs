using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace Project_Ronin
{
    public class GameScene : Scene
    {
        Game1 game;

        Texture2D _texture;

        public bool menuToggle;
        public bool bgmPlay;

        float sfxVolume;
        float bgmVolume;

        int difficultyMod = 0;
        int enemyNumber;
        public int enemyLeft;
        public int wave = 1;
        public int progress;
        public int menuIndex;
        int samuraiNum = 0;
        int ashigaruNum = 0;
        int bossNum = 0;

        Texture2D[] layerTexture = new Texture2D[4];
        Texture2D grassTexture;
        Texture2D barTexture;
        Texture2D bossBarTexture;
        Texture2D samuraiTexture;
        Texture2D ashigaruTexture;
        Texture2D matsuTexture;
        Texture2D rolandTexture;
        Texture2D enbuTexture;
        Texture2D roninTexture;
        Texture2D ingameMenu;
        Texture2D warning;

        Random random = new Random();

        Sprite[] grass = new Sprite[21];
        Sprite[] layer = new Sprite[4];
        public Ronin ronin;
        public List<Enemy> enemy = new List<Enemy>();
        
        List<SoundEffect> soundEffects = new List<SoundEffect>();
        List<Song> backgroundMusics = new List<Song>();
        SoundEffect menuMove;
        SoundEffect menuSelect;

        Rectangle playerRectangle;
        Rectangle enemyRectangle;

        Vector2[] grassPosition = new Vector2[34];
        Vector2 roninPosition = new Vector2(95, 75);
        public Vector2 cameraPosition = new Vector2(0, 0);
        Vector2 grass_scrollFactor = new Vector2(0.8f, 1);
        Vector2[] layer_scrollFactor = new Vector2[4];

        KeyboardState keyboardState;
        KeyboardState old_keyboardState;

        public GameScene (Game1 game, EventHandler SceneEvent) : base(SceneEvent)
        {
            //Load the background texture for the screen
            _texture = new Texture2D(game.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray });
            //HUD
            barTexture = game.Content.Load<Texture2D>("Assets/HUD/Bar");
            bossBarTexture = game.Content.Load<Texture2D>("Assets/HUD/BossBar");
            ingameMenu = game.Content.Load<Texture2D>("Assets/HUD/InGameMenu");
            warning = game.Content.Load<Texture2D>("Assets/HUD/Warning");

            //Scene
            layerTexture[0] = game.Content.Load<Texture2D>("Assets/Scene/Sky");
            layerTexture[1] = game.Content.Load<Texture2D>("Assets/Scene/Background_Mountain");
            layerTexture[2] = game.Content.Load<Texture2D>("Assets/Scene/Mid_Mountain");
            layerTexture[3] = game.Content.Load<Texture2D>("Assets/Scene/Ground");
            grassTexture = game.Content.Load<Texture2D>("Assets/Scene/Grass_sprite");

            //sound
            soundEffects.Add(game.Content.Load<SoundEffect>("Assets/Sound/SFX/Parry"));
            soundEffects.Add(game.Content.Load<SoundEffect>("Assets/Sound/SFX/Block"));
            soundEffects.Add(game.Content.Load<SoundEffect>("Assets/Sound/SFX/SwordSwing"));
            soundEffects.Add(game.Content.Load<SoundEffect>("Assets/Sound/SFX/GunShot"));
            soundEffects.Add(game.Content.Load<SoundEffect>("Assets/Sound/SFX/Hurt"));
            soundEffects.Add(game.Content.Load<SoundEffect>("Assets/Sound/SFX/ArmorClash"));
            soundEffects.Add(game.Content.Load<SoundEffect>("Assets/Sound/SFX/Shield"));
            soundEffects.Add(game.Content.Load<SoundEffect>("Assets/Sound/SFX/RolandAttack"));
            soundEffects.Add(game.Content.Load<SoundEffect>("Assets/Sound/SFX/Onihit"));

            //menu sound
            menuMove = game.mTitleScene.menuMove;
            menuSelect = game.mTitleScene.menuSelect;

            //backgroundMusic
            backgroundMusics.Add(game.Content.Load<Song>("Assets/Sound/BGM/Ibuki"));

            //Sound setting
            bgmVolume = (float)game.mOptionScene.bgmVolume;
            sfxVolume = (float)game.mOptionScene.sfxVolume;

            for (int i = 0; i < 21; i++)
            {
                int row = i % 4;

                grassPosition[i] = new Vector2((24 * i), 96);
                grass[i] = new Sprite(grassTexture, false, grassPosition[i], 24, 24, 4, row, 1.4f, grass_scrollFactor);
            }

            layer_scrollFactor[0] = new Vector2(0.3f,1);
            layer_scrollFactor[1] = new Vector2(0.4f, 1);
            layer_scrollFactor[2] = new Vector2(0.5f, 1);
            layer_scrollFactor[3] = new Vector2(0.8f, 1);

            for (int i = 0; i < 4; i++)
            {
                layer[i] = new Sprite(layerTexture[i], false, new Vector2( 0, 0), 500, 168, 1, 0, 1f, layer_scrollFactor[i]);
            }

            //Character
            roninTexture = game.Content.Load<Texture2D>("Assets/Character/Ronin/Ronin_sprite");
            ronin = new Ronin(roninTexture, false, roninPosition);
            ronin.barTexture = barTexture;
            ronin.attackSound = soundEffects[2];

            //Enemy
            samuraiTexture = game.Content.Load<Texture2D>("Assets/Character/Enemy/Samurai_sprite");
            ashigaruTexture = game.Content.Load<Texture2D>("Assets/Character/Enemy/Ashigaru_sprite");
            matsuTexture = game.Content.Load<Texture2D>("Assets/Character/Enemy/Matsu_sprite");
            rolandTexture = game.Content.Load<Texture2D>("Assets/Character/Enemy/Roland_sprite");
            enbuTexture = game.Content.Load<Texture2D>("Assets/Character/Enemy/Enbu_sprite");

            //Spawning Enemy
            EnemySpawn();
            this.game = game;

        }

        public override void Update(GameTime gameTime)
        {

            keyboardState = Keyboard.GetState();
            for (int i = 0; i < 21; i++)
            {
                grass[i].camera = cameraPosition;
                grass[i].Update(gameTime);
            }

            for (int i = 0; i < 4; i++)
            {
                layer[i].camera = cameraPosition;
                layer[i].Update(gameTime);
            }

            ronin.Update(gameTime);
            cameraPosition = ronin.camera;
            roninPosition = ronin.position;

            for (int i = 0; i < enemy.Count; i++)
            {
                enemy[i].camera = cameraPosition;
                enemy[i].playerPosition = roninPosition;
                enemy[i].playerState = ronin.state;
                enemy[i].Update(gameTime);

            }

            //Player Rectangle
            if (ronin.state != "dead" && ronin.state != "dash")
            {
                if (ronin.isFlip == true)
                {
                    playerRectangle = new Rectangle((int)ronin.position.X + 12, (int)ronin.position.Y + 14, 15, 32);
                }
                else
                {
                    playerRectangle = new Rectangle((int)ronin.position.X + 18, (int)ronin.position.Y + 14, 15, 32);
                }
            }
            else
                playerRectangle = Rectangle.Empty;


            InGameMenu();

            CollisionDetection();


            //Wave End
            if (enemyLeft == 0 && ronin.hp > 0)
            {
                menuToggle = false;
                if (keyboardState.IsKeyDown(Keys.Z) && old_keyboardState.IsKeyUp(Keys.Z))
                {
                    enemy.Clear();
                    ronin.hp = ronin.mhp;
                    ronin.sp = 0;
                    wave++;
                    if (wave % 10 == 0)
                        difficultyMod++;
                    ronin.position = new Vector2(95, 75);
                    ronin.camera = new Vector2( 0, 0);
                    EnemySpawn();
                    if (wave > progress)
                        progress = wave;
                    game.SaveData();
                }
                else if (keyboardState.IsKeyDown(Keys.Escape) && old_keyboardState.IsKeyUp(Keys.Escape))
                {
                    enemyLeft = 0;
                    if (wave > progress)
                        progress = wave;
                    game.SaveData();
                    game.mCurrentScene = game.mTitleScene;
                    MediaPlayer.Pause();

                }
            }
            else if (ronin.hp <= 0)
            {
                menuToggle = false;
                if (keyboardState.IsKeyDown(Keys.Z) && old_keyboardState.IsKeyUp(Keys.Z))
                {
                    enemy.Clear();
                    ronin.hp = ronin.mhp;
                    ronin.sp = 0;
                    wave = 1;
                    difficultyMod = 0;
                    ronin.position = new Vector2(95, 75);
                    ronin.camera = new Vector2(0, 0);
                    EnemySpawn();
                    if (wave > progress)
                        progress = wave;
                    game.SaveData();
                }
                else if (keyboardState.IsKeyDown(Keys.Escape) && old_keyboardState.IsKeyUp(Keys.Escape))
                {
                    enemyLeft = 0;
                    difficultyMod = 0;
                    bgmPlay = false;
                    MediaPlayer.Stop();
                    if (wave > progress)
                        progress = wave;
                    game.SaveData();
                    game.mCurrentScene = game.mTitleScene;
                }
            }
            old_keyboardState = keyboardState;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            for (int i = 0; i < 4; i++)
            {
                layer[i].Draw(spriteBatch);
            }

            for (int i = 0; i < 21; i++)
            {
                grass[i].Draw(spriteBatch);
            }


            //Draw Enemy
            for (int i = 0; i < enemy.Count; i++)
            {
                enemy[i].Draw(spriteBatch);
                enemy[i].DrawBar(spriteBatch);
            }

            //Draw Ronin
            ronin.Draw(spriteBatch);
            ronin.DrawBar(spriteBatch);

            //Draw Menu
            if (menuToggle == true)
            {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                spriteBatch.Draw(ingameMenu, new Rectangle( 0, 0, 240, 168), Color.White);
                spriteBatch.End();
            }

            //Draw Warning
            if (enemy[0].type == "enbu" && enemy[0].warning == true)
            {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                if (enemy[0].isFlip == true)
                    spriteBatch.Draw(warning, ((new Vector2((int)enemy[0].position.X+5, (int)enemy[0].position.Y - 25)) - enemy[0].camera), new Rectangle(0, 0, 22, 22), new Color(212, 78, 83) * 0.8f);
                else
                    spriteBatch.Draw(warning, ((new Vector2((int)enemy[0].position.X + 15, (int)enemy[0].position.Y - 25)) - enemy[0].camera), new Rectangle(0, 0, 22, 22), new Color(212, 78, 83) * 0.8f);
                spriteBatch.End();
            }

            //TEST PARRY HITBOX
            /*
           spriteBatch.Begin();
           if (ronin.isFlip == true)
               spriteBatch.Draw(_texture, ((new Vector2((int)ronin.position.X + 28, (int)ronin.position.Y + 15)) - cameraPosition), ronin.parryRectangle, Color.White, 0.0f, new Vector2(0, 0), 1, SpriteEffects.FlipHorizontally, 0.5f);
           else
               spriteBatch.Draw(_texture, ((new Vector2((int)ronin.position.X, (int)ronin.position.Y + 15)) - cameraPosition), ronin.parryRectangle, Color.White, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
           spriteBatch.End();
           base.Draw(spriteBatch);
           */

            //TEST ATTACK HITBOX
            /*
            spriteBatch.Begin();
            if (ronin.isFlip == true)
                spriteBatch.Draw(_texture, ((new Vector2((int)ronin.position.X + 28, (int)ronin.position.Y + 15)) - cameraPosition), ronin.attackRectangle, Color.Red, 0.0f, new Vector2(0, 0), 1, SpriteEffects.FlipHorizontally, 0.5f);
            else
                spriteBatch.Draw(_texture, ((new Vector2((int)ronin.position.X, (int)ronin.position.Y + 15)) - cameraPosition), ronin.attackRectangle, Color.Red, 0.0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.5f);
            spriteBatch.End();
            base.Draw(spriteBatch);
            */
        }

        private void InGameMenu()
        {

            //Ingame Menu
            if (keyboardState.IsKeyDown(Keys.Escape) && old_keyboardState.IsKeyUp(Keys.Escape) && enemyLeft != 0 && ronin.hp > 0)
            {
                if (menuToggle == true)
                    menuToggle = false;
                else
                    menuToggle = true;
                menuIndex = 0;
                menuSelect.Play();
            }
            //Select Menu
            if (menuToggle == true)
            {
                //Scroll
                if (keyboardState.IsKeyDown(Keys.Down) && old_keyboardState.IsKeyUp(Keys.Down))
                {
                    if (menuIndex < 2)
                    {
                        menuIndex++;
                        menuMove.Play();
                    }
                }
                if (keyboardState.IsKeyDown(Keys.Up) && old_keyboardState.IsKeyUp(Keys.Up))
                {
                    if (menuIndex > 0)
                    {
                        menuIndex--;
                        menuMove.Play();
                    }
                }

                if (keyboardState.IsKeyDown(Keys.Z) && old_keyboardState.IsKeyUp(Keys.Z))
                {
                    switch (menuIndex)
                    {
                        case 0:
                            game.mCurrentScene = game.mTitleScene;
                            game.SaveData();
                            MediaPlayer.Pause();
                            break;
                        case 1:
                            game.mOptionScene.previousScene = game.mGameScene;
                            game.mCurrentScene = game.mOptionScene;
                            game.SaveData();
                            MediaPlayer.Pause();
                            break;
                        case 2:
                            MediaPlayer.Pause();
                            game.SaveData();
                            game.Exit();
                            break;
                    }
                    menuSelect.Play();
                    menuIndex = 0;
                    menuToggle = false;
                }
            }
        }

        private void CollisionDetection()
        {
            //collision detection
            //Enemy
            for (int i = 0; i < enemy.Count; i++)
            {
                //enemyRectangle

                if (enemy[i].state != "dead")
                {
                    if (enemy[i].isFlip == true)
                    {
                        enemyRectangle = new Rectangle((int)enemy[i].position.X + 12, (int)enemy[i].position.Y + 14, 15, 32);
                    }
                    else
                    {
                        enemyRectangle = new Rectangle((int)enemy[i].position.X + 18, (int)enemy[i].position.Y + 14, 15, 32);
                    }
                }
                else
                    enemyRectangle = Rectangle.Empty;

                //enemyAttack hit player
                if (enemy[i].attackRectangle.Intersects(playerRectangle) && enemy[i].attackRectangle.Intersects(ronin.parryRectangle) != true)
                {
                    enemy[i].attackRectangle = Rectangle.Empty;
                    Console.WriteLine("hit!");
                    if (ronin.state == "block" && ronin.isFlip != enemy[i].isFlip)
                    {
                        if (enemy[i].type == "ashigaru")
                        {
                            if (ronin.sp + 5 >= ronin.msp)
                            {
                                ronin.sp = ronin.msp;
                            }
                            else
                                ronin.sp += 5;
                        }
                        else if (enemy[i].type == "enbu" && enemy[i].charge == 6)
                        {
                            if (ronin.sp + 45 >= ronin.msp)
                            {
                                ronin.sp = ronin.msp;
                            }
                            else
                                ronin.sp += 45;
                            ronin.hp -= 40;
                            ronin.state = "hurt";
                        }
                        else
                        {
                            if (ronin.sp + 35 >= ronin.msp)
                            {
                                ronin.sp = ronin.msp;
                            }
                            else
                                ronin.sp += 35;
                        }

                        //knock back
                        if (ronin.isFlip && ronin.position.X - 5 > -4)
                        {
                            ronin.position += new Vector2(-5, 0);
                        }
                        else if (ronin.isFlip != true && ronin.position.X + 5 < 412)
                        {
                            ronin.position += new Vector2(5, 0);
                        }
                        Console.WriteLine("player is blocking! SP:" + ronin.sp.ToString());
                        soundEffects[1].Play(volume: sfxVolume, pitch: 0.0f, pan: 0.0f);
                    }
                    else
                    {
                        if (enemy[i].type == "ashigaru")
                        {
                            if (ronin.state != "stun")
                            {
                                ronin.state = "hurt";
                            }
                            ronin.hp -= 20;
                            soundEffects[4].Play(volume: sfxVolume, pitch: 0.0f, pan: 0.0f);
                        }
                        else if(enemy[i].type == "enbu")
                        {
                            if (ronin.state != "stun")
                            {
                                ronin.state = "hurt";
                            }
                            ronin.hp -= 35;
                            soundEffects[8].Play(volume: sfxVolume, pitch: 0.0f, pan: 0.0f);
                        }
                        else
                        {
                            if (ronin.state != "stun")
                            {
                                ronin.state = "hurt";
                            }
                            ronin.hp -= 35;
                            soundEffects[4].Play(volume: sfxVolume, pitch: 0.0f, pan: 0.0f);
                        }
                        Console.WriteLine("player HP:" + ronin.hp.ToString());
                    }
                }

                //playerAttack hit enemy
                if (ronin.attackRectangle.Intersects(enemyRectangle))
                {
                    //ronin.attackRectangle = Rectangle.Empty;
                    Console.WriteLine("Enemy hit!");
                    if (enemy[i].state == "block" && ronin.isFlip != enemy[i].isFlip)
                    {
                        if (enemy[i].type == "roland")
                        {
                            if (enemy[i].sp + 10 >= enemy[i].msp)
                            {
                                enemy[i].sp = enemy[i].msp;
                            }
                            else
                                enemy[i].sp += 10;
                            soundEffects[6].Play(volume: sfxVolume, pitch: 0.0f, pan: 0.0f);
                        }
                        else
                        {
                            if (enemy[i].sp + 25 >= enemy[i].msp)
                            {
                                enemy[i].sp = enemy[i].msp;
                            }
                            else
                                enemy[i].sp += 25;
                            soundEffects[1].Play(volume: sfxVolume, pitch: 0.0f, pan: 0.0f);
                        }
                    }
                    else
                    {
                        if (enemy[i].type == "roland" && enemy[i].state != "stun")
                        {
                            enemy[i].state = "hurt";
                            if (enemy[i].sp + 10 >= enemy[i].msp)
                            {
                                enemy[i].sp = enemy[i].msp;
                            }
                            else
                                enemy[i].sp += 10;
                            soundEffects[5].Play(volume: sfxVolume, pitch: 0.0f, pan: 0.0f);
                        }
                        else
                        {
                            enemy[i].state = "hurt";
                            enemy[i].hp -= 35;
                            soundEffects[4].Play(volume: sfxVolume, pitch: 0.0f, pan: 0.0f);
                        }
                    }

                    //Enemy death
                    if (enemy[i].hp <= 0)
                    {
                        enemyLeft--;
                    }
                }

                //player Parry Enemy
                if (ronin.parryRectangle.Intersects(enemy[i].attackRectangle))
                {
                    if (enemy[i].type == "ashigaru")
                    {
                        ronin.parryRectangle = Rectangle.Empty;
                        enemy[i].sp += 0;
                    }
                    else if (enemy[i].type == "enbu" && enemy[i].charge == 6)
                    {
                        if (ronin.sp + 45 >= ronin.msp)
                        {
                            ronin.sp = ronin.msp;
                        }
                        else
                            ronin.sp += 45;
                        ronin.hp -= 40;
                        ronin.state = "hurt";
                    }
                    else
                    {
                        ronin.parryRectangle = Rectangle.Empty;
                        if (enemy[i].sp + 35 >= enemy[i].msp)
                        {
                            enemy[i].sp = enemy[i].msp;
                        }
                        else
                            enemy[i].sp += 35;
                    }

                    soundEffects[0].Play(volume: sfxVolume, pitch: 0.0f, pan: 0.0f);
                    Console.WriteLine("player is parry! Enemy SP:" + enemy[i].sp.ToString());
                }
            }
        }

        private void EnemySpawn()
        {
            Texture2D enemyTexture;
            int direction;
            int samurai = 0;
            int ashigaru = 0;
            int boss = 0;
            enemyLeft = 0;

            switch (wave%10)
            {
                case 1:
                    enemyNumber = 1;
                    samuraiNum = 1;
                    ashigaruNum = 0;
                    bossNum = 0;
                    break;
                case 2:
                    enemyNumber = 2;
                    samuraiNum = 2;
                    ashigaruNum = 0;
                    bossNum = 0;
                    break;
                case 3:
                    enemyNumber = 3;
                    samuraiNum = 2;
                    ashigaruNum = 1;
                    bossNum = 0;
                    break;
                case 4:
                    enemyNumber = 4;
                    samuraiNum = 3;
                    ashigaruNum = 1;
                    bossNum = 0;
                    break;
                case 5:
                    enemyNumber = 1;
                    samuraiNum = 0;
                    ashigaruNum = 0;
                    bossNum = 1;
                    break;
                case 6:
                    enemyNumber = 4;
                    samuraiNum = 2;
                    ashigaruNum = 2;
                    bossNum = 0;
                    break;
                case 7:
                    enemyNumber = 5;
                    samuraiNum = 3;
                    ashigaruNum = 2;
                    bossNum = 0;
                    break;
                case 8:
                    enemyNumber = 4;
                    samuraiNum = 4;
                    ashigaruNum = 0;
                    bossNum = 0;
                    break;
                case 9:
                    enemyNumber = 4;
                    samuraiNum = 0;
                    ashigaruNum = 4;
                    bossNum = 0;
                    break;
                case 0:
                    enemyNumber = 1;
                    samuraiNum = 0;
                    ashigaruNum = 0;
                    bossNum = 1;
                    break;
            }

            for (int i = 0; i < enemyNumber; i++)
            {
                enemyTexture = samuraiTexture;
                enemyLeft++;

                direction = random.Next(2);

                if (direction == 1)
                {
                    enemy.Add(new Enemy(enemyTexture, false, new Vector2(random.Next(-34, -4), 75), new Vector2(1f, 1)));
                }
                else
                    enemy.Add(new Enemy(enemyTexture, false, new Vector2(random.Next(500, 530), 75), new Vector2(1f, 1)));

                if(i > 0)
                {
                    for(int j = 0; j <i; j++)
                    {
                        if(enemy[i].position.X - enemy[j].position.X <= 43)
                        {
                            if(direction == 1)
                            {
                                enemy[i].position.X -= 43;
                            }
                            else
                                enemy[i].position.X += 43;
                        }
                    }
                }

                //Set Enemy Type
                if (samurai < samuraiNum)
                {
                    enemy[i].texture = samuraiTexture;
                    enemy[i].SetEnemyType("samurai");
                    enemy[i].barTexture = barTexture;
                    enemy[i].attackSound = soundEffects[2];
                    samurai++;
                }
                else if (ashigaru < ashigaruNum)
                {
                    enemy[i].texture = ashigaruTexture;
                    enemy[i].SetEnemyType("ashigaru");
                    enemy[i].barTexture = barTexture;
                    enemy[i].attackSound = soundEffects[3];
                    ashigaru++;
                }
                else if (boss < bossNum)
                {
                    //random.Next(1, 4)
                    switch (random.Next(1, 4))
                    {
                        case 1:
                            enemy[i].texture = enbuTexture;
                            enemy[i].SetEnemyType("enbu");
                            enemy[i].barTexture = barTexture;
                            enemy[i].bossBarTexture = bossBarTexture;
                            enemy[i].attackSound = soundEffects[7];
                            break;
                        case 2:
                            enemy[i].texture = matsuTexture;
                            enemy[i].SetEnemyType("matsu");
                            enemy[i].barTexture = barTexture;
                            enemy[i].bossBarTexture = bossBarTexture;
                            enemy[i].attackSound = soundEffects[2];
                            break;
                        case 3:
                            enemy[i].texture = rolandTexture;
                            enemy[i].SetEnemyType("roland");
                            enemy[i].barTexture = barTexture;
                            enemy[i].bossBarTexture = bossBarTexture;
                            enemy[i].attackSound = soundEffects[7];
                            break;
                    }
                    boss++;
                }

                //Volume set
                enemy[i].volume = sfxVolume;

                //Difficulty Mod
                enemy[i].mhp += (int)Math.Ceiling((double)0.1 * enemy[i].mhp)*difficultyMod;
                enemy[i].hp = enemy[i].mhp;
            }
        }

        public void PlayBGM()
        {
            //Setting
            sfxVolume = (float)game.mOptionScene.sfxVolume;
            bgmVolume = (float)game.mOptionScene.bgmVolume;
            ronin.volume = sfxVolume;

            //Playing BGM
            MediaPlayer.Volume = bgmVolume;
            MediaPlayer.Play(backgroundMusics[0]);
            MediaPlayer.IsRepeating = true;

            //Set sound effect
            ronin.volume = sfxVolume;
            for (int i = 0; i < enemyNumber; i++)
            {
                enemy[i].volume = sfxVolume;
            }

        }

        public void ResumeBGM()
        {
            //Setting
            sfxVolume = (float)game.mOptionScene.sfxVolume;
            bgmVolume = (float)game.mOptionScene.bgmVolume;

            //Resuming
            MediaPlayer.Volume = bgmVolume;
            MediaPlayer.Resume();

            //Set sound effect
            ronin.volume = sfxVolume;
            for (int i = 0; i < enemyNumber; i++)
            {
                enemy[i].volume = sfxVolume;
            }

        }

    }
}
