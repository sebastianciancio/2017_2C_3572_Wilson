using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TGC.Core.Sound;
using TGC.Core.Text;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo PlayMp3:
    ///     Unidades Involucradas:
    ///     # Unidad 3 - Conceptos Basicos de 3D - GameEngine
    ///     Muestra como reproducir un archivo de sonido en formato MP3.
    ///     Autor: Matias Leone, Leandro Barbagallo
    /// </summary>
    public class Musica
    {
        private int nroFile;
        private String currentFile;
        private String currentPlaying;
        private TgcMp3Player mp3Player;
        private List<String> sonidos;
        private string MediaDir;

        public Musica(string mediaDir)
        {
            MediaDir = mediaDir;
            sonidos = new System.Collections.Generic.List<String>();
            sonidos.Add(MediaDir + "MySounds\\sound3.mp3");
            sonidos.Add(MediaDir + "MySounds\\sound4.mp3");
            sonidos.Add(MediaDir + "MySounds\\sound2.mp3");
            sonidos.Add(MediaDir + "MySounds\\sound1.mp3");
            nroFile = 0;
            currentFile = null;

            mp3Player = new TgcMp3Player();
        }

        /// <summary>
        ///     Cargar un nuevo MP3 si hubo una variacion
        /// </summary>
        private void loadMp3(string filePath)
        {
            if (currentFile == null || currentFile != filePath)
            {
                currentFile = filePath;

                //Cargar archivo
                //mp3Player.closeFile();
                mp3Player.FileName = currentFile;

                //currentMusicText.Text = "Playing: " + new FileInfo(currentFile).Name;
            }
        }

        public void nextSound()
        {
            nroFile++;
            if (nroFile > 3) nroFile = 0;
            loadMp3(sonidos.ToArray()[nroFile]);
        }

        public void menuSound()
        {
            loadMp3(MediaDir + "MySounds\\intro.mp3");
        }

        public void selectionSound()
        {
            loadMp3(MediaDir + "MySounds\\intro.mp3");
        }

        public void startSound()
        {
            var currentState = mp3Player.getStatus();
            if (currentState == TgcMp3Player.States.Playing)
            {

                if (currentFile != currentPlaying)
                {
                    //Parar y reproducir MP3
                    mp3Player.closeFile();
                    mp3Player.play(true);
                    currentPlaying = mp3Player.FileName;
                }
            } else
            {
                mp3Player.play(true);
                currentPlaying = mp3Player.FileName;
            }
           
        }

        public void soundControl()
        {
            //Contro del reproductor
            var currentState = mp3Player.getStatus();
            if (currentState == TgcMp3Player.States.Open)
            {
                //Reproducir MP3
                mp3Player.play(true);
                currentPlaying = mp3Player.FileName;
            }
            if (currentState == TgcMp3Player.States.Stopped)
            {
                //Parar y reproducir MP3
                mp3Player.closeFile();
                mp3Player.play(true);
                currentPlaying = mp3Player.FileName;
            }
            if (currentState == TgcMp3Player.States.Paused)
            {
                //Resumir la ejecución del MP3
                mp3Player.resume();
            }
            if (currentState == TgcMp3Player.States.Playing)
            {

                if (currentFile != currentPlaying )
                {
                    //Parar y reproducir MP3
                    mp3Player.closeFile();
                    mp3Player.play(true);
                    currentPlaying = mp3Player.FileName;
                }
            }

        }

        public void pausa()
        {
            //Contro del reproductor
            var currentState = mp3Player.getStatus();
            if (currentState == TgcMp3Player.States.Playing)
            {
                //Pausar el MP3
                mp3Player.pause();
            }
        }

        public void startGame()
        {
            //Sonido.getInstance().playSound(MediaDir + "MySounds\\Special4.wav", new Vector3(2000, 100, 2000));
            loadMp3(MediaDir + "MySounds\\selection.mp3");
            startSound();
            loadMp3(sonidos.ToArray()[nroFile]);

            //Render texto
            //currentMusicText.render();
            //instruccionesText.render();

        }

        public void Dispose()
        {
            mp3Player.closeFile();
        }
    }
}
