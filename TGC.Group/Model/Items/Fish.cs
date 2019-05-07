using TGC.Group.Model.Items.Type;

namespace TGC.Group.Model.Items
{
    public class Fish : IItem
    {
        public string Name { get; } = "Fish";
        public string Description { get; } = "Looks delicious!";
        public ItemType type { get; } = ItemType.CONSUMABLE;
    }
}