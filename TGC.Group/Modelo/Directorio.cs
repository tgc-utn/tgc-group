using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Collision;


namespace TGC.Group.Modelo
{
    public class Directorio
    {

        public string EscenaCrash {get;set;}
        

        public string RobotDirectorio { get; set; }
        public string RobotSkeletalMesh { get; set; }
        public string RobotTextura { get; set; }

        public string RobotCaminando { get; set; }
        public string RobotParado { get; set; }
        public string RobotPateando { get; set; }
        public string RobotSaltando { get; set; }
        public string RobotEmpujando { get; set; }
        public string RobotCorriendo { get; set; }
        public string RobotPegando { get; set; }

        public string SonidoFondo { get; set; }
        public string SonidoCaminar { get; set; }
        public string SonidoSalto { get; set; }
        public string SonidoFruta { get; set; }
        public string SonidoMoneda { get; set; }

        public string Menu { get; set; }
        
        public string BarraVida { get; set; }
        public string Fruta { get; set; }
        public string Mascara { get; set; }

        public Directorio(string mediaDir)
        {
            EscenaCrash = mediaDir + "Escenas\\CrashBandicoot-TgcScene.xml";

            RobotDirectorio = mediaDir + "Personaje\\Robot\\";
            RobotSkeletalMesh = RobotDirectorio + "Robot-TgcSkeletalMesh.xml";
            RobotTextura = RobotDirectorio + "Textures\\uvwGreen.jpg";

            RobotCaminando = RobotDirectorio + "Caminando-TgcSkeletalAnim.xml";
            RobotPateando = RobotDirectorio + "Pateando-TgcSkeletalAnim.xml";
            RobotParado = RobotDirectorio + "Parado-TgcSkeletalAnim.xml";
            RobotCorriendo = RobotDirectorio + "Corriendo-TgcSkeletalAnim.xml";
            RobotEmpujando = RobotDirectorio + "Empujando-TgcSkeletalAnim.xml";

            SonidoFondo = mediaDir + "Sonidos\\background.mp3";
            SonidoCaminar = mediaDir + "Sonidos\\FootSteps.wav";
            SonidoSalto = mediaDir + "Sonidos\\jump.wav";
            SonidoMoneda = mediaDir + "Sonidos\\coin.wav";
            SonidoFruta = mediaDir + "Sonidos\\fruit.mp3";

            Menu = mediaDir + "Menu\\";

            BarraVida = mediaDir + "Imagenes\\barra_vida.png";
            Fruta = mediaDir + "Imagenes\\fruta.png";
            Mascara = mediaDir + "Imagenes\\mascara.png";
        }

    }
}
