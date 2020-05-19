using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    public class Laser : IRenderizable
    {
        private string direccionDeScene;
        private readonly TGCVector3 posicionInicial;
        private readonly TGCVector3 direccion;
        private readonly float velocidad;

        private TGCBox modeloDeLaser;

        public Laser(string direccionDeScene, TGCVector3 posicionInicial,TGCVector3 direccion)
        {
            this.direccionDeScene = direccionDeScene;
            this.posicionInicial = posicionInicial;
            this.direccion = direccion;
            this.velocidad = 100f;
        }

        public void Init()
        {
            modeloDeLaser = TGCBox.fromSize(new TGCVector3(0.5f, 5, 0.5f), Color.Red);
            modeloDeLaser.Position = posicionInicial;
        }

        public void Update(float elapsedTime)
        {
            TGCVector3 direccionDisparo = direccion;
            direccionDisparo.Normalize();
            TGCVector3 movement = direccionDisparo * velocidad * elapsedTime;
            modeloDeLaser.Position += movement;
            modeloDeLaser.Transform = TGCMatrix.Translation(modeloDeLaser.Position);
        }

        public void Render()
        {
            modeloDeLaser.Render();
            modeloDeLaser.BoundingBox.Render();

        }
        public void Dispose()
        {
            modeloDeLaser.Dispose();
        }

        private TGCQuaternion QuaternionDireccion(TGCVector3 direccionDisparoNormalizado)
        {
            TGCVector3 DireccionA = new TGCVector3(0, 0, -1);
            TGCVector3 cross = TGCVector3.Cross(DireccionA, direccionDisparoNormalizado);
            TGCQuaternion newRotation = TGCQuaternion.RotationAxis(cross, FastMath.Acos(TGCVector3.Dot(DireccionA, direccionDisparoNormalizado)));
            return newRotation;
        }

        public TGCBox GetModeloLaser()
        {
            return modeloDeLaser;
        }
    }
}
