//  Entity Designer Documentation Generator
//  Copyright 2017 Matthew Hamilton - matthamilton@live.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Runtime.Serialization;

namespace DocumentationGenerator.ConnectionStrings
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