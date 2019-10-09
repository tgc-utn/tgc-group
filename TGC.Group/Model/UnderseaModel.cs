using System.Drawing;
using TGC.Core.Example;
using TGC.Group.Model.Levels;

namespace TGC.Group.Model
{
    public class UnderseaModel : TgcExample
    {
        private Level1Model currentLevel;

        public UnderseaModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            currentLevel = new Level1Model(Camara, Input, MediaDir, Frustum);

            currentLevel.Init();

            Camara = currentLevel.Camera;

            BackgroundColor = Color.Black;
        }

        public override void Update()
        {
            PreUpdate();

            currentLevel.Update(ElapsedTime);

            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            currentLevel.Render(ElapsedTime);

            PostRender();
        }

        public override void Dispose()
        {
            currentLevel.Dispose();
        }
    }
}
