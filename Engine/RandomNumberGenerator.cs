using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class RandomNumberGenerator
    {
        static Random generator = new Random();

        public static int NumberBetween(int min, int max)
        {
            return generator.Next(min, max + 1);
        }
    }
}
