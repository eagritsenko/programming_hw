using System;

namespace HW4
{
    /// <summary>
    /// Attribute which annotates the property value should be displayed and a way to do so.
    /// </summary>
    class ValueDisplayAttribute : Attribute
    {
        /// <summary>
        /// Indicates whether the property value should be displayed as read only.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Header text for the property value to be displayed
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// Initializes this attribute.
        /// </summary>
        public ValueDisplayAttribute() { }

        /// <summary>
        /// Initializes this attribute.
        /// </summary>
        /// <param name="readOnly">Indicates whether the property value should be displayed as read only.</param>
        public ValueDisplayAttribute(bool readOnly) => ReadOnly = readOnly;
        /// <summary>
        /// Initializes this attribute.
        /// </summary>
        /// <param name="headerText">Header text for the property value to be displayed.</param>
        public ValueDisplayAttribute(string headerText) => HeaderText = headerText;

        /// <summary>
        /// Initializes this attribute.
        /// </summary>
        /// <param name="readOnly">Indicates whether the property value should be displayed as read only.</param>
        /// <param name="headerText">Header text for the property value to be displayed.</param>
        public ValueDisplayAttribute(bool readOnly, string headerText)
        {
            ReadOnly = readOnly;
            HeaderText = headerText;
        }
    }
}
