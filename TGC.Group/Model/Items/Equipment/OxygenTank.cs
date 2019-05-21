using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items.Equipment
{
    public class OxygenTank : ICrafteable, IEquipable
    {
        private const int Capacity = 10;
        public static readonly Recipe Recipe = 
            new Recipe(new List<Ingredient>{new Ingredient(new Gold(), 3)});

        public string Name { get; } = "Oxygen tank";
        public string Description { get; } = "Increments the oxygen capacity in " + Capacity;
        public ItemType type { get; } = ItemType.EQUIPABLE;
        public Recipe CrafteableRecipe { get; } = Recipe;

        public CustomSprite Icon { get; } = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Fish);
        public TGCVector2 DefaultScale { get; } = new TGCVector2(1f, 1f);

        public void ApplyEffect(Stats character)
        {
            character.Oxygen += Capacity;
        }
    }
}