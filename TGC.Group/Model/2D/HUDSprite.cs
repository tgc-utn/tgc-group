using TGC.Core.Mathematica;

namespace TGC.Group.Model._2D
{
    class HUDSprite : HUD
    {
        private CustomSprite sprite;

        public HUDSprite(AnclajeHorizontal anclajeHorizontal, AnclajeVertical anclajeVertical, TGCVector2 desplazamiento, TGCVector2 escala, Drawer2D drawer2D, CustomSprite sprite) : base(anclajeHorizontal, anclajeVertical, desplazamiento, escala, drawer2D)
        {
            this.sprite = sprite;
        }

        private void escalar()
        {
            sprite.Scaling = escala * scalingFactor;
        }

        protected override TGCVector2 getSize()
        {
            return new TGCVector2(sprite.Scaling.X * sprite.Bitmap.Width, sprite.Scaling.Y * sprite.Bitmap.Height);
        }

        public override void Init()
        {
            escalar();
            sprite.Position = trasladar();
        }

        public override void Render()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(sprite);
            drawer2D.EndDrawSprite();
        }

        public override void Dispose()
        {
            base.Dispose();
            sprite.Dispose();
        }
    }
}
