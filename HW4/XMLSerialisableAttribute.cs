using System;

namespace HW4
{
    /// <summary>
    /// Attribute indicating that a type or member should or can be serialized.
    /// </summary>
    class XMLSerializableAttribute : Attribute
    {
        public object Serializer { get; set; } = null;

        public XMLSerializableAttribute()
        {

        }

        /// <summary>
        /// Creates this attribute using the serializer type provided.
        /// </summary>
        /// <param name="serializer">Type of serializer to create this attribute with.</param>
        public XMLSerializableAttribute(Type serializer)
        {
            Serializer = Activator.CreateInstance(serializer);
        }
    }
}
