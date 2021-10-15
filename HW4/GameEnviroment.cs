using System;
namespace HW4
{
    /// <summary>
    /// Represents game enviroment
    /// </summary>
    [XMLSerializable(typeof(XMLPropertyClassSerializer<GameEnvironment>))]
    class GameEnvironment
    {
        private const double runningUnitHealthMultipler = 0.01;
        private const double minRunningUnitHealth = 2;
        private const double unitHealingProbability = 0.2;
        private const double unitHealingMultipler = 5;
        private const double unitStrengthDamageMultipler = 1D / 20;


        /// <summary>
        /// Enum of possible game results.
        /// </summary>
        public enum GameResult
        {
            InProcess = 0,
            WonByFirstPlayer = 1,
            WonBySecondPlayer = 2,
            Draw = 3
        }

        private static readonly Random r = new Random();

        /// <summary>
        /// Represents the first unit in this game.
        /// </summary>
        [XMLSerializable]
        public PlayingUnit First { get; set; }

        /// <summary>
        /// Represents the second unit in this game.
        /// </summary>
        [XMLSerializable]
        public PlayingUnit Second { get; set; }

        /// <summary>
        /// Returns this game's result
        /// </summary>
        public GameResult Result
        {
            get
            {
                GameResult result = GameResult.InProcess;
                if (First.Health <= 0)
                    result |= GameResult.WonBySecondPlayer;
                if (Second.Health <= 0)
                    result |= GameResult.WonByFirstPlayer;
                return result;
            }
        }

        /// <summary>
        /// Game finished event.
        /// </summary>
        public event Action<GameEnvironment> OnGameFinished;

        /// <summary>
        /// Heals this unit
        /// </summary>
        /// <param name="unit"></param>
        private void Heal(Unit unit)
        {
            double newHealth = unit.Health + unit.Regeneration * unitHealingMultipler;
            unit.Health = newHealth > unit.MaxHealth ? unit.MaxHealth : newHealth;
        }

        /// <summary>
        /// Actions to perform if both units are running.
        /// </summary>
        private void HealOnDoubleRunning()
        {
            if (r.NextDouble() <= unitHealingProbability)
                Heal(First);
            if (r.NextDouble() <= unitHealingProbability)
                Heal(Second);
        }

        /// <summary>
        /// Damages unit using damage rule for running units.
        /// </summary>
        /// <param name="which"></param>
        private void DamageRunningUnit(Unit which)
        {
            double newHealth = which.Health - which.MaxHealth * runningUnitHealthMultipler;
            // Thus unit will be healed if it's health is lower than 2
            // I suppose this is the desired behavior
            which.Health = newHealth > minRunningUnitHealth ? newHealth : minRunningUnitHealth;
        }

        /// <summary>
        /// Actions to perform if at least one of the unit's is in attack state.
        /// </summary>
        private void OnAttackState()
        {
            void ApplyAttackDamage(Unit unit) {
                double newHealth = unit.Health - unit.Damage * unit.Strength * unitStrengthDamageMultipler;
                unit.Health = newHealth < 0 ? 0 : newHealth;
            }
            if (First.AttackPoints >= Second.AttackPoints)
                ApplyAttackDamage(Second);
            if (First.AttackPoints <= Second.AttackPoints)
                ApplyAttackDamage(First);
        }


        /// <summary>
        /// Returns the status string of tis game.
        /// </summary>
        public string GetStatusString()
        {
            var builder = new System.Text.StringBuilder(8);
            if(First.State == Second.State)
            {
                builder.Append(First.Name).Append(" and ").Append(Second.Name).Append(" are both in ");
                builder.Append(Enum.GetName(typeof(PlayingUnit.UnitState), First.State).ToLower());
                builder.Append(" state.");
            }
            else
            {
                builder.Append(First.Name).Append(" is in ");
                builder.Append(Enum.GetName(typeof(PlayingUnit.UnitState), First.State).ToLower());
                builder.Append(" state; ");
                builder.Append(Second.Name).Append(" is in ");
                builder.Append(Enum.GetName(typeof(PlayingUnit.UnitState), Second.State).ToLower());
                builder.Append(" state.");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Performs an iteration of the game.
        /// </summary>
        public void Iteration()
        {
            switch (First.State)
            {
                case PlayingUnit.UnitState.Running:
                    switch (Second.State)
                    {
                        case PlayingUnit.UnitState.Running:
                            HealOnDoubleRunning();
                            break;
                        case PlayingUnit.UnitState.Attacking:
                            DamageRunningUnit(First);
                            OnAttackState();
                            break;
                        case PlayingUnit.UnitState.Defence:
                            DamageRunningUnit(First);
                            break;
                    }
                    break;
                case PlayingUnit.UnitState.Defence:
                    switch (Second.State)
                    {
                        case PlayingUnit.UnitState.Running:
                            DamageRunningUnit(Second);
                            break;
                        case PlayingUnit.UnitState.Attacking:
                            OnAttackState();
                            break;
                        case PlayingUnit.UnitState.Defence:
                            break;
                    }
                    break;
                case PlayingUnit.UnitState.Attacking:
                    switch (Second.State)
                    {
                        case PlayingUnit.UnitState.Running:
                            DamageRunningUnit(Second);
                            OnAttackState();
                            break;
                        case PlayingUnit.UnitState.Attacking:
                            OnAttackState();
                            break;
                        case PlayingUnit.UnitState.Defence:
                            OnAttackState();
                            break;
                    }
                    break;
            }

            if (First.Health <= 0 || Second.Health <= 0)
                OnGameFinished?.Invoke(this);

        }

        /// <summary>
        /// Creates an instance of this class using PlayingUnit-s provided.
        /// </summary>
        public GameEnvironment (PlayingUnit first, PlayingUnit second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Creates an instance of this class using Unit-s provided.
        /// </summary>
        public GameEnvironment (Unit first, Unit second) : this(new PlayingUnit(first), new PlayingUnit(second))
        {

        }

        public GameEnvironment () : this(new PlayingUnit(), new PlayingUnit())
        {

        }

    }
}
