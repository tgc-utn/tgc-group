using System.Collections.Generic;
using System.Linq;

namespace TGC.Group.Model.Items.Recipes
{
    public class Recipe
    {
        public IEnumerable<Ingredient> Ingredients{get;}

        public Recipe(IEnumerable<Ingredient> ingredients)
        {
            this.Ingredients = ingredients;
        }

        public bool CanCraft(List<Ingredient> availableIngredients)
        {
            return this.Ingredients.All(availableIngredients.Contains);
        }
    }
}