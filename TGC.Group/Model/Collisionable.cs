using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    abstract class Collisionable
    {
        public bool CheckCollision { get; set; } = true;

        public abstract TgcBoundingAxisAlignBox getCollisionVolume();
    }
}
