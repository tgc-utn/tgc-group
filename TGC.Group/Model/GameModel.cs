
using TGC.Core.Direct3D;
using TGC.Core.Example;
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
        }

        
        public override void Update()
        {
            PreUpdate();
            this.Camara = this.world.Camera;
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