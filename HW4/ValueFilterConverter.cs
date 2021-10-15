using System;
using System.ComponentModel;
using System.Globalization;

namespace HW4
{
    /// <summary>
    /// TypeConverter for ValueFilter\<T> class.
    /// </summary>
    /// <typeparam name="T">Generic parameter of ValueFilter.</typeparam>
    class ValueFilterConverter<T> : TypeConverter where T : IComparable
    {
        /// <summary>
        /// Converter of ValueFilter<T> generic type parameter.
        /// </summary>
        TypeConverter bConverter = TypeDescriptor.GetConverter(typeof(T));
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.Equals(typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if(CanConvertFrom(context, value.GetType()))
            {
                string str = ((string)value).TrimStart();
                string body;
                int pId = -1;
                if (string.IsNullOrEmpty(str))
                    return new ValueFilter<T>(default(T), ValueFilter<T>.Options.Everyone);
                else if (str[0] == '~')
                    return new ValueFilter<T>(default(T), ValueFilter<T>.Options.None);

                for (int i = 1; i < 7; i++)
                {
                    if (str.StartsWith(ValueFilter<T>.OptionStrings[i]))
                        pId = i;
                }
                if (pId == -1)
                { 
                    body = str;
                    pId = (int)ValueFilter<T>.Options.Equal;
                }
                else
                    body = str.Substring(ValueFilter<T>.OptionStrings[pId].Length);

                return new ValueFilter<T>((T)bConverter.ConvertFrom(context, culture, body), (ValueFilter<T>.Options)pId);
            }
            else
                return base.ConvertFrom(context, culture, value);
        }

    }
}
