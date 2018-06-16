using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Collision;
using TGC.Group.SphereCollisionUtils;


namespace TGC.Group.Modelo
{

    public class PisoInercia
    {
        private TGCVector3 vectorEntrante = new TGCVector3(0,0,0);
        public float aceleracionFrenada = 0.999f;

        public TgcMesh pisoMesh { get; }
        private TgcBoundingAxisAlignBox SlidingBox { get; set; }

        private bool seMovioBB = false;

        public PisoInercia(TgcMesh mesh)
        {
            this.pisoMesh = mesh;

        }

        public TGCVector3 vectorEntrada()
        {
            vectorEntrante *= aceleracionFrenada;
            return vectorEntrante;
        }

        public void setVectorEntrante(TGCVector3 nuevoVectorEntrante)
        {
            if (TGCVector3.Length(this.vectorEntrante) == 0)
            {
                this.vectorEntrante = new TGCVector3(nuevoVectorEntrante.X, 0f, nuevoVectorEntrante.Z);
            }
            else
            {
                TGCVector3 versor = nuevoVectorEntrante * (1 / TGCVector3.Length(nuevoVectorEntrante));
                this.vectorEntrante = versor * TGCVector3.Length(this.vectorEntrante);
            }
        }


        public TGCVector3 versorEntrada()
        {
            return this.vectorEntrante * (1 / TGCVector3.Length(this.vectorEntrante));
        }

     

       


        public TgcBoundingAxisAlignBox BoundingBox
        {
            get
            {
                return pisoMesh.BoundingBox;
            }
        }
        public TgcBoundingAxisAlignBox SlidingBoundingBox
        {
            get
            {
                if (!seMovioBB)
                {
                    var SlidingMesh = pisoMesh.clone("Clon");

                    SlidingMesh.BoundingBox.move(new TGCVector3(0, 20, 0));
                    SlidingBox = SlidingMesh.BoundingBox.clone();
                    seMovioBB = true;
                }
                return SlidingBox;
            }
        }
        

      

        #region MeshAdapter
        public TGCVector3 Position
        {
            get
            {
                return pisoMesh.Position;
            }
            set
            {
                pisoMesh.Position = value;
            }
        }
        public bool AutoTransform
        {
            get
            {
                return pisoMesh.AutoTransform;
            }
            set
            {
                pisoMesh.AutoTransform = value;
            }
        }

        public TGCMatrix Transform
        {
            get
            {
                return pisoMesh.Transform;
            }
            set
            {
                pisoMesh.Transform = value;
            }
        }


        public void Render() => pisoMesh.Render();
        #endregion



    }
}
