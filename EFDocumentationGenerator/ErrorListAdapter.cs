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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using EnvDTE;
using EnvDTE80;
using DocumentationGenerator.Utilities;

namespace DocumentationGenerator
{
	/// <summary>
	/// An adapter for the Visual Studio Error List.
	/// </summary>
	[Export(typeof(IReadOnlyList<ErrorItem>))]
	public class ErrorListAdapter : IReadOnlyList<ErrorItem>
	{
		/// <summary>
		/// Initializes a new <see cref="ErrorListAdapter"/>.
		/// </summary>
		/// <param name="serviceProvider">Provides access to application services</param>
		[ImportingConstructor]
		public ErrorListAdapter(IServiceProvider serviceProvider)
			: this((ErrorList)((DTE2)serviceProvider.GetService<DTE>()).Windows.Item(EnvDTEConstants.vsWindowKindErrorList).Object)
		{
		}

		/// <summary>
		/// Initializes a new <see cref="ErrorListAdapter"/>
		/// </summary>
		/// <param name="innerErrorList">A wrapped Visual Studio Error List</param>
		public ErrorListAdapter(ErrorList innerErrorList)
		{
			_innerErrorList = innerErrorList;
		}

		/// <see cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<ErrorItem> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}

		/// <see cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// The number of errors in the list.
		/// </summary>
		public int Count 
		{ 
			get { return _innerErrorList.ErrorItems.Count; } 
		}

		/// <summary>
		/// Retrieves the error at a given index.
		/// </summary>
		/// <param name="index">The index to access</param>
		/// <returns>An error list item</returns>
		public ErrorItem this[int index]
		{
			// Error list uses 1-based indices.
			get { return _innerErrorList.ErrorItems.Item(index + 1); }
		}

		private readonly ErrorList _innerErrorList;
	}
}