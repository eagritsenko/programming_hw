using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace HW4
{
    /// <summary>
    /// Class representing unit entry as its seen in the CSV file.
    /// </summary>
    public class CSVUnitEntry
    {
        // those fields represent the corresponding entries in the csv file
        // they are assigned values with reflections which can not be detected by InteliSence
        // therefore CS0649 (field in never assigned to warning) is wrong and should be ignored

        public string name;
        public int type;
        public int baseStr;
        public int baseAgi;
        public int baseInt;
        public int moveSpeed;
        public double baseArmor;
        public int minDmg;
        public double regeneration;

        private static Action<object, object>[] setValueActions;
        private static int fieldsCount;
        private static Dictionary<string, int> nameToFieldId;

        /// <summary>
        /// Returns number of non-static fields.
        /// </summary>
        public static int FieldsCount => fieldsCount;

        /// <summary>
        /// Returns dictionary of non-static field names bound to their indecies in declaration order
        /// </summary>
        public static Dictionary<string, int> NameToFieldID => nameToFieldId;

        static CSVUnitEntry()
        {
            var fields = Array.FindAll(typeof(CSVUnitEntry).GetFields(), f => !f.IsStatic);
            var fieldTypes = fields.Select(f => f.FieldType).Distinct();
            var fTypeConverters = fieldTypes.ToDictionary(t => t, TypeDescriptor.GetConverter);
            fieldsCount = fields.Length;
            setValueActions = new Action<object, object>[fieldsCount];
            nameToFieldId = new Dictionary<string, int>(fieldsCount);
            for(int i = 0; i < fields.Length; i++)
            {
                var current = fields[i];
                if (current.FieldType == typeof(string))
                    setValueActions[i] = current.SetValue;
                else
                {
                    TypeConverter converter = fTypeConverters[current.FieldType];
                    setValueActions[i] = (_this, value) =>
                        current.SetValue(_this, converter.ConvertFromInvariantString(value as string));
                }
                nameToFieldId.Add(current.Name, i);
            }
        }

        /// <summary>
        /// Converts string value to the type of id field using invariant culture and assigns it.
        /// </summary>
        /// <param name="id">Field index in declaration order.</param>
        /// <param name="value">String value.</param>
        public void SetValueFromString(int id, object value) => setValueActions[id].Invoke(this, value);

    }


}
