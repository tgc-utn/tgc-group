using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    // TODO: deberian patinar tambien las cajas?
    // si se patinan, entonces deberiamos refactorizar esta clase y la clase personaje
    // en una sola clase "entidad fisica" o algo asi
    class Caja {
        TgcBoundingAxisAlignBox inferior;
        TgcBoundingAxisAlignBox centro;
        TgcBoundingAxisAlignBox superior;

        TGCBox box;
        TGCVector3 vel;

        public Caja(string mediaDir, TGCVector3 pos) {
            // TODO: tengo que hacer dispose de esta textura? la hago global?
            var texture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "caja.jpg");
            // TODO: pasar tamaño por parámetro
            box = TGCBox.fromSize(new TGCVector3(50, 50, 50),  texture);

            var minInferior = box.BoundingBox.PMin;
            var maxInferior = box.BoundingBox.PMax;
            var posInferior = box.BoundingBox.calculateBoxCenter();
            maxInferior.Y = minInferior.Y;
            posInferior.Y = minInferior.Y;

            inferior = new TgcBoundingAxisAlignBox(minInferior, maxInferior, posInferior, TGCVector3.One);

            var minSuperior = box.BoundingBox.PMin;
            var maxSuperior = box.BoundingBox.PMax;
            var posSuperior = box.BoundingBox.calculateBoxCenter();
            minSuperior.Y = maxSuperior.Y;
            posSuperior.Y = maxSuperior.Y;

            superior = new TgcBoundingAxisAlignBox(minSuperior, maxSuperior, posSuperior, TGCVector3.One);

            centro = box.BoundingBox;

            centro.setRenderColor(Color.Yellow);
            superior.setRenderColor(Color.Red);
            inferior.setRenderColor(Color.Green);

            move(pos);
        }

        public void render() {
            box.Render();
            
            // para debuggear
            // renderizo en este orden asi superior e inferior, que tienen 1px de altura
            // puedan tapar a centro.
            centro.Render();
            superior.Render();
            inferior.Render();
        }

        public void dispose() {
            box.Dispose();
        }

        public void move(TGCVector3 movement) {
            box.Move(movement);
            // no muevo centro porque es una referencia al bounding box de la caja,
            // que ya se mueve cuando hago move
            superior.move(movement);
            inferior.move(movement);
            box.Transform = TGCMatrix.Translation(box.BoundingBox.calculateBoxCenter());
        }

        public TgcBoundingAxisAlignBox getSuperior() {
            return superior;
        }

        public TgcBoundingAxisAlignBox getCentro() {
            return centro;
        }
        
        public TgcBoundingAxisAlignBox getInferior() {
            return inferior;
        }
    }
}
