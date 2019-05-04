using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class PhysicsWorld
    {
        public static DiscreteDynamicsWorld DynamicsWorld;

        public static void Init()
        {
            var collisionConfiguration = new DefaultCollisionConfiguration();
            var dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            DynamicsWorld = new DiscreteDynamicsWorld(
                dispatcher,
                new DbvtBroadphase(),
                new SequentialImpulseConstraintSolver(),
                collisionConfiguration
            )
            {
                Gravity = new TGCVector3(0, -10f, 0).ToBulletVector3(),
            };

        }
    }
}
