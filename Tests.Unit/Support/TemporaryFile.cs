using System;
using System.IO;

namespace Tests.Unit.Support
{
	/// <summary>
	/// Class that manages a temporary file and aids in clean up
	/// after its use.
	/// </summary>
	public class TemporaryFile : IDisposable
	{
		/// <summary>
		/// Initializes a new <see cref="TemporaryFile"/>.
		/// </summary>
		public TemporaryFile()
		{
			File = new FileInfo(Path.GetTempFileName());
		}

		/// <summary>
		/// Initializes a new <see cref="TemporaryFile"/>.
		/// </summary>
		/// <param name="content">The file content</param>
		public TemporaryFile(string content)
			: this()
		{
			using (var writer = File.CreateText())
				writer.Write(content);
		}

		/// <summary>
		/// Creates an empty temporary file for the file path represented by this <see cref="TemporaryFile"/>.
		/// </summary>
		/// <remarks>This object is returned to enable a more fluent syntax.</remarks>
		public TemporaryFile Touch()
		{
			File.Create().Close();
			return this;
		}

		/// <summary>
		/// The actual temporary file.
		/// </summary>
		public FileInfo File { get; private set; }

		#region IDisposable Implementation

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			Dispose(true);

			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Use C# destructor syntax for finalization code.
		/// This destructor will run only if the Dispose method
		/// does not get called.
		/// It gives your base class the opportunity to finalize.
		/// Do not provide destructors in types derived from this class.
		/// </summary>
		~TemporaryFile()
		{
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			Dispose(false);
		}

		/// <summary>
		/// Implements the actual disposal logic.  Subclasses should
		/// override this method to clean up resources.
		/// </summary>
		/// <param name="disposing">Whether the class is disposing from the Dispose() method</param>
		private void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (File.Exists)
					File.Delete();

				_isDisposed = true;
			}
		}

		private bool _isDisposed;

		#endregion IDisposable Implementation
	}
}