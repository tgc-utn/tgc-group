using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Input;
using TGC.Core.Text;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Input;
using TGC.Group.Model.Items;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Player;

namespace TGC.Group.Model.Scenes
{
    class GameScene : Scene
    {
        readonly TgcText2D DrawText = new TgcText2D();
        private World World { get; }
        private bool BoundingBox { get; set; }
        
        private string mediaDir;

        public delegate void Callback();
        Callback onEscapeCallback = () => {};
        
        private Character character = new Character();

        private float oneSecond = 0; //TODO remove
        private bool gaveOxygenTank = false; //TODO remove

        public GameScene(TgcD3dInput input, string mediaDir) : base(input)
        {
            this.mediaDir = mediaDir;
            backgroundColor = Color.FromArgb(1, 78, 129, 179);
            World = new World(new TGCVector3(0, 0, 0));
            Camera = new Camera(new TGCVector3(30, 30, 200), input);
        }

        public override void Update(float elapsedTime)
        {
            this.oneSecond += elapsedTime;
            
            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(elapsedTime);

            CollisionManager.CheckCollitions(this.World.GetCollisionables());

            this.World.Update(this.Camera);

            var item = manageSelectableElement(this.World.SelectableElement); // Important: get this AFTER updating the world
            
            if(item != null)
                this.character.GiveItem(item);

            //TODO crafter logic, move to crafter when coded
            if (OxygenTank.Recipe.CanCraft(this.character.inventory.AsIngredients()) && !this.gaveOxygenTank)
            {
                this.character.RemoveIngredients(OxygenTank.Recipe.Ingredients);
                var oxygenTank = new OxygenTank();
                this.character.GiveItem(oxygenTank);
                
                ///////TODO when UI is ready, the selected element will be equipped
                this.character.Equip(oxygenTank);

                this.gaveOxygenTank = true;
            }
            //***********************************************

            if (this.oneSecond > 1.0f)
            {
                this.oneSecond = 0;
                this.character.UpdateStats(new Stats(-1,0));
            }
            
            if (GameInput.Statistic.IsPressed(this.Input))
            {
                this.BoundingBox = !this.BoundingBox;
            }
            if (GameInput.Escape.IsPressed(this.Input))
            {
                onEscapeCallback();
            }
        }

        private IItem manageSelectableElement(Element element)
        {
            if (element == null) return null;
            IItem item = null;

            element.Selectable = true;
            
            if (GameInput.Enter.IsPressed(this.Input))
            {
                this.World.Remove(element);
                item = element.item;
            }

            return item;
        }

        public override void Render()
        {
            ClearScreen();
            this.DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            this.DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(this.Camera.Position), 0, 30, Color.OrangeRed);
            this.DrawText.drawText("Oxygen = " + this.character.ActualStats.Oxygen + "/" + this.character.MaxStats.Oxygen, 0, 60, Color.Bisque);
            this.World.Render(this.Camera);

            if (this.BoundingBox) {
                this.World.RenderBoundingBox(this.Camera);
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
