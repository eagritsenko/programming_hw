using System.Collections.Generic;
using System.Xml.Linq;

namespace HW4
{
    /// <summary>
    /// Base class for class XML serializators.
    /// </summary>
    /// <typeparam name="T">Class type to perform serialization on.</typeparam>
    abstract class XMLserializer<T> where T : class
    {
        /// <summary>
        /// Returns XElement with the givven name for a T instance.
        /// </summary>
        /// <param name="value">Value to return XElement for.</param>
        /// <param name="name">Name to create XElement with.</param>
        public abstract XElement GetXElement(T value, string name);

        /// <summary>
        /// Parses value from XElement and sets it to the value link given.
        /// </summary>
        /// <param name="element">XElement to parse.</param>
        /// <param name="value">Value link to where the parsing should be performed.</param>
        public abstract void SetXElement(XElement element, T value);

        /// <summary>
        /// Creates instance of T object.
        /// </summary>
        protected abstract T CreateInstance();

        /// <summary>
        /// Serializes T value to the writer using node name provided.
        /// </summary>
        /// <param name="writer">Writer to serialize value to.</param>
        /// <param name="value">Value to be serialized.</param>
        /// <param name="name">Name to create serialized value XElement with.</param>
        public virtual void SerializeTo(System.Xml.XmlWriter writer, T value, string name)
        {
            GetXElement(value, name).WriteTo(writer);
        }

        /// <summary>
        /// Deserializes T value from the reader provided.
        /// </summary>
        /// <param name="reader">Reader to deserialize T value from.</param>
        /// <param name="name">Name of the element to deserialize.</param>
        /// <returns>Deserialized T value.</returns>
        public virtual T DeserializeFrom(System.Xml.XmlReader reader, string name)
        {
            T result = CreateInstance();
            XElement element = XElement.Load(reader);
            while (element.Name != name && !reader.EOF)
                element = XElement.Load(reader);
            if (element.Name != name)
                throw new KeyNotFoundException("Unable to find node with the name given.");
            SetXElement(element, result);
            return result;
        }
    }
}
