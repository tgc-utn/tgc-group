﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Crafting
{
    class Inventory
    {
        private List<Item> inventory = new List<Item>();

        public void Add(Item item) {
            Item inventoryItem = inventory.Find(i => ItemMatches(i,item));
            if (inventoryItem != null)
                inventoryItem.Add(item.Amount());
            else
                inventory.Add(item); 
        }
        public void Combine(Item a, Item b) { 
            Combinations.Combine(a, b);
            if (a.Amount() <= 0) inventory.Remove(a);
            if (b.Amount() <= 0) inventory.Remove(b);
        }
        private bool ItemMatches(Item inventoryItem, Item item) { return inventoryItem.IsSameItem(item); }
    }
}
