using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Group.Model.Entities;

namespace TGC.Group.Model
{
    
    
    
    
    
    
    public class GameModel : TgcExample
    {
        
        
        
        
        
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        
        private Player player;


        
        private bool BoundingBox { get; set; }


        

        public override void Init()
        {
            
            var d3dDevice = D3DDevice.Instance.Device;
            
            player = new Player(new TgcSceneLoader().loadSceneFromFile(MediaDir + "/ModelosTgc/Robot/Robot-TgcScene.xml").Meshes[0]);
            player.rotateY((float)Math.PI);
            player.Position = new Vector3(0, 0, 50);
            player.shouldRenderBoundingBox = true;


            
            var cameraPosition = new Vector3(0, 0, 125);
            
            var lookAt = Vector3.Empty;


            Camara.SetCamera(cameraPosition, lookAt);

        }

        
        
        
        
        
        public override void Update()
        {
            PreUpdate();

            player.update();
            
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                Camara.SetCamera(Camara.Position + new Vector3(0, 10f, 0), Camara.LookAt);
                
                if (Camara.Position.Y > 300f)
                {
                    Camara.SetCamera(new Vector3(Camara.Position.X, 0f, Camara.Position.Z), Camara.LookAt);
                }
            }
            
        }

        
        
        
        
        
        public override void Render()
        {
            
            PreRender();

            
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TgcParserUtils.printVector3(Camara.Position), 0, 30, Color.OrangeRed);

            player.render();

            PostRender();
        }

        
        
        
        
        
        public override void Dispose()
        {
            player.dispose();  
        }
    }
}