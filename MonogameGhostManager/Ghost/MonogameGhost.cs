using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Sprite;
using MonoGameLibrary.Util;
using PacMan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MonogameGhost.Ghost.Ghost;

namespace MonogameGhost.Ghost
{
    class MonogameGhost : DrawableSprite
    {
        protected GameConsoleGhost ghost;
        protected GhostManager gm;
        public GameConsoleGhost Ghost
        {
               get { return this.ghost; }
               protected set { this.ghost = value; }
        }

        protected PacMan.MonogamePacMan pacMan;

        public Texture2D ghostHit;
        public Texture2D ghostTexture;
        public string strGhostTexture;

        Random r;

        public MonogameGhost(Game game, GhostManager m, PacMan.MonogamePacMan pacMan)
            : base(game)
        {
            this.pacMan = pacMan;
            this.gm = m;
            this.ghost = new GameConsoleGhost((GameConsole)game.Services.GetService<IGameConsole>());
            
            strGhostTexture = "RedGhost";
            this.ghost.State = GhostState.Roving;
            
            r = new Random();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.ShowMarkers = false;
            this.Origin = new Vector2(this.spriteTexture.Width / 2, this.spriteTexture.Height / 2);
        }

        protected override void LoadContent()
        {

            this.ghostTexture = this.Game.Content.Load<Texture2D>(strGhostTexture);
            this.ghostHit = this.Game.Content.Load<Texture2D>("GhostHit");
            this.spriteTexture = ghostTexture;
            this.Direction = new Vector2(0, 1);

            this.Speed = 100.0f;

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float turnAmount = .04f;

            switch (this.ghost.State)
            {
                case GhostState.Dead:

                    //TODO Dead moving and dead animation.
                    //Until then
                    this.ghost.State = GhostState.Roving;
                    //Pick random direction
                    this.Direction = this.GetRandomDirection(); ;                 //Assign random direction


                    break;
                case GhostState.Chasing:
                    //Change texture if Chasing
                    if (!(this.spriteTexture == this.ghostTexture))
                    {
                        this.ghost.Log(this.ToString() + " Chasing changed texture to spriteTexture");
                        this.spriteTexture = this.ghostTexture;
                    }
                    if (pacMan.Location.Y > this.Location.Y)
                    {
                        //this.Direction.Y = 1;
                        this.Direction.Y = MathHelper.Clamp(this.Direction.Y += turnAmount, -1, 1);
                    }
                    else
                    {
                        //this.Direction.Y = -1;
                        this.Direction.Y = MathHelper.Clamp(this.Direction.Y -= turnAmount, -1, 1);
                    }
                    if (pacMan.Location.X > this.Location.X)
                    {
                        //this.Direction.X = 1;
                        this.Direction.X = MathHelper.Clamp(this.Direction.X += turnAmount, -1, 1);
                    }
                    else
                    {
                        //this.Direction.X = -1;
                        this.Direction.X = MathHelper.Clamp(this.Direction.X -= turnAmount, -1, 1);
                    }
                    break;
                case GhostState.Evading:
                    //Change texture if evading
                    if (this.spriteTexture != this.ghostHit)
                    {
                        this.ghost.Log(this.ToString() + " Evading changed texture to ghostHit");
                        this.spriteTexture = this.ghostHit;
                    }

                    if (pacMan.Location.Y > this.Location.Y)
                    {
                        this.Direction.Y = -1;
                    }
                    else
                    {
                        this.Direction.Y = 1;
                    }
                    if (pacMan.Location.X > this.Location.X)
                    {
                        this.Direction.X = -1;
                    }
                    else
                    {
                        this.Direction.X = -1;
                    }
                    break;
                case GhostState.Roving:
                    //Change texture if Chasing
                    if (!(this.spriteTexture == this.ghostTexture))
                    {
                        this.ghost.Log(this.ToString() + " Roving changed texture to spriteTexture");
                        this.spriteTexture = this.ghostTexture;
                    }
                    //check if ghost can see pacman
                    Vector2 normD = Vector2.Normalize(this.Direction);
                    Vector2 p = new Vector2(this.Location.X, this.Location.Y);
                    while (p.X < this.Game.GraphicsDevice.Viewport.Width &&
                          p.X > 0 &&
                          p.Y < this.Game.GraphicsDevice.Viewport.Height &&
                          p.Y > 0)
                    {
                        if (pacMan.LocationRect.Contains(new Point((int)p.X, (int)p.Y)))
                        {
                            this.ghost.State = GhostState.Chasing;
                            this.ghost.Log(this.ToString() + " saw pacman");
                            break;
                        }
                        p += this.Direction;
                    }

                    break;
            }

            Location += ((this.Direction * (lastUpdateTime / 1000)) * Speed);      //Simple Move

            //Borders Keep Ghost on the Screen
            if ((this.Location.Y + this.spriteTexture.Height / 2 > this.Game.GraphicsDevice.Viewport.Height)
                ||
                (this.Location.Y - this.spriteTexture.Height / 2 < 0)
                )
            {
                this.Direction.Y *= -1;
                this.ghost.State = GhostState.Roving;
            }
            if ((this.Location.X + this.spriteTexture.Width / 2 > this.Game.GraphicsDevice.Viewport.Width)
                ||
                (this.Location.X - this.spriteTexture.Width / 2 < 0)
                )
            {
                this.Direction.X *= -1;
                this.ghost.State = GhostState.Roving;
            }

            //Collision
            if (this.Intersects(pacMan))
            {
                this.ghost.Log(this.ToString() + " Intersects PacsMan" + gameTime.TotalGameTime.Seconds + "." + gameTime.TotalGameTime.Milliseconds);
                //PerPixel Collision
                if (this.PerPixelCollision(pacMan))
                {
                    this.ghost.Log(this.ToString() + " Pixels touched PacsMan" + gameTime.TotalGameTime.Seconds + "." + gameTime.TotalGameTime.Milliseconds);
                    this.ghost.State = GhostState.Dead;
                    gm.AddGhost();
                }
            }

            //Ghost on Ghost

            base.Update(gameTime);
        }


        //Possible move to Sprite
        public Vector2 GetRandomDirection()
        {
            Vector2 v = new Vector2((float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f);
            Vector2.Normalize(ref v, out v);    //Normalize
            return v;
        }

        public Vector2 GetRandLocation()
        {
            //System.Threading.Thread.Sleep(1);
            Vector2 loc;
            loc.X = r.Next(Game.Window.ClientBounds.Width - this.spriteTexture.Width) + this.spriteTexture.Width;
            loc.Y = r.Next(Game.Window.ClientBounds.Height - this.spriteTexture.Height) + this.spriteTexture.Height;

            //FIXES HALFWAY CLIPPING ON SPAWN
            if(loc.X > 755 || loc.X < 55)
            {
                GetRandLocation();
            }
            if(loc.Y > 455 || loc.Y < 55)
            {
                GetRandLocation();
            }
            return loc;
        }


    }
}
