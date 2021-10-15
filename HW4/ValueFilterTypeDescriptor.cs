using System;
using System.ComponentModel;

namespace HW4
{
    /// <summary>
    /// TypeDescriptor for ValueFilter\<T> class
    /// </summary>
    class ValueFilterTypeDescriptor : CustomTypeDescriptor
    {
        private Type valueFilterType;

        /// <summary>
        /// Type of ValueFilter\<T>
        /// </summary>
        public Type ValueFilterType => valueFilterType;

        /// <summary>
        /// Creates an instance of ValueFilterTypeDescriptor.
        /// </summary>
        /// <param name="valueFilterType">Variation of ValueFilter\<T> to create descriptor for.</param>
        public ValueFilterTypeDescriptor(Type valueFilterType)
        {
            this.valueFilterType = valueFilterType;
        }

        /// <summary>
        /// Returns TypeConverter for ValueFilterType
        /// </summary>
        public override TypeConverter GetConverter()
        {
            Type valueType = valueFilterType.GenericTypeArguments[0];
            Type converterType = typeof(ValueFilterConverter<>).MakeGenericType(valueType);
            return (TypeConverter)Activator.CreateInstance(converterType);
        }

    }
}
