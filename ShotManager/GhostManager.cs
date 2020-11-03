using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonogameGhost.Ghost;

namespace ShotManager
{
    class GhostManager : Microsoft.Xna.Framework.GameComponent
    {
        public List<Ghost> ghostList;
        public GhostManager(Game game) : base(game)
        {
            ghostList = new List<Ghost>();
        }



    }
}
