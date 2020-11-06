using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameGhost.Ghost
{
    class GhostManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<MonogameGhost> Ghosts;
        Random r;

        InputHandler input;
        GameConsole console;

        PacMan.MonogamePacMan pac;

        public GhostManager(Game game, PacMan.MonogamePacMan pac)
           : base(game)
        {
            this.pac = pac;
            Ghosts = new List<MonogameGhost>();
            r = new Random(System.DateTime.Now.Millisecond);

            input = (InputHandler)game.Services.GetService(typeof(IInputHandler));
            console = (GameConsole)game.Services.GetService(typeof(IGameConsole));
        }

        public override void Update(GameTime gameTime)
        {
            
            if (input.WasKeyPressed(Keys.C))
            {
                this.ChangeState(Ghost.GhostState.Chasing);
                console.GameConsoleWrite("Ghosts state changed to Chasing");
            }
            if (input.WasKeyPressed(Keys.R))
            {
                this.ChangeState(Ghost.GhostState.Chasing);
                console.GameConsoleWrite("Ghosts state changed to Roving");
            }
            if (input.WasKeyPressed(Keys.E))
            {
                this.ChangeState(Ghost.GhostState.Chasing);
                console.GameConsoleWrite("Ghosts state changed to Evading");
            }
            if (input.WasKeyPressed(Keys.D))
            {
                this.ChangeState(Ghost.GhostState.Chasing);
                console.GameConsoleWrite("Ghosts state changed to Dead");
            }

            for (int i = 0; i < Ghosts.Count; i++)
            {
                Ghosts[i].Update(gameTime);
            }

            CheckCollision();
            
            base.Update(gameTime);
        }

        protected void ChangeState(Ghost.GhostState ghostState)
        {
            foreach (MonogameGhost g in Ghosts)
            {
                g.Ghost.State = ghostState;
            }

        }

        public void AddGhost()
        {
            AddGhost("RedGhost");
        }

        private void AddGhost(string TextureName)
        {
            MonogameGhost g = new MonogameGhost(Game,this, this.pac);
            g.strGhostTexture = TextureName;
            g.Initialize();
            g.Location = g.GetRandLocation();
            g.SetTranformAndRect(); //Ghost location changed and update wasn't called to we need to update the rectagle

            //no overlapping
            foreach (MonogameGhost otherGhost in Ghosts)
            {
                while (g.Intersects(otherGhost) || (g.Intersects(pac)))
                {
                    g.Location = g.GetRandLocation();
                    g.SetTranformAndRect(); //Ghost location changed and update wasn't called to we need to update the rectagle
                }
            }
            g.Scale = 1.0f;
            g.Enabled = true;
            g.Visible = true;
            Ghosts.Add(g);
            console.GameConsoleWrite(string.Format("Added {0} Ghost Number {1}", TextureName, this.Ghosts.Count));
        }

        //Add 4 ghosts
        protected override void LoadContent()
        {
            LoadThreeGhosts();
            AddGhost();
            base.LoadContent();
        }

        private void LoadThreeGhosts()
        {
            AddGhost("RedGhost");
            AddGhost("TealGhost");
            AddGhost("PurpleGhost");

        }

        //Check for collision and send in random direction if true
        private void CheckCollision()
        {
            for(int i = 0; i < Ghosts.Count-1; i++)
            {
                if(Ghosts[i].Intersects(Ghosts[i+1]))
                {
                    SendInRandomDirection(i);
                }
            }
        }

        private void SendInRandomDirection(int i)
        {
            Ghosts[i].Direction = Ghosts[i].GetRandomDirection();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (MonogameGhost g in Ghosts)
            {
                g.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }

}

