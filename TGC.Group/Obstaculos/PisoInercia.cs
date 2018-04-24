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


namespace TGC.Group.Obstaculos
{

    public class PisoInercia
    {
        private TGCVector3 _vectorEntrada;
        public float AceleracionFrenada { get; set; }
        public TGCBox RenderBox { get; }//Por ahora uso un TGCBox ya que son mas faciles de manejar que las mesh
        private TGCBox SlidingBox;

        private bool seMovioBB = false;
        public TgcBoundingAxisAlignBox BoundingBox
        {
            get
            {
                return RenderBox.BoundingBox;
            }
        }
        public TgcBoundingAxisAlignBox SlidingBoundingBox
        {
            get
            {
                if (!seMovioBB)
                {
                    SlidingBox = RenderBox.clone();
                    SlidingBox.BoundingBox.move(new TGCVector3(0, 20, 0));
                    seMovioBB = true;
                }
                return SlidingBox.BoundingBox;
            }
        }
        public TGCVector3 Position
        {
            get
            {
                return RenderBox.Position;
            }
            set
            {
                RenderBox.Position = value;
            }
        }

        public TGCVector3 VectorEntrada
        {
            get
            {
                _vectorEntrada = _vectorEntrada * AceleracionFrenada;
                //if (_vectorEntrada.X < AceleracionFrenada || _vectorEntrada.Z < AceleracionFrenada)
                //{
                //    return TGCVector3.Empty;
                //}
                return _vectorEntrada;
            }
            set
            {
                _vectorEntrada = new TGCVector3(value.X, 0f, value.Z);
            }
        }

        public bool AutoTransform
        {
            get
            {
                return RenderBox.AutoTransform;
            }
            set
            {
                RenderBox.AutoTransform = value;
            }
        }

        public TGCMatrix Transform
        {
            get
            {
                return RenderBox.Transform;
            }
            set
            {
                RenderBox.Transform = value;
            }
        }
        public PisoInercia(TGCBox box)
        {
            this.RenderBox = box;
        }

        public static PisoInercia fromSize(TGCVector3 size)
        {
            return new PisoInercia(TGCBox.fromSize(size));
        }    

        public void Render()
        {
            RenderBox.Render();
        }

        public void updateValues()
        {
            RenderBox.updateValues();
        }

        public bool aCollisionFound(TgcSkeletalMesh personaje)
        {
            var collisionResult = TgcCollisionUtils.classifyBoxBox(personaje.BoundingBox, this.SlidingBoundingBox);
            return (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera);
        }

    }


}
