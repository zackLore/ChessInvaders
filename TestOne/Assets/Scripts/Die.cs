using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts
{
    public class Die
    {
        // ****************************************************
        // Properties
        // ****************************************************
        public int CurrentRoll;
        public int LastRoll;
        public int UpperLimit;
        public int LowerLimit;

        // ****************************************************
        // Constructors
        // ****************************************************
        public Die()
        {
            UpperLimit = 6;
            LowerLimit = 1;
            CurrentRoll = 1;
            LastRoll = 1;
        }

        public Die(int upperLimit)
        {
            UpperLimit = upperLimit;
            LowerLimit = 1;
            CurrentRoll = 1;
            LastRoll = 1;
        }

        public Die(int upperLimit, int lowerLimit)
        {
            UpperLimit = upperLimit;
            LowerLimit = lowerLimit;
            CurrentRoll = 1;
            LastRoll = 1;
        }

        // ****************************************************
        // Public Methods
        // ****************************************************
        public int RollDie()
        {
            CurrentRoll = UnityEngine.Random.Range(UpperLimit, LowerLimit);
            return CurrentRoll;
        }
    }
}
