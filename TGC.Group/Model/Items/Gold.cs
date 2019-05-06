using TGC.Group.Model.Items.Type;

namespace TGC.Group.Model.Items
{
    public class Gold:IItem
    {
        public string Name { get; } = "Gold";
        public string Description { get; } = "A piece of gold found under the sea.";
        public ItemType type { get; } = ItemType.MATERIAL;
    }
}