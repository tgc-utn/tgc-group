using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class AquaticPhysics
    { 
        private static AquaticPhysics instance = null;

        public static AquaticPhysics Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AquaticPhysics();
                    instance.Init();
                }
                return instance;
            }
        }

        public DiscreteDynamicsWorld DynamicsWorld;
        private AquaticPhysics() {}

        
        private void Init()
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

        public void Add(RigidBody physicsBody)
        {
            DynamicsWorld.AddRigidBody(physicsBody);   
        }

        internal void Remove(RigidBody physicsBody)
        {
            DynamicsWorld.RemoveRigidBody(physicsBody);
        }
    }
}
