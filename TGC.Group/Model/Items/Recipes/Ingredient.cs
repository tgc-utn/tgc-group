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
            return 
                ReferenceEquals(this, o) ||
                !ReferenceEquals(this, null) &&
                !ReferenceEquals(o, null) &&
                this.GetType() == o.GetType() &&
                Equals(this.Item.Name, ((Ingredient) o).Item.Name) && this.Quantity == ((Ingredient) o).Quantity;
        }

        public override int GetHashCode()
        {
            return this.Item.Name.GetHashCode() ^ this.Quantity;
        }
    }
}