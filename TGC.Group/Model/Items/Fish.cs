using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public class Fish : IItem
    {
        public string Name { get; } = "Fish";
        public string Description { get; } = "Looks delicious!";
        public ItemType type { get; } = ItemType.CONSUMABLE;
        public CustomSprite Icon { get;set; }

        public TGCVector2 DefaultScale { get; } = new TGCVector2(.1f, .05f);

        public Fish()
        {
            CustomSprite icon = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Fish);
            icon.Scaling = DefaultScale;
            Icon = icon;
        }
    }
}