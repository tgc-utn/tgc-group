using TGC.Group.Model.Items.Type;

namespace TGC.Group.Model.Items
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }

        ItemType type { get; }
        //TODO icon
    }
}