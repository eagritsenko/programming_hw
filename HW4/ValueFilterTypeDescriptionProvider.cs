using System;
using System.ComponentModel;

namespace HW4
{
    /// <summary>
    /// TypeDesctiptionProvider for ValueFilter\<T> class
    /// </summary>
    class ValueFilterTypeDescriptionProvider : TypeDescriptionProvider
    {
        /// <summary>
        /// Return an instance of ValueFilterTypeDescriptor.
        /// </summary>
        /// <param name="objectType">Variation of ValueFilter\<T> to create descriptor for.</param>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new ValueFilterTypeDescriptor(objectType);
        }
    }
}
