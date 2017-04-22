using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Group.Model.Entities;
using TGC.Group.Model.GameWorld;

namespace TGC.Group.Model
{
    
    public class GameModel : TgcExample
    {

        private World world;
        
        
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {            
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {            
            var d3dDevice = D3DDevice.Instance.Device;
            this.world    = new World(MediaDir, this.Input);
            this.Camara   = this.world.Camera;
        }



        

        public override void Update()
        {
            PreUpdate();
            
            this.world.update(ElapsedTime);
        }

        
        public override void Render()
        {
            PreRender();
            this.world.render();
            PostRender();
        }


        public override void Dispose()
        {
            this.world.dispose();
        }
    }
}