using System;
using System.Collections.Generic;
using TGC.Group.Model.Items.Equipment;

namespace TGC.Group.Model.Player
{
    public class Equipment
    {
        private List<IEquipable> equipables = new List<IEquipable>();

        public Stats ExtraStats()
        {
            var res = new Stats(0, 0);
            this.equipables.ForEach(equipable => equipable.ApplyEffect(res));

            return res;
        }

        public void AddEquipable(IEquipable equipable)
        {
            this.equipables.Add(equipable);
        }
    }
}