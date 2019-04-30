using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    public abstract class Collisionable
    {
        public bool CheckCollision { get; set; } = true;

        public abstract TgcBoundingAxisAlignBox getCollisionVolume();
    }
}
