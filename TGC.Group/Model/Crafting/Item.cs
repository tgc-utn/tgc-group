using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    class Item
    {
        private string name;
        private int amount;

        public Item(string name, int amount) { this.name = name; this.amount = amount; }

        public void Take(int amount) { this.amount -= amount; }
        public void Add(int amount) { this.amount += amount; }
        public bool IsSameItem(Item item) { return this.name == item.name; }
        public bool NoAmountLeft() { return amount <= 0; }

        public int Amount() { return amount; }
    }
}
