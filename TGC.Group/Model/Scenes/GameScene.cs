using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Text;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Model.Input;
using TGC.Core.SkeletalAnimation;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Core.Terrain;

namespace TGC.Group.Model.Scenes
{
    class GameScene : Scene
    {
        readonly TgcText2D DrawText = new TgcText2D();
        private World World { get; }
        private bool BoundingBox { get; set; }

        public delegate void Callback();
        Callback onEscapeCallback = () => {};

        public GameScene(TgcD3dInput input, string mediaDir) : base(input)
        { 
            PhysicsWorld.Init();
            backgroundColor = Color.FromArgb(1, 78, 129, 179);
            World = new World(new TGCVector3(0, 0, 0));
            Camera = new Camera(new TGCVector3(30, 30, 200), input);
        }

        public override void Update(float elapsedTime)
        {

            PhysicsWorld.DynamicsWorld.StepSimulation(elapsedTime);

            CollisionManager.CheckCollitions(this.World.GetCollisionables());

            this.World.Update(this.Camera.Position);

            if (GameInput.Statistic.IsPressed(Input))
            {
                this.BoundingBox = !this.BoundingBox;
            }
            if (GameInput.Escape.IsPressed(Input))
            {
                onEscapeCallback();
            }
        }
        public override void Render()
        {
            ClearScreen();
            this.DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            this.DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(this.Camera.Position), 0, 30, Color.OrangeRed);
            this.World.Render(this.Camera.Position);

            if (this.BoundingBox) {
                this.World.RenderBoundingBox(this.Camera.Position);
            }
        }

        public override void Dispose()
        {
            this.World.Dispose();
        }


        public GameScene OnEscape(Callback onEscapeCallback)
        {
            this.onEscapeCallback = onEscapeCallback;
            return this;
        }
    }
}
