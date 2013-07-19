using System;

namespace DocumentationGenerator.Utilities
{
	/// <summary>
	/// Contains extension methods for <see cref="IServiceProvider"/>.
	/// </summary>
	internal static class ServiceProviderExtensions
	{
		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <typeparam name="TService">The type of the desired service</typeparam>
		/// <param name="serviceProvider">A service provider instance</param>
		/// <returns>The desired service</returns>
		public static TService GetService<TService>(this IServiceProvider serviceProvider) where TService : class
		{
			return (TService)serviceProvider.GetService(typeof(TService));
		}
	}
}