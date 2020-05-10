using System.Collections.Generic;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Nave // es un singleton!
    {
        private TgcScene escenaNave;
        //private TGCMatrix escalaBase;

        private static Nave _instance;

        protected Nave()
        {
        }

        public static Nave Instance()
        {
            if (_instance == null)
            {
                _instance = new Nave();
            }

            return _instance;
        }

        public void Init(TgcScene _escenaNave)
        {
            escenaNave = _escenaNave;
            //escalaBase = TGCMatrix.Scaling(new TGCVector3(0.2f, 0.2f, 0.2f));
        }
        public void Update()
        {
        }
        public void Render()
        {
            foreach (var mesh in escenaNave.Meshes)
            {
                if (mesh == null)
                {
                    break;
                }
                mesh.Render();
                //mesh.BoundingBox.Render();
            }
        }
        
        public void Interact()
        {
            /*foreach (var mesh in escenaNave.Meshes)
            {
                if (mesh == null)
                {
                    break;
                }
                mesh.Dispose();
            }*/
        }

        public List<TgcMesh> obtenerMeshes()
        {
            return escenaNave.Meshes;
        }

        public void Dispose()
        {
            escenaNave.DisposeAll();
        }
    }
}
