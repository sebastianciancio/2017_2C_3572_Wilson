using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Objetos;

namespace TGC.Group.Model.Escenario.Objetos
{
    public interface IDestroyable
    {
        List<Objeto> Items { get; set; }

        List<Objeto> Destroy();
    }
}
