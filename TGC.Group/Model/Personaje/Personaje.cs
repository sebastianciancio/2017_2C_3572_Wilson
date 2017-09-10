using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.SkeletalAnimation;
using TGC.Group.Model.Objetos;

namespace TGC.Group.Model.Character
{
    class Personaje
    {
        private GameModel env;
        public TgcSkeletalMesh personaje;

        public List<Objeto> Inventario { get; set; };

        public float MaxHambre { get; set; } = 100;
        public float MaxSed { get; set; } = 100;
        public float MaxCansancio { get; set; } = 100;

        public float Hambre { get; set; } = 100;
        public float Sed { get; set; } = 100;
        public float Cansancio { get; set; } = 100;

        public Personaje(GameModel env)
        {
            this.env = env;
        }

        public void Init()
        {
            var skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                env.MediaDir + "SkeletalAnimations\\Robot\\Robot-TgcSkeletalMesh.xml",
                env.MediaDir + "SkeletalAnimations\\Robot\\",
                new[]
                {
                    env.MediaDir + "SkeletalAnimations\\Robot\\Caminando-TgcSkeletalAnim.xml",
                    env.MediaDir + "SkeletalAnimations\\Robot\\Parado-TgcSkeletalAnim.xml"
                });

            personaje.playAnimation("Parado", true);

            var posicionX = 0;
            var posicionZ = -90;
            var posicionY = env.terreno.CalcularAlturaTerreno(posicionX, posicionZ) + 1;

            var alturaOjos = 15f;

            personaje.Position = new Vector3(posicionX * env.terreno.SceneScaleXZ, posicionY * env.terreno.SceneScaleY + alturaOjos, posicionZ * env.terreno.SceneScaleXZ);
            personaje.Scale = new Vector3(10000, 10000, 10000);

            Inventario = new List<Objeto>();
        }

        public void Update(float ElapsedTime, TgcD3dInput Input)
        {
            var velocidadCaminar = 400f;
            var velocidadRotacion = 120f;

            //Calcular proxima posicion de personaje segun Input
            var moveForward = 0f;
            float rotate = 0;
            var moving = false;
            var rotating = false;

            //Adelante
            if (Input.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
            }

            //Atras
            if (Input.keyDown(Key.S))
            {
                moveForward = velocidadCaminar;
                moving = true;
            }

            //Derecha
            if (Input.keyDown(Key.D))
            {
                rotate = velocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (Input.keyDown(Key.A))
            {
                rotate = -velocidadRotacion;
                rotating = true;
            }

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                personaje.rotateY(rotate * ElapsedTime);
                //env.Camara.rotate??;
            }

            //Si hubo desplazamiento
            if (moving)
            {
                personaje.playAnimation("Caminando", true);
                var lastPos = personaje.Position;
                personaje.moveOrientedY(moveForward * ElapsedTime);
                var collide = DetectCollision();

                if (collide)
                {
                    personaje.Position = lastPos;
                }

                //Hacer que la camara siga al personaje en su nueva posicion
                // env.Camara.SetCamera(personaje.Position, env.Camara.LookAt);
            }
            else
            {
                personaje.playAnimation("Parado", true);
            }

        }

        public void Render(float ElapsedTime)
        {
            personaje.animateAndRender(ElapsedTime);
        }

        public void Dispose()
        {
            personaje.dispose();
        }

        private bool DetectCollision()
        {
            foreach (var obstaculo in new List<TgcSkeletalMesh>())
            {
                var result = TgcCollisionUtils.classifyBoxBox(personaje.BoundingBox, obstaculo.BoundingBox);
                var colliding = (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando);
                if (colliding)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
