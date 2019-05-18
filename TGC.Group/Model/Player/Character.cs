using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using BulletSharp;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Items.Recipes;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model.Player
{
    public class Character
    {
        private static readonly Stats BaseStats = new Stats(15 * 100, 100);

        public Stats MaxStats => BaseStats + this.equipment.ExtraStats();

        public Stats ActualStats { get; }
        public Inventory Inventory { get; } = new Inventory(30);

        private Equipment equipment = new Equipment();
        
        public Character()
        {
            this.ActualStats = this.MaxStats;
        }

        public void UpdateStats(Stats newStats)
        {
            this.ActualStats.Update(newStats, this.MaxStats);
        }

        public bool IsDead()
        {
            return this.ActualStats.Life == 0 || this.ActualStats.Oxygen == 0;
        }

        public void GiveItem(IItem item)
        {
            this.Inventory.AddItem(item);
        }

        public void Equip(IEquipable equipable)
        {
            this.Inventory.RemoveItem(equipable);
            this.equipment.AddEquipable(equipable);
        }

        public void RemoveIngredients(IEnumerable<Ingredient> recipeIngredients)
        {
            this.Inventory.RemoveIngredients(recipeIngredients);
        }

        public void RemoveItem(IItem item)
        {
            this.Inventory.RemoveItem(item);
        }
    }
}
