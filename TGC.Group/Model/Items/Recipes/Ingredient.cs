using System.Collections.Generic;

namespace TGC.Group.Model.Items.Recipes
{
    public class Ingredient
    {
        public IItem Item { get; }
        public int Quantity { get; }

        public Ingredient(IItem item, int quantity)
        {
            this.Item = item;
            this.Quantity = quantity;
        }

        public override bool Equals(object o)
        {
            if (ReferenceEquals(this, o)) return true;
            if (ReferenceEquals(this, null)) return false;
            if (ReferenceEquals(o, null)) return false;
            if (this.GetType() != o.GetType()) return false;
            var oo = (Ingredient) o;

            return Equals(this.Item.Name, oo.Item.Name) && this.Quantity == oo.Quantity;
        }

        public override int GetHashCode()
        {
            return this.Item.Name.GetHashCode() ^ this.Quantity;
        }
    }
}