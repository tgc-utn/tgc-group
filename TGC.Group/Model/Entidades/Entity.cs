using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Entidades
{
    class Entity
    {
        public void Init(string MediaDir) { 
            Entities.Add(this);
            InitEntity(MediaDir);
        }

        public void Update(float ElapsedTime) { UpdateEntity(ElapsedTime); }
        public void Render() { RenderEntity(); }

        public void Dispose() { Entities.Remove(this); DisposeEntity(); }

        public TgcMesh GetMesh() { return GetEntityMesh(); }


        //Override functions
        protected virtual TgcMesh GetEntityMesh() { return null; }
        protected virtual void InitEntity(string MediaDir) { }
        protected virtual void UpdateEntity(float ElapsedTime) { }
        protected virtual void RenderEntity() { }
        protected virtual void DisposeEntity() { }
    }
}
