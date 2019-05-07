using TGC.Group.Model.Items.Recipes;

namespace TGC.Group.Model.Items
{
    public interface ICrafteable:IItem
    {
        Recipe CrafteableRecipe { get; }
    }
}