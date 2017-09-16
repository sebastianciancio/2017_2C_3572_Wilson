using Microsoft.DirectX;
using System.Collections.Generic;
using TGC.Core.Input;
using TGC.Group.Model.Objetos;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model.Character
{
    public class Personaje
    {
        private GameModel env;

        private List<Objeto> Inventario { get; set; }

        public float MaxHambre { get; set; } = 100;
        public float MaxSed { get; set; } = 100;
        public float MaxCansancio { get; set; } = 100;

        public float Hambre { get; set; } = 100;
        public float Sed { get; set; } = 100;
        public float Cansancio { get; set; } = 100;
        public Vector3 Posicion;
        public TgcBoundingSphere BoundingSphere;

        public Personaje(GameModel env)
        {
            this.env = env;
        }

        public void Init()
        {
            this.Posicion = new Vector3(0f,0f,0f);
            Inventario = new List<Objeto>();

            // Creo la Espera que envuelve al personaje para detectar colisiones
            BoundingSphere = new TgcBoundingSphere(new Vector3(0 * env.terreno.SceneScaleXZ, env.terreno.CalcularAlturaTerreno(0, -90) * env.terreno.SceneScaleY + 20 * env.terreno.SceneScaleXZ, -90 * env.terreno.SceneScaleXZ), 0.2f * env.terreno.SceneScaleXZ);
        }

        public void Update(float ElapsedTime, TgcD3dInput Input)
        {
            Posicion = env.Camara.Position;

            // Actualizo la Posicion de la Esfera de Colisión
            BoundingSphere.setCenter(Posicion);
        }

        public void Render(float ElapsedTime)
        {
            // Esfera para detectar las colisiones
            BoundingSphere.render();
        }

        public void Dispose()
        {
            BoundingSphere.dispose();
        }
    }
}
