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
        public Combination(Item a, Item b) { this.a = a; this.b = b; } //Used for List.Find() in Combinations class
        public bool IsEnough(Item itemA, Item itemB) { return itemA.Amount() >= AmountA() && itemB.Amount() >= AmountB(); }

        public Item ItemA() { return a; }
        public Item ItemB() { return b; }
        public int AmountA() { return a.Amount(); }
        public int AmountB() { return b.Amount(); }
        public Item Result() { return result; }
    }
}
