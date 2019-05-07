using TGC.Group.Model.Player;

namespace TGC.Group.Model.Items.Equipment
{
    public interface IEquipable : IItem
    {
        void ApplyEffect(Stats character);
    }
}