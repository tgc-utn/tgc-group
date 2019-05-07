using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public abstract class Collisionable
    {
        public bool CheckCollision { get; set; } = true;

        public abstract IRenderObject getCollisionVolume();
    }
}
