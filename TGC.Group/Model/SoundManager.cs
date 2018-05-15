using TGC.Core.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGC.Group.Model
{
    public class SoundManager
    {
        public TgcStaticSound sonidoCaminar;
        public TgcStaticSound sonidoFondo;

        static string mediaDir;

        public static SoundManager Instance;

        public static SoundManager getInstance()
        {
            if (Instance == null)
            {
                Instance = new SoundManager(mediaDir);
            }
            return Instance;
        }

        public SoundManager(string unMediaDir)
        {


            sonidoCaminar = new TgcStaticSound();
            sonidoFondo = new TgcStaticSound();

            mediaDir = unMediaDir;

            sonidoFondo.loadSound(mediaDir + "Sound\\Background.wav");
            sonidoCaminar.loadSound(mediaDir + "Sound\\FootSteps.wav");

        }

        public void playSonidoCaminar()
        {
            sonidoCaminar.play();
        }

        public void playSonidoFondo()
        {
            sonidoFondo.play();
        }

        public void dispose()
        {
            sonidoFondo.dispose();
            sonidoCaminar.dispose();
        }
    }
}
