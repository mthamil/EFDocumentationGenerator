namespace DocumentationGenerator
{
	/// <summary>
	/// An interface for logging messages.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Logs a message.
		/// </summary>
		/// <param name="message">The log message format</param>
		/// <param name="arguments">Any message format arguments</param>
		void Log(string message, params object[] arguments);
	}
}