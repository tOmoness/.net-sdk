// -----------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Text;

namespace Nokia.Music.Internal
{
    internal static class StreamExtensions
    {
        private const int BufferLength = 4096;
 
        public static string AsString(this Stream stream)
        {
            return AsString(stream, Encoding.UTF8);
        }
 
        /// <summary>
        ///   Converts the stream to a string. The stream will be disposed by this operation.
        /// </summary>
        /// <param name = "stream">The stream from which to read a string</param>
        /// <param name = "encoding">The encoding of the stream</param>
        /// <returns>The string held within the stream</returns>
        public static string AsString(this Stream stream, Encoding encoding)
        {
            if (stream != null)
            {
                var sb = new StringBuilder();
                var buffer = new char[BufferLength];
 
                using (var reader = new StreamReader(stream, encoding))
                {
                    // ReadToEnd() calls the Length property, which throws NotSupportedException internally for web streams,
                    // so we will use a buffer-based approach here...
                    int readCount;
                    while (0 != (readCount = reader.Read(buffer, 0, BufferLength)))
                    {
                        sb.Append(buffer, 0, readCount);
                    }
                }
 
                return sb.ToString();
            }

            return null;
        }
    }
}
