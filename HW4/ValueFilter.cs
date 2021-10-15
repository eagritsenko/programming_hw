using System;
using System.ComponentModel;

namespace HW4
{
    /// <summary>
    /// Class representing a simple value filter which uses comparisions to a given value for filtering.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [TypeDescriptionProvider(typeof(ValueFilterTypeDescriptionProvider))]
    class ValueFilter<T> where T : IComparable
    {

        private static readonly string[] optionStrings = { "~", "<", "==", "<=", ">", "!=", ">=", "" };

        /// <summary>
        /// Array of strings which represent corresponding filter options.
        /// </summary>
        public static string[] OptionStrings => optionStrings;

        /// <summary>
        /// Enum of comparision types which must be true for the value tested.
        /// </summary>
        public enum Options
        {
            None = 0,
            LowerThan = 1,
            Equal = 2,
            GreaterThan = 4,
            Everyone = 7
        }

        /// <summary>
        /// Value to compare tested values against.
        /// </summary>
        public T inner;

        /// <summary>
        /// Creates an instance of ValueFilter using value to compare others against and filter options provided.
        /// </summary>
        /// <param name="inner">Value to compare against.</param>
        /// <param name="filterOptions">Comparision types which must be true for the value tested.</param>
        public ValueFilter(T inner, Options filterOptions)
        {
            this.inner = inner;
            FilterOptions = filterOptions;
        }

        /// <summary>
        /// Creates an instance of filter throgh which any value will pass.
        /// </summary>
        public ValueFilter() : this(default(T), Options.Everyone)
        {

        }

        /// <summary>
        /// Comparision types which must be true for the value tested.
        /// </summary>
        public Options FilterOptions { get; set; }

        /// <summary>
        /// Checks whether value satisfies the restrictions of this filter.
        /// </summary>
        public bool Satisfies(T value)
        {
            if (FilterOptions == Options.Everyone)
                return true;
            else if (FilterOptions == Options.None)
                return false;
            else
                return ((byte)FilterOptions & (1 << (value.CompareTo(inner) + 1))) > 0;
        }

        public override string ToString()
        {
            int id = (int)FilterOptions;
            if (FilterOptions == Options.None || FilterOptions == Options.Everyone)
                return optionStrings[id];
            else
                return optionStrings[id] + inner.ToString();
        }

    }
}
