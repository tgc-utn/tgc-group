using System.Collections.Generic;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Player;

namespace TGC.Group.Model.Items
{
    public class Crafter
    {
        public ICrafteable CraftedItem;

        public List<ICrafteable> Crafteables { get; } = new List<ICrafteable>
        {
            new OxygenTank()
        };

        public List<ICrafteable> CrafteablesBy(List<Ingredient> ingredients)
        {
            return Crafteables.FindAll(crafteable => crafteable.CrafteableRecipe.CanCraft(ingredients));
        }

        public void Craft(ICrafteable crafteable, Character character)
        {
            if (!crafteable.CrafteableRecipe.CanCraft(character.Inventory.AsIngredients())) return;
            
            character.RemoveIngredients(crafteable.CrafteableRecipe.Ingredients);

            this.CraftedItem = crafteable;
        }
    }
}