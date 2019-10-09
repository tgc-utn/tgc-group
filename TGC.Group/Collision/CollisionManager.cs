using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Camera;

namespace TGC.Group.Collision
{
    public class CollisionManager
    {
        private TgcBoundingCylinder cilindroColision;
        private float leftrightRot;
        private float updownRot;
        private float rotationSpeed;
        private TgcD3dInput Input;
        private List<TgcMesh> collisionMeshes;

        /// <summary>
        ///     Crear nuevo manejador de colsiones
        /// </summary>
        public CollisionManager(TgcD3dInput input, TGCVector3 lookAt, float rotationSpeed)
        {
            Input = input;
            rotationSpeed = rotationSpeed;

            collisionMeshes = new List<TgcMesh>();

            cilindroColision = new TgcBoundingCylinder(lookAt, 100f, 500f);
            updownRot = Geometry.DegreeToRadian(90f) + (FastMath.PI / 10.0f);
            cilindroColision.rotateZ(updownRot);
            cilindroColision.setRenderColor(Color.LimeGreen);
            cilindroColision.updateValues();
        }

        /// <summary>
        ///     Actualizar colisiones y camara
        /// </summary>
        /// <returns>Nueva posicion de la camara</returns>
        public void update(TGCVector3 position, TGCVector3 lookAt)
        {
            cilindroColision.Center = lookAt;
            cilindroColision.move(lookAt - position);
            leftrightRot -= -Input.XposRelative * rotationSpeed;
            updownRot -= -Input.YposRelative * rotationSpeed;
            cilindroColision.Rotation = new TGCVector3(0, leftrightRot, updownRot);
            cilindroColision.updateValues();

            //FindCollision();
        }

        public void Render()
        {
            cilindroColision.Render();
        }

        public void Dispose()
        {
            cilindroColision.Dispose();
        }

        public void AddCollisionMesh(TgcMesh mesh)
        {
            collisionMeshes.Add(mesh);
        }

        public bool FindCollision()
        {
            List<TgcMesh> collisions = collisionMeshes.Where(objeto => HasCollision(cilindroColision, objeto)).ToList();
            if (collisions.Any())
            {
                cilindroColision.setRenderColor(Color.Red);
                var objetoAMostrar = collisions.First();

                return true;

                // TODO Inventario
                //if (Input.keyPressed(Key.E))
                //{
                //    GModel.Personaje.Inventario.agregaObjeto(new ObjetoInventario(objetoAMostrar.Nombre, 1));
                //    objetosRecolectables.Remove(objetoAMostrar);
                //    if (objetoAMostrar.CuerpoRigido != null)
                //    {
                //        dynamicsWorld.RemoveCollisionObject(objetoAMostrar.CuerpoRigido);
                //        objetoAMostrar.CuerpoRigido.Dispose();
                //    }
                //    objetoAMostrar = null;

                //    sonidoRecojoElemento.play();
                //}
            }
            else
            {
                cilindroColision.setRenderColor(Color.LimeGreen);
                //objetoAMostrar = null;

                return false;
            }
        }

        public bool HasCollision(TgcBoundingCylinder cilindro, TgcMesh mesh)
        {
            var boundingBox = mesh.BoundingBox;
            //var EsferaColision = new TgcBoundingSphere(mesh.Position, mesh.Scale.X);
            //return TgcCollisionUtils.testSphereCylinder(EsferaColision, cilindro);
            TgcBoundingCylinderFixedY cilindro2 = new TgcBoundingCylinderFixedY(cilindro.Center, cilindro.Radius, 1);
            return TgcCollisionUtils.testAABBCylinder(boundingBox, cilindro2);
        }
    }
}
