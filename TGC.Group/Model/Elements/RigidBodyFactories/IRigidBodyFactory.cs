using System;
using BulletSharp;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    interface IRigidBodyFactory
    {
        RigidBody Create(TgcMesh mesh);

    }

}