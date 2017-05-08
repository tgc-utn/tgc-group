using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BsVector3 = BulletSharp.Math.Vector3;
using DxVector3 = Microsoft.DirectX.Vector3;

namespace TGC.Group.Wrappers
{
    public class Vector
    {
        public float X;
        public float Y;
        public float Z;
        public Vector(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            
        }
        public Vector(DxVector3 v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            
        }
        public Vector(BsVector3 v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            
        }

        public DxVector3 ToDxVector
        {
            get { return new DxVector3(X, Y, Z); }
        }
        public BsVector3 ToBsVector
        {
            get { return new BsVector3(X, Y, Z); }
        }
    }
}
