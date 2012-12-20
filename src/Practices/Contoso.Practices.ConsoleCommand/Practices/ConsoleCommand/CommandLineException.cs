using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Contoso.Practices.ConsoleCommand
{
    /// <summary>
    /// CommandLineException
    /// </summary>
    [Serializable]
    public class CommandLineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineException"/> class.
        /// </summary>
        public CommandLineException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CommandLineException(string message)
            : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected CommandLineException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public CommandLineException(string message, Exception exception)
            : base(message, exception) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public CommandLineException(string format, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, format, args)) { }
    }
}

