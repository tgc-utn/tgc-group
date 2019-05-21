using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public class Gold : IItem
    {
        public string Name { get; } = "Gold";
        public string Description { get; } = "A piece of gold found under the sea.";
        public ItemType type { get; } = ItemType.MATERIAL;
        public CustomSprite Icon { get; set; }
        public TGCVector2 DefaultScale { get; } = new TGCVector2(.05f, .05f);
        public Gold()
        {
            CustomSprite icon = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Plant);
            icon.Scaling = DefaultScale;
            Icon = icon;
        }
    }
}