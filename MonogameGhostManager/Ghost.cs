using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Sprite;
using MonoGameLibrary.Util;

namespace MonogameGhostManager
{
    public enum GhostState { Chasing, Evading, Roving, Dead }

    public class Ghost : DrawableSprite
    {
        GhostState state;

        protected PacMan.MonogamePacMan pacMan;

        public Texture2D ghostTexture;


        public GhostState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                }
            }
        }

        public Ghost(Game game, GhostManager m, PacMan.MonogamePacMan pacMan)
            : base(game)
        {
            this.state = GhostState.Roving;
        }


            public void Evade()
        {
            this.state = GhostState.Evading;
        }
    }
}
