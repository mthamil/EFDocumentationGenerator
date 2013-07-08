using System;
using System.Runtime.Serialization;

namespace DocumentationGenerator
{
	/// <summary>
	/// Exception thrown when a connection string cannot be found.
	/// </summary>
	[Serializable]
	public class ConnectionStringLocationException : Exception
	{
		/// <summary>
		/// Initializes a new <see cref="ConnectionStringLocationException"/>.
		/// </summary>
		protected ConnectionStringLocationException()
		{
		}

		/// <summary>
		/// Initializes a new <see cref="ConnectionStringLocationException"/>.
		/// </summary>
		/// <param name="message">A message descrbing the failure</param>
		public ConnectionStringLocationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new <see cref="ConnectionStringLocationException"/>.
		/// </summary>
		/// <param name="message">A message descrbing the failure</param>
		/// <param name="inner">An exception that caused this one</param>
		public ConnectionStringLocationException(string message, Exception inner) 
			: base(message, inner)
		{
		}

		/// <see cref="Exception(SerializationInfo,StreamingContext)"/>
		protected ConnectionStringLocationException(SerializationInfo info, StreamingContext context) 
			: base(info, context)
		{
		}
	}
}