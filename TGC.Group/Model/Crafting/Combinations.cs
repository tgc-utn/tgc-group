using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TGC.Group.Model.Crafting
{
    //Esta clase almacena todas las posibles combinaciones en el juego y tambien las produce para cualquier inventario.
    static class Combinations
    {
        private static List<Combination> combinations = new List<Combination>();

        public static Item Combine(Item a, Item b) {
            var combination = GetCombination(a, b);
            if (combination != null && CanCombine(a, b, combination))
            {
                a.Take(combination.AmountA());
                b.Take(combination.AmountB());
                return combination.Result();
            }
            else
                return null;
        }

        private static bool CanCombine(Item a, Item b, Combination combination) { return combination.IsEnough(a, b); }


        //List Functions//

        private static Combination GetCombination(Item a, Item b) { return combinations.Find(c => CombinationMatches(c, a, b)); }

        private static bool CombinationMatches(Combination combination, Item a, Item b)
        {
            //Check
            if (combination.IsSameItemA(a))
                if (combination.IsSameItemB(b))
                    return true;

            /*Check but reversed
            if (combination.IsSameItemB(a))
                if (combination.IsSameItemA(b))
                    return true;
            */

            //Not the same
            return false;
        }

    }
}
