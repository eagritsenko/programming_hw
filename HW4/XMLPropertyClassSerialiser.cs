using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Linq;

namespace HW4
{

    /// <summary>
    /// XML serializer for classes which uses their properties to perform serialization.
    /// </summary>
    /// <typeparam name="T">Class type to perform serialization on.</typeparam>
    class XMLPropertyClassSerializer<T> : XMLserializer<T> where T : class
    {
        /// <summary>
        /// Array of functions which return XElements for corresponding property values.
        /// </summary>
        private static Func<T, XElement>[] getPropertyElements;
        /// <summary>
        /// Array of methods which parse values from XElements and set them to corresponding properties.
        /// </summary>
        private static Dictionary<string, Action<XElement, T>> setPropertyElements;
        /// <summary>
        /// Array of properties to be serialized.
        /// </summary>
        private static PropertyInfo[] serializedProperties;


        // Those are vatious base types for getters, setters, base class methods and attr/value types.
        // While their values are constant for a generic type and can be used directly,
        // The variables are declared for better write- and readability
        private static Type getXElementGenericType = typeof(Func<,,>);
        private static Type setXElementGenericType = typeof(Action<,>);
        private static Type createInstanceType = typeof(Func<>);
        private static Type getValueGenericType = typeof(Func<,>);
        private static Type setValueGenericType = typeof(Action<,>);
        private static Type valueType = typeof(T);
        private static Type attrType = typeof(XMLSerializableAttribute);

        /// <summary>
        /// Text to send as message if no converter was found for the value of serialized property.
        /// </summary>
        private const string noConverterExText =
            "Unable to get converter for a property value type which fully supports string convertion";

        /// <summary>
        /// Initializes this generic type.
        /// </summary>
        static XMLPropertyClassSerializer()
        {
            serializedProperties = valueType.GetProperties().Where(p => Attribute.IsDefined(p, attrType)).ToArray();
            getPropertyElements = new Func<T, XElement>[serializedProperties.Length];
            setPropertyElements = new Dictionary<string, Action<XElement, T>>(serializedProperties.Length);
            for (int i = 0; i < serializedProperties.Length; i++)
            {
                var property = serializedProperties[i];
                var attr = (XMLSerializableAttribute)Attribute.GetCustomAttribute(property, attrType);
                bool hasValueAttr = Attribute.IsDefined(property.PropertyType, attrType);
                XMLSerializableAttribute valueAttr = null;
                var getValue = property
                    .GetGetMethod()
                    .CreateDelegate(getValueGenericType.MakeGenericType(valueType, property.PropertyType));
                var setValue = property
                    .GetSetMethod()
                    .CreateDelegate(setValueGenericType.MakeGenericType(valueType, property.PropertyType));
                if (hasValueAttr)
                {
                    valueAttr = (XMLSerializableAttribute)Attribute
                        .GetCustomAttribute(property.PropertyType, attrType);
                }
                if (attr.Serializer != null || (hasValueAttr && valueAttr.Serializer != null))
                {
                    if (attr.Serializer == null)
                        attr = valueAttr;
                    var serializer = attr.Serializer;
                    var serializerType = serializer.GetType();
                    var getPropertyElement = serializerType
                        .GetMethod(nameof(XMLserializer<object>.GetXElement))
                        .CreateDelegate(getXElementGenericType.MakeGenericType(property.PropertyType, typeof(string), typeof(XElement)), serializer);
                    var setPropertyElement = serializerType
                        .GetMethod("SetXElement")
                        .CreateDelegate(setXElementGenericType.MakeGenericType(typeof(XElement), property.PropertyType), serializer);
                    var createInstance = serializerType
                        .GetMethod("CreateInstance", BindingFlags.NonPublic | BindingFlags.Instance)
                        .CreateDelegate(createInstanceType.MakeGenericType(property.PropertyType), serializer);

                    getPropertyElements[i] =
                        (unit) => (XElement)getPropertyElement.DynamicInvoke(getValue.DynamicInvoke(unit), property.Name);
                    setPropertyElements[property.Name] = (e, value) =>
                    {
                        object propertyValue = createInstance.DynamicInvoke();
                        setPropertyElement.DynamicInvoke(e, propertyValue);
                        setValue.DynamicInvoke(value, propertyValue);
                    };
                }
                else
                {
                    TypeConverter valueTypeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                    if (!valueTypeConverter.CanConvertFrom(typeof(string)) || !valueTypeConverter.CanConvertTo(typeof(string)))
                        throw new Exception(noConverterExText);
                    getPropertyElements[i] = (value) => {
                        XElement element = new XElement(property.Name);
                        element.SetAttributeValue("type", property.PropertyType.FullName);
                        element.SetValue(valueTypeConverter.ConvertToInvariantString(getValue.DynamicInvoke(value)));
                        return element;
                    };
                    setPropertyElements[property.Name] = (e, value) =>
                    {
                        setValue.DynamicInvoke(value, valueTypeConverter.ConvertFromInvariantString(e.Value));
                    };
                }
            }
        }

        public override XElement GetXElement(T value, string name)
        {
            XElement element = new XElement(name);
            element.SetAttributeValue("type", valueType.FullName);
            for (int i = 0; i < getPropertyElements.Length; i++)
                element.Add(getPropertyElements[i](value));
            return element;
        }

        public override void SetXElement(XElement element, T value)
        {
            var propertyNodes = element.Nodes();
            foreach (var node in propertyNodes)
            {
                if (!(node is XElement))
                    continue;
                XElement current = (XElement)node;
                if(setPropertyElements.ContainsKey(current.Name.LocalName))
                    setPropertyElements[current.Name.LocalName].Invoke(current, value);
            }
        }

        protected override T CreateInstance()
        {
            return (T)Activator.CreateInstance(valueType);
        }

    }


}
