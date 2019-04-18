using System;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    class Entity : Collisionable
    {
        public void Render()
        {
            return;
        }

        public void Update()
        {
            return;
        }

        public void Dispose()
        {
            return;
        }

        public override TgcBoundingAxisAlignBox getCollisionVolume()
        {
            throw new NotImplementedException();
        }
    }
}