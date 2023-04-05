using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Ronin
{
    public class Scene
    {
        protected EventHandler SceneEvent;
        public Scene(EventHandler SceneEvent) 
        { 

        }
        public virtual void Update(GameTime gameTime) 
        { 

        }
        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            
        }
    }
}
