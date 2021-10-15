using System;
using System.Linq;

namespace HW4
{
    public class Unit
     {
        protected const int defaultHealthMultipler = 29;
        protected const int defaultIntPropertyMinValue = 0;
        protected const int defaultIntPropertyMaxValue = 1 << 15;
        protected const int defaultDoublePropertyMinValue = 0;
        protected const int defaultDoublePropertyMaxValue = 1 << 24;
        protected const int defaultTypeMinValue = 0;
        protected const int defaultTypeMaxValue = 3;

        protected string name;
        protected int type, strength, agilty, inteligence, moveSpeed;
        protected double armor, damage, regeneration, health, maxHealth;


        /// <summary>
        /// Checks whether a double value is nan or infinity
        protected bool IsNanOrInfinity(double d)
        {
            return double.IsNaN(d) || double.IsInfinity(d);
        }

        /*
        *   Note: The Validate* methods bellow were created as an attempt to separate property validation from properties.
        *   Which appeared to be costly (in terms of code-writing time).
        */

        /// <summary>
        /// Validates Name property. Throws exception on invalid value.
        /// </summary>
        protected void ValidateNameProperty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name value appers to be null, empty or white space.");
        }

        /// <summary>
        /// Validates a int property using value ranges. Throws exception on invalid value.
        /// </summary>
        /// <param name="propertyName">Name of property to display in exception message.</param>
        /// <param name="from">Lower value bound.</param>
        /// <param name="to">Upper value bound. Non-inclusive.</param>
        /// <param name="value">Value to be validated.</param>
        public void ValidateIntPropertyRange(string propertyName, int from, int to, int value)
        {
            if(value < from || value >= to )
                throw new ArgumentException($"{propertyName} value appears to be not in range [{from}, {to}).");
        }

        /// <summary>
        /// Validates common int property. Throws exception on invalid value.
        /// </summary>
        /// <param name="propertyName">Name of property to display in exception message.</param>
        /// <param name="value">Value to be validated.</param>
        public void ValidateBaseIntProperty(string propertyName, int value)
        {
            ValidateIntPropertyRange(propertyName, defaultIntPropertyMinValue, defaultIntPropertyMaxValue, value);
        }

        /// <summary>
        /// Validates common double property. Throws exception on invalid value.
        /// </summary>
        /// <param name="propertyName">Name of property to display in exception message.</param>
        /// <param name="value">Value to be validated.</param>
        public void ValidateBaseDoubleProperty(string propertyName, double value)
        {
            if (!(value >= defaultDoublePropertyMinValue && value < defaultDoublePropertyMaxValue))
                throw new ArgumentException
                    ($"{propertyName} value appears to be not in range [{defaultDoublePropertyMinValue}, {defaultDoublePropertyMaxValue}).");
        }

        /// <summary>
        /// Validates Type property. Throws exception on invalid value.
        /// </summary>
        public void ValidateTypeProperty(int value)
        {
            ValidateIntPropertyRange(nameof(Type), defaultTypeMinValue, defaultTypeMaxValue, value);
        }

        /// <summary>
        /// Validates Health property. Throws exception on invalid value.
        /// </summary>
        public void ValidateHealthProperty(double value)
        {
            ValidateBaseDoubleProperty(nameof(Health), value);
            if (value > MaxHealth)
                throw new ArgumentException("Health value can not be greater than MaxHealth");
        }

        /// <summary>
        /// Validates MaxHealth property. Throws exception on invalid value.
        /// </summary>
        public void ValidateMaxHealthProperty(double value)
        {
            ValidateBaseDoubleProperty(nameof(MaxHealth), value);
            if (value < Health)
                throw new ArgumentException("MaxHealth value can not be lower than Health");
        }

        /// <summary>
        /// Represents name of this unit.
        /// </summary>
        [ValueDisplay(readOnly: true), XMLSerializable]
        public string Name
        {
            get => name;
            set
            {
                ValidateNameProperty(value);
                name = value;
            }
        }

        /// <summary>
        /// Represents type of this unit.
        /// </summary>
        [ValueDisplay, XMLSerializable]
        public int Type
        {
            get => type;
            set
            {
                ValidateTypeProperty(value);
                type = value;
            }
        }

        /// <summary>
        /// Represents strength of this unit.
        /// </summary>
        [ValueDisplay, XMLSerializable]
        public int Strength
        {
            get => strength;
            set
            {
                ValidateBaseIntProperty(nameof(Strength), value);
                strength = value;
            }
        }

        /// <summary>
        /// Represents agilty of this unit.
        /// </summary>
        [ValueDisplay, XMLSerializable]
        public int Agilty
        {
            get => agilty;
            set
            {
                ValidateBaseIntProperty(nameof(Agilty), value);
                agilty = value;
            }
        }

        /// <summary>
        /// Represents intelligence of this unit.
        /// </summary>
        [ValueDisplay, XMLSerializable]
        public int Inteligence
        {
            get => inteligence;
            set
            {
                ValidateBaseIntProperty(nameof(Inteligence), value);
                inteligence = value;
            }
        }

        /// <summary>
        /// Represents move speed of this unit.
        /// </summary>
        [ValueDisplay(headerText: "Move speed"), XMLSerializable]
        public int MoveSpeed
        {
            get => moveSpeed;
            set
            {
                ValidateBaseIntProperty(nameof(MoveSpeed), value);
                moveSpeed = value;
            }
        }

        /// <summary>
        /// Represents armor of this unit.
        /// </summary>
        [ValueDisplay, XMLSerializable]
        public double Armor
        {
            get => armor;
            set
            {
                ValidateBaseDoubleProperty(nameof(Armor), value);
                armor = value;
            }
        }

        /// <summary>
        /// Represents damage of this unit.
        /// </summary>
        [ValueDisplay, XMLSerializable]
        public double Damage
        {
            get => damage;
            set
            {
                ValidateBaseDoubleProperty(nameof(Damage), value);
                damage = value;
            }
        }

        /// <summary>
        /// Represents regeneration of this unit.
        /// </summary>
        [ValueDisplay, XMLSerializable]
        public double Regeneration
        {
            get => regeneration;
            set
            {
                ValidateBaseDoubleProperty(nameof(Regeneration), value);
                regeneration = value;
            }
        }

        /// <summary>
        /// Represents health of this unit.
        /// </summary>
        [ValueDisplay, XMLSerializable]
        public double Health
        {
            get => health;
            set
            {
                ValidateHealthProperty(value);
                health = value;
            }
        }

        /// <summary>
        /// Represents max health of this unit.
        /// </summary>
        [ValueDisplay(headerText: "Max health"), XMLSerializable]
        public double MaxHealth
        {
            get => maxHealth;
            set
            {
                ValidateMaxHealthProperty(value);
                maxHealth = value;
            }
        }

        /// <summary>
        /// Returns the array of properties with ValueDisplay attribute define.
        /// </summary>
        public static System.Reflection.PropertyInfo[] DisplayedProperties { get; }

        /// <summary>
        /// Array of this class field info classes.
        /// </summary>
        private static System.Reflection.FieldInfo[] fields;

        static Unit()
        {
            DisplayedProperties = typeof(Unit)
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(ValueDisplayAttribute)))
                .ToArray();
            fields = typeof(Unit).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }

        /// <summary>
        /// Copies this unit's data to another one.
        /// </summary>
        /// <param name="unit"></param>
        public void CopyTo(Unit unit)
        {
            foreach (var field in fields)
                field.SetValue(unit, field.GetValue(this));
        }

        public Unit()
        {
            name = "";
            maxHealth = defaultDoublePropertyMaxValue - 1;
        }

        /// <summary>
        /// Creates this Unit copying data from CSVUnitEntry provided.
        /// </summary>
        public Unit (CSVUnitEntry csvEntry) : this()
        {
            Name = csvEntry.name;
            Type = csvEntry.type;
            Strength = csvEntry.baseStr;
            Agilty = csvEntry.baseAgi;
            Inteligence = csvEntry.baseInt;
            MoveSpeed = csvEntry.moveSpeed;
            Armor = csvEntry.baseArmor;
            Damage = csvEntry.minDmg;
            Regeneration = csvEntry.regeneration;
            Health = csvEntry.baseStr * defaultHealthMultipler;
            MaxHealth = csvEntry.baseStr * defaultHealthMultipler;
        }


    }
}
