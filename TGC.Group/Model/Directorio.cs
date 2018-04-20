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


namespace TGC.Group.Model
{
    class Directorio
    {

        public string EscenaSelva {get;set;}
        public string EscenaArena { get; set; }
        public string EscenaMontania { get; set; }

        public string RobotDirectorio { get; set; }
        public string RobotSkeletalMesh { get; set; }
        public string RobotCaminando { get; set; }
        public string RobotParado { get; set; }
        public string RobotSaltando { get; set; }
        public string RobotTextura { get; set; }

        public Directorio(string mediaDir)
        {

            
            EscenaSelva = mediaDir + "MeshCreator\\Scenes\\Selva\\Selva-TgcScene.xml" ;
            EscenaArena = mediaDir + "MeshCreator\\Scenes\\Arena\\Arena-TgcScene.xml";
            EscenaMontania = mediaDir + "MeshCreator\\Scenes\\Mountains\\Mountains-TgcScene.xml";

            RobotDirectorio = mediaDir + "SkeletalAnimations\\Robot\\";
            RobotSkeletalMesh = RobotDirectorio + "Robot-TgcSkeletalMesh.xml";
            RobotCaminando = RobotDirectorio + "Caminando-TgcSkeletalAnim.xml";
            RobotParado = RobotDirectorio + "Parado-TgcSkeletalAnim.xml";
            RobotTextura = RobotDirectorio + "Textures\\uvwGreen.jpg";
            
        }

    }
}
