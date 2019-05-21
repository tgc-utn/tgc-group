using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Type;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }

        ItemType type { get; }
        
        CustomSprite Icon { get; }

        TGCVector2 DefaultScale { get; }
    }
}