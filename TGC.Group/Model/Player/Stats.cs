using System;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model.Player
{
    public class Stats
    {
        public float Oxygen { get; set; }
        public int Life { get; set; }

        public Stats(float oxygen, int life)
        {
            this.Oxygen = oxygen;
            this.Life = life;
        }

        public static Stats operator +(Stats left, Stats right)
        {
            return new Stats(left.Oxygen + right.Oxygen,
                left.Life + right.Life);
        }

        public void Update(Stats toAdd, Stats maxStats)
        {
            this.Oxygen = Math.Min(this.Oxygen + toAdd.Oxygen, maxStats.Oxygen);
            this.Life = Math.Min(this.Life + toAdd.Life, maxStats.Life);
        }
    }
}