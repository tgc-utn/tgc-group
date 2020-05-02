using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Crafting
{
    class Combination
    {
        private Item a;
        private Item b;
        private Item result;
        
        public Combination(Item a, Item b, Item result) { this.a = a; this.b = b; this.result = result; } //Used for instancing
        public bool IsEnough(Item itemA, Item itemB) { return itemA.Amount() >= a.Amount() && itemB.Amount() >= AmountB(); }
        public int AmountA() { return a.Amount(); }
        public int AmountB() { return b.Amount(); }
        public bool IsSameItemA(Item item) { return a.IsSameItem(item); }
        public bool IsSameItemB(Item item) { return b.IsSameItem(item); }

        public Item ItemA() { return a; }
        public Item ItemB() { return b; }
        public Item Result() { return result; }
    }
}
