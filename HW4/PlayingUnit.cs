using System;

namespace HW4
{
    /// <summary>
    /// Represents a playing unit
    /// </summary>
    [XMLSerializable(typeof(XMLPropertyClassSerializer<PlayingUnit>))]
    public class PlayingUnit : Unit
    {
        /// <summary>
        /// Enum of unit states.
        /// </summary>
        public enum UnitState
        {
            Attacking = 0,
            Running = 1,
            Defence = 2
        }

        /// <summary>
        /// Represents this Unit's state.
        /// </summary>
        public UnitState State { get; set; }

        /// <summary>
        /// Returns this unit's attack points.
        /// </summary>
        public double AttackPoints => Damage * Strength / 10 + Armor * Agilty / 10;

        /// <summary>
        /// Returns an array of possible unit states
        /// </summary>
        public static UnitState[] States { get; }

        static PlayingUnit()
        {
            States = (UnitState[])Enum.GetValues(typeof(UnitState));
        }

        public PlayingUnit()
        {

        }

        /// <summary>
        /// Creates an instance of this class copying the values from the Unit class provided.
        /// </summary>
        public PlayingUnit(Unit unit)
        {
            unit.CopyTo(this);
        }

    }
}
