using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using TGC.Group.Model.Items;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Items.Recipes;

namespace TGC.Group.Model.Player
{
    public class Inventory
    {
        private List<IItem> Items { get; }
        private readonly int size;

        public Inventory(int size)
        {
            this.Items = new List<IItem>(size);
            this.size = size;
        }

        public void AddItem(IItem item)
        {
            if (this.Items.Count >= this.size)
            {
                return; // TODO error message
            }
            this.Items.Add(item);
        }

        public void RemoveItem(IItem item)
        {
            this.Items.Remove(item);
        }

        public List<Ingredient> AsIngredients()
        {
            return this.Items
                .ConvertAll(item => new Ingredient(item, this.Items.FindAll(item2 => item.Name == item2.Name).Count))
                .Distinct()
                .ToList();
        }

        public void RemoveIngredients(IEnumerable<Ingredient> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                for (var i = 0; i < ingredient.Quantity; i++)
                {
                    this.Items.Remove(ingredient.Item);
                }
            }
        }
    }
}