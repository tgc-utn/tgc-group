using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    class Plataforma {
        protected TGCBox box;

        public Plataforma(TGCVector3 pos, TGCVector3 size, TgcTexture textura) {
            box = TGCBox.fromSize(size, textura);

            box.Move(pos);
            box.Transform = TGCMatrix.Translation(pos);
        }

        public void render() {
            box.Render();
        }

        public void dispose() {
            box.Dispose();
        }

        public TgcBoundingAxisAlignBox getAABB() {
            return box.BoundingBox;
        }

    }
}
