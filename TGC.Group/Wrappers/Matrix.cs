using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp.Math;
using BsMatrix = BulletSharp.Math.Matrix;
using DxMatrix = Microsoft.DirectX.Matrix;

namespace TGC.Group.Wrappers
{
    public class Matrix
    {
        public float M11;
        //
        // Summary:
        //     Retrieves or sets the element in the first row and the second column of the matrix.
        public float M12;
        //
        // Summary:
        //     Retrieves or sets the element in the first row and the third column of the matrix.
        public float M13;
        //
        // Summary:
        //     Retrieves or sets the element in the first row and the fourth column of the matrix.
        public float M14;
        //
        // Summary:
        //     Retrieves or sets the element in the second row and the first column of the matrix.
        public float M21;
        //
        // Summary:
        //     Retrieves or sets the element in the second row and the second column of the
        //     matrix.
        public float M22;
        //
        // Summary:
        //     Retrieves or sets the element in the second row and the third column of the matrix.
        public float M23;

        //
        // Summary:
        //     Retrieves or sets the element in the second row and the fourth column of the
        //     matrix.
        public float M24;
        //
        // Summary:
        //     Retrieves or sets the element in the third row and the first column of the matrix.
        public float M31;
        //
        // Summary:
        //     Retrieves or sets the element in the third row and the second column of the matrix.
        public float M32;
        //
        // Summary:
        //     Retrieves or sets the element in the third row and the third column of the matrix.
        public float M33;
        //
        // Summary:
        //     Retrieves or sets the element in the third row and the fourth column of the matrix.
        public float M34;
        //
        // Summary:
        //     Retrieves or sets the element in the fourth row and the first column of the matrix.
        public float M41;
        //
        // Summary:
        //     Retrieves or sets the element in the fourth row and the second column of the
        //     matrix.
        public float M42;
        //
        // Summary:
        //     Retrieves or sets the element in the fourth row and the third column of the matrix.
        public float M43;
        //
        // Summary:
        //     Retrieves or sets the element in the fourth row and the fourth column of the
        //     matrix.
        public float M44;

        public Matrix() { }

        public Matrix(DxMatrix m)
        {
            this.M11 = m.M11;
            this.M12 = m.M12;
            this.M13 = m.M13;
            this.M14 = m.M14;
            this.M21 = m.M21;
            this.M22 = m.M22;
            this.M23 = m.M23;
            this.M24 = m.M24;
            this.M31 = m.M31;
            this.M32 = m.M32;
            this.M33 = m.M33;
            this.M34 = m.M34;
            this.M41 = m.M41;
            this.M42 = m.M42;
            this.M43 = m.M43;
            this.M44 = m.M44;
        }

        public Matrix(BsMatrix m)
        {
            this.M11 = m.M11;
            this.M12 = m.M12;
            this.M13 = m.M13;
            this.M14 = m.M14;
            this.M21 = m.M21;
            this.M22 = m.M22;
            this.M23 = m.M23;
            this.M24 = m.M24;
            this.M31 = m.M31;
            this.M32 = m.M32;
            this.M33 = m.M33;
            this.M34 = m.M34;
            this.M41 = m.M41;
            this.M42 = m.M42;
            this.M43 = m.M43;
            this.M44 = m.M44;
        }

        public DxMatrix ToDxMatrix
        {
            get
            {
                var m = new DxMatrix();
                m.M11 = this.M11;
                m.M12 = this.M12;
                m.M13 = this.M13;
                m.M14 = this.M14;
                m.M21 = this.M21;
                m.M22 = this.M22;
                m.M23 = this.M23;
                m.M24 = this.M24;
                m.M31 = this.M31;
                m.M32 = this.M32;
                m.M33 = this.M33;
                m.M34 = this.M34;
                m.M41 = this.M41;
                m.M42 = this.M42;
                m.M43 = this.M43;
                m.M44 = this.M44;
                return m;
            }
        }

        public BsMatrix ToBsMatrix
        {
            get
            {
                var m = new BsMatrix();
                m.M11 = this.M11;
                m.M12 = this.M12;
                m.M13 = this.M13;
                m.M14 = this.M14;
                m.M21 = this.M21;
                m.M22 = this.M22;
                m.M23 = this.M23;
                m.M24 = this.M24;
                m.M31 = this.M31;
                m.M32 = this.M32;
                m.M33 = this.M33;
                m.M34 = this.M34;
                m.M41 = this.M41;
                m.M42 = this.M42;
                m.M43 = this.M43;
                m.M44 = this.M44;
                return m;
            }
        }

        public static Matrix Scaling(int x, int y, int z)
        {
            return new Matrix(DxMatrix.Scaling(x,y,z));
        }

        public static Matrix RotationYawPitchRoll(float yaw, float pitch, float roll)
        {
            return new Matrix(BsMatrix.RotationYawPitchRoll(yaw, pitch, roll));                
        }

        public static Matrix Identity
        {
            get
            {
                Matrix result = new Matrix();
                result.M11 = 1.0f;
                result.M22 = 1.0f;
                result.M33 = 1.0f;
                result.M44 = 1.0f;

                return result;
            }
        }

        public Vector Origin
        {
            get { return new Vector(M41, M42, M43); }
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
            }
        }
    }
}
