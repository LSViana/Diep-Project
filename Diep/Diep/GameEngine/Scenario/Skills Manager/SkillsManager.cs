using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Skills_Manager
{
    public class SkillsManager
    {
        public SkillsManager()
        {
            // Standard Property Values
            shootSpeed = StandardShootSpeed;
            ShootImpact = StandardShootImpact;
            ShootRate = StandardShootRate;
        }

        //public const Single MinShootSpeed = .000015f;
        public const Single MinShootSpeed = .00006f;
        public const Single MaxShootSpeed = .0003f;
        public const Single StandardShootSpeed = .00015f;
        public const float MinShootRate = 0;
        public const float MaxShootRate = 10f;
        public const float StandardShootRate = 1f;
        public const float MinEndurance = 0f;
        public const float MaxEndurance = .8f;
        public const float MinShootImpact = -1;
        public const float MaxShootImpact = 2;
        public const float StandardShootImpact = 1;
        private float shootSpeed;
        private Single power;
        private float endurance;
        private float shootRate;
        private float shootImpact;

        public Single Power
        {
            get { return power; }
            set { if (value > 1) value = 1; else if (value < 0) value = 0; power = value; }
        }

        public float Endurance
        {
            get { return endurance; }
            set { if (value < MinShootSpeed) value = MinShootSpeed; else if (value > MaxShootSpeed) value = MaxShootSpeed; endurance = value; }
        }

        public float ShootImpact
        {
            get { return shootImpact; }
            set { if (value < MinShootImpact) value = MinShootImpact; else if (value > MaxShootImpact) value = MaxShootImpact; shootImpact = value; }
        }

        public Single ShootSpeed
        {
            get { return shootSpeed; }
            set { if (value < MinShootSpeed) value = MinShootSpeed; else if (value > MaxShootSpeed) value = MaxShootSpeed; shootSpeed = value; }
        }

        public Single ShootsPerSecond { get; set; }

        public Single ShootRate
        {
            get { return shootRate; }
            set { if (value > MaxShootRate) value = MaxShootRate; else if (value < MinShootRate) value = MinShootRate; shootRate = value; }
        }

        public Single MillisecondsPerShoot { get { return 1000 / ShootRate; } }
    }
}
