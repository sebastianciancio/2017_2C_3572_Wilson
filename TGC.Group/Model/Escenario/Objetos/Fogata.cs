using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Objetos;

namespace TGC.Group.Model.Escenario.Objetos
{
    class Fogata: ObjetoEscena, IDestroyable
    {
        public List<Objeto> Items { get; set; }

        public Fogata(GameModel env) : base(env)
        {
            this.Items = new List<Objeto>();
        }

        protected override string getMeshPath()
        {
            return env.MediaDir + "Roca\\Roca-TgcScene.xml";
        }

        public override List<Objeto> Destroy()
        {
            this.status = false;
            return this.Items;
        }
    }
}
