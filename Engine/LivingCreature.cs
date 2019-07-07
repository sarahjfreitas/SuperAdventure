using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class LivingCreature
    {
        public int MaximumHitPoints { get; set; }
        public int CurrentHitPoints { get; set; }
        public int Level { get; set; }

        public LivingCreature(int currentHitPoints, int maximumHitPoints, int level = 1)
        {
            CurrentHitPoints = currentHitPoints;
            MaximumHitPoints = maximumHitPoints;
            Level = level;
        }
    }
}
