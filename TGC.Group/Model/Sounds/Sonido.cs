using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TGC.Core.Example;
using TGC.Core.Sound;
using Microsoft.DirectX;
using TGC.Core.Text;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;

namespace TGC.Group.Model.Sounds
{
    /// <summary>
    ///     Ejemplo PlayMp3:
    ///     Unidades Involucradas:
    ///     # Unidad 3 - Conceptos Basicos de 3D - GameEngine
    ///     Muestra como reproducir un archivo de sonido en formato MP3.
    ///     Autor: Matias Leone, Leandro Barbagallo
    /// </summary>
    public class Sonido : TgcExample
    {

        private static Sonido myInstance;
        private TgcDirectSound DirectSound;
        private Boolean reproducir = false;
        private int TiempoRetardo = 2;
        private int contadorDeCiclos = 0;

        private Tgc3dSound theSound;

        public static Sonido getInstance()
        {
            return myInstance;
        }

        public Sonido(string mediaDir, string shadersDir, TgcDirectSound DS) : base(mediaDir, shadersDir)
        {
            myInstance = this;
            this.DirectSound = DS;
            Init();
        }

        public override void Init()
        {
        }

        public void playSound(String fileName, Vector3 position)
        {
            Tgc3dSound sound = new Tgc3dSound(fileName, position, DirectSound.DsDevice);
            sound.MinDistance = 500f;
            theSound = sound;
            contadorDeCiclos = 0;
        }

        public void stopSound()
        {
            if (theSound != null) theSound.stop();
        }

        public void startSound()
        {
            if (theSound != null) theSound.play(false);
        }

        public override void Update()
        {
            //PreUpdate();
            //Ejecutar en loop los sonidos
            contadorDeCiclos++;
            if (contadorDeCiclos > TiempoRetardo)
            {
                contadorDeCiclos = 0;
                theSound.stop();
            }
            else
            {
                theSound.play();
            }
        }

        public override void Render()
        {
            PreRender();



            PostRender();

        }

        public override void Dispose()
        {
            theSound.dispose();

        }
    }
}
