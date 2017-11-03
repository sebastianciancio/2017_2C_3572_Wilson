using Microsoft.DirectX;
using System.Collections.Generic;
using TGC.Core.Input;
using TGC.Group.Model.Objetos;
using TGC.Core.BoundingVolumes;
using System.Drawing;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Utils;

namespace TGC.Group.Model.Character
{
    public class Personaje
    {
        private GameModel env;

        public List<ObjetoInventario> Inventario { get; set; }

        public float MaxHambre { get; set; } = 100;
        public float MaxSed { get; set; } = 100;
        public float MaxCansancio { get; set; } = 100;

        public float Hambre { get; set; } = 100;
        public float Sed { get; set; } = 100;
        public float Cansancio { get; set; } = 100;
        public bool Muerto = false;

        private float tTranscurridoHambre = 0;
        private float tTranscurridoSed = 0;
        private float tTranscurridoCansancio = 0;

        public Vector3 Posicion;
        public TgcBoundingSphere BoundingSphere;

        private TgcSceneLoader loader = new TgcSceneLoader();
        public TgcMesh hachaPersonaje;

        public Personaje(GameModel env)
        {
            this.env = env;
        }

        public void Init()
        {
            this.Posicion = new Vector3(0f, 0f, 0f);
            Inventario = new List<ObjetoInventario>();

            var fruta = new ObjetoInventario {
                objeto = new Fruta(),
                cantidad = 0
            };

            Inventario.Add(fruta);

            var agua = new ObjetoInventario {
                objeto = new Agua(),
                cantidad = 0
            };

            Inventario.Add(agua);

            var madera = new ObjetoInventario {
                objeto = new Madera(),
                cantidad = 0
            };

            Inventario.Add(madera);

            var piedra = new ObjetoInventario {
                objeto = new Piedra(),
                cantidad = 0
            };

            Inventario.Add(piedra);

            // Creo la Espera que envuelve al personaje para detectar colisiones
            BoundingSphere = new TgcBoundingSphere(new Vector3(this.Posicion.X * env.terreno.SceneScaleXZ, env.terreno.CalcularAlturaTerreno(this.Posicion.X, this.Posicion.Z) * env.terreno.SceneScaleY + 10, this.Posicion.Z * env.terreno.SceneScaleXZ), 0.1f);
            BoundingSphere.setRenderColor(Color.Yellow);

            // Creo el Hacha del Personaje
            hachaPersonaje = loader.loadSceneFromFile(env.MediaDir + "hacha\\Hacha-TgcScene.xml").Meshes[0];
            hachaPersonaje.Position = new Vector3(12 * env.terreno.SceneScaleXZ, env.terreno.CalcularAlturaTerreno(12, 22) * env.terreno.SceneScaleY, 22 * env.terreno.SceneScaleXZ);
            hachaPersonaje.AlphaBlendEnable = true;
            hachaPersonaje.Scale = new Vector3(0.05f, 0.05f, 0.05f);
            hachaPersonaje.rotateY(FastMath.PI_HALF);
        }

        public void comer(int cantidad)
        {
            if (Inventario[0].cantidad > 0) {

                if(this.Hambre < 84)
                    this.Hambre = Hambre + 15;

                Inventario[0].cantidad--;
            }
        }

        public void beber(int cantidad)
        {
            if (Inventario[1].cantidad > 0) {

                if (this.Sed < 84)
                    this.Sed = Sed + 15;

                Inventario[1].cantidad--;
            }
        }

        public void descansar(int valor)
        {
            this.Cansancio = Cansancio + valor;
        }

        public void deshidratarse(int valor)
        {
            this.Sed = Sed - valor;
        }

        public void pasarHambre(int valor)
        {
            this.Hambre = Hambre - valor;
        }

        public void caminar(int valor)
        {
            this.Cansancio = Cansancio - valor;
        }

        public void actualizarEstado(float elapsedTime)
        {
            if (!this.Muerto)
            {
                tTranscurridoHambre += elapsedTime;
                tTranscurridoSed += elapsedTime;

                var mult = 1;
                if (env.Input.keyDown(Key.LeftShift)) {
                    mult = 8;
                }

                tTranscurridoCansancio += elapsedTime * mult;

                if (tTranscurridoHambre > 2)
                {
                    this.Hambre = Hambre - 2;
                    tTranscurridoHambre = 0;
                }
                if (tTranscurridoSed > 4)
                {
                    this.Sed = this.Sed - 2;
                    tTranscurridoSed = 0;
                }
                if (tTranscurridoCansancio > 12)
                {
                    this.Cansancio = this.Cansancio - 3;
                    tTranscurridoCansancio = 0;
                }

                if (this.Sed < 1 || this.Cansancio < 1 || this.Hambre < 1) this.Muerto = true;
            }
        }

        public void actualizarControles(float elapsedTime)
        {
            if (env.Input.keyPressed(Key.D1)) {
                comer(1);
            }

            if (env.Input.keyPressed(Key.D2)) {
                beber(1);
            }
            if (env.Input.keyPressed(Key.D3)) {
                if (Inventario[2].cantidad > 100) {
                    Inventario[2].cantidad -= 100;

                    // instanciar fogata apagada
                    env.terreno.activarFogata = true;
                }
            }

            // Verifico si la fogata esta cerca
            if (estaCerca(env.terreno.fogata))
            {
                // Si el objeto es una fogata apagada
                if (env.Input.keyPressed(Key.D4))
                {
                    if (Inventario[3].cantidad > 5)
                    {
                        env.fogataEncendido = true;
                        Inventario[3].cantidad--;
                    }
                }

            }

        }

        public void soltarObjeto()
        {
            //if(Inventario[algo].cantidad > 0)
            //env.terreno.SceneMeshes.Add( algo );
            //Inventario[algo].cantidad--;
        }

        public void guardarObjetoInventario(Objeto item)
        {
            if (item is Fruta) 
            {
                Inventario[0].cantidad++;
            }

            if (item is Agua) {
                Inventario[1].cantidad++;
            }

            if (item is Madera) {
                Inventario[2].cantidad++;
            }

            if (item is Piedra) {
                Inventario[3].cantidad++;
            }
        }


        public void sonidoHacha(bool activado)
        {
            if (activado)
            {
                env.musica.selectionSound("Sonido\\talar.mp3");
                env.musica.startSound();
            }
            else
            {
                env.musica.selectionSound("Sonido\\ambiente1.mp3");
                env.musica.startSound();
                env.sonidoHacha = false;
                env.tiempoAcumHacha = 0;
            }
        }


        public void Update(float ElapsedTime, TgcD3dInput Input)
        {
            // Actualizo la posicion del Hacha
            Posicion = env.Camara.Position;
            hachaPersonaje.Position = new Vector3(Posicion.X-1, Posicion.Y-0.8f, Posicion.Z);

            // Actualizo la Posicion de la Esfera de Colisión
            BoundingSphere.setCenter(Posicion);

            // Actualizo las Variables de Estado del Personaje
            actualizarEstado(ElapsedTime);

            actualizarControles(ElapsedTime);

            if (this.Muerto)
            {
                env.musica.selectionSound("Sonido\\game_over.mp3");
                env.musica.startSound();
            }else
            {

                // Creo la fogata
                if (env.terreno.activarFogata && !env.terreno.ubicacionFogataFija)
                {
                    env.terreno.fogata.Position = new Vector3(env.Camara.Position.X - (1 * env.terreno.SceneScaleXZ), env.terreno.CalcularAlturaTerreno((env.Camara.Position.X - (1 * env.terreno.SceneScaleXZ)) / env.terreno.SceneScaleXZ, env.Camara.Position.Z / env.terreno.SceneScaleXZ) * env.terreno.SceneScaleY, env.Camara.Position.Z);
                    env.terreno.ubicacionFogataFija = true;
                }



                // Verifico cuantos objetos estan Cerca
                env.objetosCerca = 0;
                foreach (var objeto in env.terreno.SceneMeshes)
                {
                    if (estaCerca(objeto))
                    {
                        env.objetosCerca++;

                        // Si el objeto es una Fruta
                        if (objeto.Name.StartsWith("Fruta") && env.Input.keyPressed(Key.E))
                        {
                            Inventario[0].cantidad++;

                            // Desactivo el objeto y lo muevo a un lugar lejano ya que no puedo sacarlos de SceneMeshes
                            objeto.Enabled = false;
                            objeto.dispose();
                            objeto.Position = new Vector3(0, 0, 0);
                        }

                        // Si el objeto es una Palmera
                        if (objeto.Name.StartsWith("Palmera") && Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                        {
                            sonidoHacha(true);
                            env.sonidoHacha = true;

                            Inventario[2].cantidad++;

                            // Desactivo el objeto y lo muevo a un lugar lejano ya que no puedo sacarlos de SceneMeshes
                            objeto.Enabled = false;
                            objeto.dispose();
                            objeto.Position = new Vector3(0, 0, 0);
                        }

                        // Si el objeto es un Pino
                        if (objeto.Name.StartsWith("Pino") && Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                        {
                            sonidoHacha(true);
                            env.sonidoHacha = true;

                            Inventario[2].cantidad++;

                            // Desactivo el objeto y lo muevo a un lugar lejano ya que no puedo sacarlos de SceneMeshes
                            objeto.Enabled = false;
                            objeto.dispose();
                            objeto.Position = new Vector3(0, 0, 0);
                        }

                        // Si el objeto es una Piedra
                        if (objeto.Name.StartsWith("Roca") && Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                        {
                            sonidoHacha(true);
                            env.sonidoHacha = true;

                            Inventario[3].cantidad++;

                            // Desactivo el objeto y lo muevo a un lugar lejano ya que no puedo sacarlos de SceneMeshes
                            objeto.Enabled = false;
                            objeto.dispose();
                            objeto.Position = new Vector3(0, 0, 0);
                        }

                    }
                }


            }
        }

        public bool estaCerca(TgcMesh item)
        {
            Vector3 posicionPersonaje = this.Posicion;
            Vector3 posicionObjeto = item.Position;

            float distanciaCuadrada = Vector3.LengthSq(posicionObjeto - posicionPersonaje);

            return distanciaCuadrada < 2500 * 20000;
        }

        public void Render(float ElapsedTime)
        {
            if (this.Muerto)
            {
                env.terreno.desactivarLluvia();
                env.hud.morirPersonaje();
            }
            else
            {
                // Verifico si la fogata esta cerca
                if (estaCerca(env.terreno.fogata))
                {
                    env.DrawText.drawText("Prueba encender la fogata con las piedras... \n", 400, 20, Color.OrangeRed);
                }

                // Esfera para detectar las colisiones
                // BoundingSphere.render();
                //hachaPersonaje.render();
            }
        }

        public void Dispose()
        {
            BoundingSphere.dispose();
        }

    }
}
