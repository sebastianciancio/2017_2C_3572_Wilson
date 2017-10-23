using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Objetos;

namespace TGC.Group.Model.Character {
    public class ObjetoInventario {
        public Objeto objeto { get; set; }
        public int cantidad { get; set; }

        public ObjetoInventario(Objeto objeto = null, int cantidad = 0) {
            this.objeto = objeto;
            this.cantidad = cantidad;
        }
    }
}
