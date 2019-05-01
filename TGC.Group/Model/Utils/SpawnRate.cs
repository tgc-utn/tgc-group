using System;

namespace TGC.Group.Model.Utils
{
    class SpawnRate
    {
        private static readonly Random Random = new Random();
        private readonly int min;
        private readonly int max;

        private SpawnRate(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public static SpawnRate Of(int min, int max)
        {
            return new SpawnRate(min, max);
        }

        public bool HasToSpawn()
        {
            return this.min >= Random.Next(this.max);
        }
    }
}