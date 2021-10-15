using System;
using System.Collections.Generic;

namespace HW4
{
    /// <summary>
    /// Reads CSVUnitEntry instances from CSVReader provided
    /// </summary>
    class UnitEntryReader
    {
        CSVReader reader;

        /// <summary>
        /// Maps CSV collumns to the corresponding CSVUnitEntry property indecies (in declaration order)
        /// </summary>
        private int[] mapper;

        /// <summary>
        /// Max number of collumns
        /// </summary>
        private const int maxUnitEntryFieldsCount = 1024;

        /// <summary>
        /// Indicates whether EOF is reached.
        /// </summary>
        public bool EOF => reader.EOF;

        /// <summary>
        /// Reads a CSVUnitEntry from the CSVReader
        /// </summary>
        public CSVUnitEntry ReadEntry()
        {
            CSVUnitEntry unit = new CSVUnitEntry();
            int i = 0;
            if (reader.State.HasFlag(CSVReader.ParsingState.lineExpected))
            {
                do
                {
                    string cValue = reader.ReadCell();
                    if (i >= mapper.Length)
                        throw new Exception("Non-constant number of table columns. Broken CSV file.");
                    if (mapper[i] != -1)
                        unit.SetValueFromString(mapper[i], cValue);
                    i++;
                } while (!reader.State.HasFlag(CSVReader.ParsingState.lineEndReached));
            }
            if (i < mapper.Length)
                throw new Exception("Non-constant number of table columns. Broken CSV file.");
            return unit;
        }

        /// <summary>
        /// Creates an instance of this class unding CSVReader provided
        /// </summary>
        public UnitEntryReader(CSVReader reader)
        {
            this.reader = reader;
            List<string> collumnHeaders = reader.ReadRow();
            HashSet<int> foundHeaders = new HashSet<int>(CSVUnitEntry.FieldsCount);
            if (collumnHeaders.Capacity > maxUnitEntryFieldsCount)
                throw new Exception("Unsupported collumns count.");
            mapper = new int[collumnHeaders.Count];
            for (int i = 0; i < collumnHeaders.Count; i++)
            {
                //remove zero-width no-break space (Since BinaryReader won't strip it.)
                collumnHeaders[i] = collumnHeaders[i].Replace(char.ConvertFromUtf32(65279).ToString(), "");
                if (CSVUnitEntry.NameToFieldID.ContainsKey(collumnHeaders[i]))
                {
                    int valueID = CSVUnitEntry.NameToFieldID[collumnHeaders[i]];
                    if (foundHeaders.Contains(valueID))
                        throw new Exception("Unit collumn field names are required to be unique.");
                    foundHeaders.Add(valueID);
                    mapper[i] = valueID;
                }
                else
                    mapper[i] = -1;
            }
            if (foundHeaders.Count != CSVUnitEntry.FieldsCount)
                throw new Exception("Some field collumns are missing.");
        }

    }
}
