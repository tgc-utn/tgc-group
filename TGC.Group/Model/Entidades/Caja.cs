using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    public class Caja : IRenderObject {
        private TgcBoundingAxisAlignBox cuerpo;
        private TgcBoundingAxisAlignBox superior;

        private TGCBox box;
        private TGCVector3 vel;

        bool alpha = true;
        public bool AlphaBlendEnable { get => alpha; set { alpha = value; } }

        public Caja(string mediaDir, TGCVector3 pos) :
            this(mediaDir, pos, new TGCVector3(80, 80, 80)) { }

        public Caja(string mediaDir, TGCVector3 pos, TGCVector3 size) {
            // TODO: tengo que hacer dispose de esta textura? la hago global?
            var texture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "caja.jpg");
            box = TGCBox.fromSize(size, texture);

            var minInferior = box.BoundingBox.PMin;
            var maxInferior = box.BoundingBox.PMax;
            var posInferior = box.BoundingBox.calculateBoxCenter();
            maxInferior.Y = minInferior.Y;
            posInferior.Y = minInferior.Y;

            var minSuperior = box.BoundingBox.PMin;
            var maxSuperior = box.BoundingBox.PMax;
            var posSuperior = box.BoundingBox.calculateBoxCenter();
            minSuperior.Y = maxSuperior.Y;
            posSuperior.Y = maxSuperior.Y;

            superior = new TgcBoundingAxisAlignBox(minSuperior, maxSuperior, posSuperior, TGCVector3.One);

            cuerpo = box.BoundingBox;

            // debug
            cuerpo.setRenderColor(Color.Yellow);
            superior.setRenderColor(Color.Red);

            move(pos);
            vel = TGCVector3.Empty;
        }

        public void setEffect(Effect e) {
            box.Effect = e;
            box.Technique = "RenderScene";
            // effect.SetValue("time", 0);
        }

        public void setTechnique(string t) {
            box.Technique = t;
        }

        public void Render() {
            box.Render();
            
            // para debuggear
            // renderizo en este orden asi superior e inferior, que tienen 1px de altura
            // puedan tapar a centro.
            cuerpo.Render();
            superior.Render();
        }

        public void Dispose() {
            box.Dispose();
        }

        public void move(TGCVector3 movement) {
            box.Move(movement);
            // no muevo centro porque es una referencia al bounding box de la caja,
            // que ya se mueve cuando hago move
            superior.move(movement);
            box.Transform = TGCMatrix.Translation(box.Position);
        }

        public void applyGravity() {
            box.Move(vel);
            superior.move(vel);
            box.Transform = TGCMatrix.Translation(box.Position);
        }

        public TgcBoundingAxisAlignBox getSuperior() {
            return superior;
        }

        public TgcBoundingAxisAlignBox getCuerpo() {
            return cuerpo;
        }

        public void addVel(TGCVector3 moreVel) {
            vel += moreVel;
        }

        public void resetVel() {
            vel = TGCVector3.Empty;
        }
    }
}
