using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Objetos;

namespace TGC.Group.Model.Escenario.Objetos
{
    class Arbol : ObjetoEscena, IDestroyable
    {
        public List<Objeto> Items { get; set; }

        public Arbol(GameModel env) : base(env)
        {
            this.Items = new List<Objeto>();
        }

        protected override string getMeshPath()
        {
            return env.MediaDir + "ArbolSelvatico\\ArbolSelvatico-TgcScene.xml";
        }

        public override List<Objeto> Destroy()
        {
            this.status = false;
            return this.Items;
        }
    }
}
