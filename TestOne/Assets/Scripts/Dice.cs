using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts
{
    public class Dice : MonoBehaviour
    {
        // ****************************************************
        // Properties
        // ****************************************************
        public int CurrentTotal = 0;
        public int LastTotal = 0;
        public List<Die> DiceCollection = new List<Die>();

        // ****************************************************
        // Constructors
        // ****************************************************
        public Dice()
        {
        }

        public Dice(int numberOfDice, int diceUpperLimit)
        {
            DiceCollection.Clear();
            for(int i=0; i<numberOfDice; i++)
            {
                DiceCollection.Add(new Die(diceUpperLimit));
            }
        }

        // ****************************************************
        // Public Methods
        // ****************************************************
        public void InitDice(int numberOfDice, int diceUpperLimit)
        {
            DiceCollection.Clear();
            for (int i = 0; i < numberOfDice; i++)
            {
                DiceCollection.Add(new Die(diceUpperLimit));
            }
        }

        public void InitDice(int numberOfDice, int diceUpperLimit, int lowerLimit)
        {
            DiceCollection.Clear();
            for (int i = 0; i < numberOfDice; i++)
            {
                DiceCollection.Add(new Die(diceUpperLimit, lowerLimit));
            }
        }

        public int RollDice()
        {
            LastTotal = CurrentTotal;

            int runningTotal = 0;
            foreach(Die die in DiceCollection)
            {
                die.LastRoll = die.CurrentRoll;
                die.CurrentRoll = UnityEngine.Random.Range(die.UpperLimit, die.LowerLimit);
                runningTotal += die.CurrentRoll;
            }
            CurrentTotal = runningTotal;

            return CurrentTotal;
        }
    }
}
