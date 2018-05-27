using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    public class Plataforma : IRenderObject {
        protected TGCBox box;

        public bool AlphaBlendEnable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Plataforma(TGCVector3 pos, TGCVector3 size, TgcTexture textura) {
            box = TGCBox.fromSize(size, textura);

            box.Move(pos);
            box.Transform = TGCMatrix.Translation(pos);
        }

        public void Render() {
            box.Render();
        }

        public void Dispose() {
            box.Dispose();
        }

        public TgcBoundingAxisAlignBox getAABB() {
            return box.BoundingBox;
        }

    }
}
