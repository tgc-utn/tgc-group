using System;

namespace TGC.Group.Model.Utils
{
    public class SpamRate
    {
        private static readonly Random _random = new Random();
        private readonly int _min;
        private readonly int _max;

        private SpamRate(int min, int max)
        {
            this._min = min;
            this._max = max;
        }

        public static SpamRate of(int min, int max)
        {
            return new SpamRate(min, max);
        }

        public bool spam()
        {
            return this._min >= _random.Next(this._max);
        }
    }
}