using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace HW4
{
    /// <summary>
    /// Reader class for CSV files.
    /// </summary>
    public class CSVReader : IDisposable
    {
        public enum ParsingState
        {
            lineEndReached = 1,
            atLine = 2,
            cellExpected = 4,
            lineExpected = 8
        }



        BinaryReader reader;
        Encoding encoding;
        ParsingState state;

        bool exceptionOnEmptyCell;
        bool exceptionOnUnclosedQuotation;

        /// <summary>
        /// Indicates whether EOF is reached using stream position.
        /// </summary>
        public bool EOF => reader.BaseStream.Length == reader.BaseStream.Position;

        byte quoteByteCount;
        byte lastCharByteCount;
        char cellSpr = ';';

        /// <summary>
        /// Returns parsing state.
        /// </summary>
        public ParsingState State => state;

        /// <summary>
        /// Indicates whether to through exception if quotation is unclosed.
        /// </summary>
        public bool ExceptionOnUnclosedQuotation
        {
            get => exceptionOnUnclosedQuotation;
            set => exceptionOnUnclosedQuotation = value;
        }

        /// <summary>
        /// Indicates whether to through exception if cell is empty.
        /// </summary>
        public bool ExceptionOnEmptyCell
        {
            get => exceptionOnEmptyCell;
            set => exceptionOnEmptyCell = value;
        }

        /// <summary>
        /// Reverts stream position one character back.
        /// </summary>
        void Revert()
        {
            reader.BaseStream.Position -= lastCharByteCount;
        }

        /// <summary>
        /// Reads next character and adjusts stream position respectively.
        /// </summary>
        /// <returns>Next character.</returns>
        char ReadChar()
        {
            long currentPosition = reader.BaseStream.Position;
            char result = '\0';
            try
            {
                result = reader.ReadChar();
            }
            catch(EndOfStreamException eofException)
            {
                throw new EndOfStreamException("Unexpected EOF while trying to read the char.", eofException);
            }
            lastCharByteCount = (byte)(reader.BaseStream.Position - currentPosition);
            return result;
        }

        /// <summary>
        /// Moves current position to that of the ending quote and returns it.
        /// </summary>
        /// <returns>Position of the ending quote, -1 if the quote is not found.</returns>
        long MoveToEndingQuote()
        {
            bool quoteFound = false;
            while (!EOF)
            {
                bool isQuote = ReadChar() == '"';
                if (isQuote)
                {
                    quoteFound ^= true;
                }
                else
                {
                    if (quoteFound)
                    {
                        Revert();
                        reader.BaseStream.Position -= quoteByteCount;
                        return reader.BaseStream.Position;
                    }
                }
            }

            if (quoteFound)
            {
                Revert();
                return reader.BaseStream.Position;
            }
            else
                return -1; // quote not found
        }

        /// <summary>
        /// Moves current position to the start of the next cell body.
        /// </summary>
        /// <returns>Body start position, -1 if no body found.</returns>
        long MoveToCellBodyStart()
        {
            char current = '\0';

            while (!EOF)
            {
                current = ReadChar();
                if (current != ' ' && current != '\t')
                    break;
            }

            if (!EOF || current != '\0')
                Revert();
            if (current != '\0' && current != ' ' && current != '\t' && current != '\n' && current != '\r')
                return reader.BaseStream.Position;
            else
                return -1;
        }

        /// <summary>
        /// Moves current position to that of cell end.
        /// </summary>
        /// <param name="bodyEnd">Link to write body end position to.</param>
        /// <returns>Cell end position.</returns>
        long MoveToCellEnd(out long bodyEnd)
        {
            char current = '\0';
            bool firstTabOrSpace = true;
            bodyEnd = reader.BaseStream.Position;
            while (!EOF)
            {
                current = ReadChar();
                if (current == cellSpr || current == '\n' || current == '\r')
                    break;
                else if (current == '\t' || current == ' ')
                {
                    if (firstTabOrSpace)
                    {
                        bodyEnd = reader.BaseStream.Position - lastCharByteCount;
                        firstTabOrSpace = false;
                    }
                }
                else
                    firstTabOrSpace = true;
            }

            if (firstTabOrSpace)
                if(EOF && current != cellSpr && current != '\n' && current != '\r')
                    bodyEnd = reader.BaseStream.Position;
                else
                    bodyEnd = reader.BaseStream.Position - lastCharByteCount;

            if (current == cellSpr)
                state = ParsingState.cellExpected;
            else
            {
                state = ParsingState.lineEndReached;
                if (!EOF && current == '\r' && ReadChar() != '\n')
                    Revert();
                if (!EOF)
                    state |= ParsingState.lineExpected;
            }
            return reader.BaseStream.Position;
        }

        /// <summary>
        /// Reads string from the stream in between from and to positions.
        /// </summary>
        /// <param name="from">First byte of the starting symbol.</param>
        /// <param name="to">Last byte of the ending symbol.</param>
        /// <returns>String from the stream in between from and to positions.</returns>
        string ReadString(long from, long to)
        {
            reader.BaseStream.Position = from;
            // BinaryReader does not seem to support reading byte buffers of length larger than 32 bit
            byte[] buffer = new byte[to - from];
            reader.Read(buffer, 0, (int)(to - from));
            return encoding.GetString(buffer);
        }

        /// <summary>
        /// Reads quoted cell body from the current position.
        /// </summary>
        /// <returns>Text of quoted cell body.</returns>
        string ReadQuotedCellBody()
        {
            long qStart = reader.BaseStream.Position;
            long qEnd = MoveToEndingQuote();
            if (qEnd == -1)
                if (ExceptionOnUnclosedQuotation)
                    throw new Exception("Closing quotation mark expected");
                else
                    qEnd = reader.BaseStream.Position;
            if (qStart == qEnd)
                return "";
            else
                return ReadString(qStart, qEnd);
        }

        /// <summary>
        /// Reads unquoted cell body from its start assuming start is the current position.
        /// </summary>
        /// <returns>Cell body text.</returns>
        string ReadUnquotedCellFromBodyStart()
        {
            long bStart = reader.BaseStream.Position;
            long bEnd;
            long cEnd = MoveToCellEnd(out bEnd);
            string text;

            if (bEnd == bStart)
                return "";
            else
            {
                text = ReadString(bStart, bEnd);
                reader.BaseStream.Position = cEnd;
                return text;
            }

        }

        /// <summary>
        /// Reads cell from the current position.
        /// </summary>
        /// <returns>Cell text.</returns>
        public string ReadCell()
        {
            string text;
            if (!EOF && state.HasFlag(ParsingState.lineExpected))
                state = ParsingState.atLine;
            if (MoveToCellBodyStart() == -1)
            {
                MoveToCellEnd(out _);
                if (ExceptionOnEmptyCell)
                    throw new Exception("Cell body expected");
                else
                    text = "";
            }
            else
            {
                char startingSymbol = ReadChar();
                if (startingSymbol == '"')
                {
                    text = ReadQuotedCellBody();
                    MoveToCellEnd(out _);
                }
                else
                {
                    Revert();
                    text = ReadUnquotedCellFromBodyStart();
                }
            }

            return text;
        }

        /// <summary>
        /// Reads row from the current position.
        /// </summary>
        /// <returns>List of cells' text</returns>
        public List<string> ReadRow()
        {
            List<string> cells = new List<string>();
            if (state.HasFlag(ParsingState.lineExpected) || state.HasFlag(ParsingState.atLine))
            {
                do
                    cells.Add(ReadCell());
                while (!state.HasFlag(ParsingState.lineEndReached));
            }
            return cells;
        }

        /// <summary>
        /// Reads rows to the stream end from the current position.
        /// </summary>
        /// <returns>List of rows.</returns>
        public List<List<string>> ReadToEnd()
        {
            List<List<string>> rows = new List<List<string>>();
            while(!EOF)
            {
                rows.Add(ReadRow());
            }
            return rows;
        }

        /// <summary>
        /// Dispose this object and the stream its based on.
        /// </summary>
        public void Dispose()
        {
            reader.Close();
            reader.Dispose();
        }

        /// <summary>
        /// Creates an instance of CSV reader using stream, encoding, and cellSeparator provided
        /// </summary>
        /// <param name="on">Stream to create reader on. It has to support reading and seeking.</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <param name="cellSeparator">Cell separator symbol to use.</param>
        public CSVReader(Stream on, Encoding encoding, char cellSeparator)
        {
            if ("\t \r\n\"".IndexOf(cellSeparator) > -1 || cellSeparator == '\0')
                throw new ArgumentException("Cell separator should not be \\r, \\n, \\0, \\t, \" or space");
            if (!on.CanRead)
                throw new Exception("Base stream does not support reading.");
            if (!on.CanSeek)
                throw new Exception("Base stream does not support seeking.");
            cellSpr = cellSeparator;
            reader = new BinaryReader(on, encoding);
            this.encoding = encoding;
            quoteByteCount = (byte)encoding.GetByteCount(new char[] {'"'});
            state = ParsingState.atLine;
        }
    }
}
