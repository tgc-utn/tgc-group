using System.Collections.Generic;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Textures;
using TGC.Core.Mathematica;
using TGC.Core.Geometry;

namespace TGC.Group.Model
{
    class InteriorNave // es un singleton
    {
        private static InteriorNave _instance;
        private List<TgcMesh> meshes;

        protected InteriorNave()
        {
        }

        public static InteriorNave Instance()
        {
            if (_instance == null)
            {
                _instance = new InteriorNave();
            }

            return _instance;
        }

        public List<TgcMesh> obtenerMeshes()
        {
            return meshes;
        }

        public void Init(string MediaDir)
        {
            var diffuseMap = TgcTexture.createTexture(MediaDir + "Textures//Lisas.bmp");

            var paredSur = TGCBox.fromExtremes(new TGCVector3(-200, 0, -210), new TGCVector3(200, 100, -200), diffuseMap);
            paredSur.Transform = TGCMatrix.Translation(paredSur.Position);

            var paredOeste = TGCBox.fromExtremes(new TGCVector3(-210, 0, -200), new TGCVector3(-200, 100, 200), diffuseMap);
            paredOeste.Transform = TGCMatrix.Translation(paredOeste.Position);

            var paredEste = TGCBox.fromExtremes(new TGCVector3(200, 0, -200), new TGCVector3(210, 100, 200), diffuseMap);
            paredEste.Transform = TGCMatrix.Translation(paredEste.Position);

            var piso = TGCBox.fromExtremes(new TGCVector3(-200, -1, -200), new TGCVector3(200, 0, 200), diffuseMap);
            piso.Transform = TGCMatrix.Translation(piso.Position);

            //Convertir TgcBox a TgcMesh
            var m1 = paredSur.ToMesh("paredSur");
            var m2 = paredOeste.ToMesh("paredOeste");
            var m3 = paredEste.ToMesh("paredEste");
            var m4 = piso.ToMesh("piso");

            //Convertir TgcMesh a TgcMeshBumpMapping
            meshes = new List<TgcMesh>();
            meshes.Add(m1);
            meshes.Add(m2);
            meshes.Add(m3);
            meshes.Add(m4);

            //Borrar TgcMesh y TgcBox, ya no se usan
            paredSur.Dispose();
            paredOeste.Dispose();
            paredEste.Dispose();
            piso.Dispose();
        }

        public void Update()
        {

        }
        public void Render()
        {
            foreach (var mesh in meshes)
            {
                mesh.Render();
            }
        }
        public void Dispose()
        {
            foreach (var mesh in meshes)
            {
                mesh.Dispose();
            }
        }
    }
}
