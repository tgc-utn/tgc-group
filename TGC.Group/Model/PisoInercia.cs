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
using TGC.Examples.Collision.SphereCollision;


namespace TGC.Group.Model
{

    public class PisoInercia
    {
        private TGCVector3 _vectorEntrada;
        public float AceleracionFrenada { get; set; }
        public TgcMesh RenderMesh { get; }
        private TgcBoundingAxisAlignBox SlidingBox { get; set; }

        private bool seMovioBB = false;
        public TgcBoundingAxisAlignBox BoundingBox
        {
            get
            {
                return RenderMesh.BoundingBox;
            }
        }
        public TgcBoundingAxisAlignBox SlidingBoundingBox
        {
            get
            {
                if (!seMovioBB)
                {
                    var SlidingMesh = RenderMesh.clone("Clon");

                    SlidingMesh.BoundingBox.move(new TGCVector3(0, 20, 0));
                    SlidingBox = SlidingMesh.BoundingBox.clone();
                    seMovioBB = true;
                }
                return SlidingBox;
            }
        }
        public TGCVector3 Position
        {
            get
            {
                return RenderMesh.Position;
            }
            set
            {
                RenderMesh.Position = value;
            }
        }

        public TGCVector3 VectorEntrada
        {
            get
            {
                _vectorEntrada = _vectorEntrada * AceleracionFrenada;
                //if (Math.Abs(_vectorEntrada.X )< 0.00000001 || Math.Abs(_vectorEntrada.Z) < 0.00000001)
                //{
                //    return TGCVector3.Empty;
                //}
                return _vectorEntrada;
            }
            set
            {
                if(TGCVector3.Length(_vectorEntrada) == 0)
                {
                    _vectorEntrada = new TGCVector3(value.X, 0f, value.Z);
                }
                else
                {
                    TGCVector3 versor = value * (1/TGCVector3.Length(value));
                    _vectorEntrada = versor * TGCVector3.Length(_vectorEntrada);
                }
            }
        }


        public TGCVector3 VersorEntrada
        {
            get { return _vectorEntrada * (1 / TGCVector3.Length(_vectorEntrada)); }
        }


        public bool AutoTransform
        {
            get
            {
                return RenderMesh.AutoTransform;
            }
            set
            {
                RenderMesh.AutoTransform = value;
            }
        }

        public TGCMatrix Transform
        {
            get
            {
                return RenderMesh.Transform;
            }
            set
            {
                RenderMesh.Transform = value;
            }
        }
        public PisoInercia(TgcMesh mesh)
        {
            this.RenderMesh = mesh;
        }

        public PisoInercia(TgcMesh mesh, float aceleracion)
        {
            this.RenderMesh = mesh;
            this.AceleracionFrenada = aceleracion;
        }

        //public static PisoInercia fromSize(TGCVector3 size)
        //{
        //    return new PisoInercia(TGCBox.fromSize(size));
        //}    

        public void Render()
        {
            RenderMesh.Render();
        }

        //public void updateValues()
        //{
        //    RenderMesh.updateValues();
        //}

        public bool aCollisionFound(TgcSkeletalMesh personaje)
        {
            var collisionResult = TgcCollisionUtils.classifyBoxBox(personaje.BoundingBox, this.SlidingBoundingBox);
            return (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera);
        }

    }


}
