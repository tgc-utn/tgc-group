using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Jugador
    {
        public TgcMesh Mesh { get; }
        public TGCVector3 Position { get; set; }
        public TGCVector3 Rotation { get; set; }

        
        public Jugador(TgcMesh mesh, TGCVector3 position, TGCVector3 rotation)
        {
            Mesh = mesh;
            Position = position;
            Rotation = rotation;
        }

        public void Render()
        {
            Mesh.Position = Position;
            Mesh.Rotation = Rotation;
            Mesh.UpdateMeshTransform();
            Mesh.Render();
            Mesh.BoundingBox.Render();
        }
    }
}
