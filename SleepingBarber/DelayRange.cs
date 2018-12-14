using System;

namespace SleepingBarberAsync
{
    public class DelayRange
    {
        private readonly Random _rand = new Random();

        public int GetDelay()
        {
            return _rand.Next(Min, Max);
        }

        public DelayRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Max { get; }
        public int Min { get; }
    }
}