using System;

namespace TGC.Group.Model.Utils
{
    class SpawnRate
    {
        private static readonly Random _random = new Random();
        private readonly int _min;
        private readonly int _max;

        private SpawnRate(int min, int max)
        {
            this._min = min;
            this._max = max;
        }

        public static SpawnRate of(int min, int max)
        {
            return new SpawnRate(min, max);
        }

        public bool hasToSpawn()
        {
            return this._min >= _random.Next(this._max);
        }
    }
}