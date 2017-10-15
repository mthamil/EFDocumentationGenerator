using System.Collections.Generic;
using System.Linq;
using DocumentationGenerator;
using EnvDTE80;
using Moq;
using Xunit;

namespace Tests.Unit
{
	public class ErrorListAdapterTests
	{
		public ErrorListAdapterTests()
		{
			_errorList.SetupGet(el => el.ErrorItems).Returns(_errorItems.Object);
			_errorItems.SetupGet(ei => ei.Count).Returns(() => _errors.Count);
		    _errorItems.Setup(ei => ei.Item(It.IsAny<object>()))
		               .Returns((object index) => _errors[(int)index - 1]);

		    _errors = Mocks.Of<ErrorItem>()
		                   .Where((error, i) => error.Description == "Error" + (i + 1))
		                   .Take(3)
		                   .ToList();

			_underTest = new ErrorListAdapter(_errorList.Object);
		}

		[Fact]
		public void Test_Count()
		{
			// Act.
			int count = _underTest.Count;

			// Assert.
			Assert.Equal(3, count);
		}

		[Fact]
		public void Test_Indexer()
		{
			// Act.
			var error1 = _underTest[0];
			var error2 = _underTest[1];
			var error3 = _underTest[2];

			// Assert.
			Assert.Equal("Error1", error1.Description);
			Assert.Equal("Error2", error2.Description);
			Assert.Equal("Error3", error3.Description);
		}

		[Fact]
		public void Test_GetEnumerator()
		{
			// Act.
			var errorDescriptions = _underTest.Select(e => e.Description);

			// Assert.
			Assert.Equal(new[] { "Error1", "Error2", "Error3" }, errorDescriptions);
		}

		private readonly ErrorListAdapter _underTest;

		private readonly List<ErrorItem> _errors;
		private readonly Mock<ErrorItems> _errorItems = new Mock<ErrorItems> { DefaultValue = DefaultValue.Mock };
		private readonly Mock<ErrorList> _errorList = new Mock<ErrorList> { DefaultValue = DefaultValue.Mock};
	}
}