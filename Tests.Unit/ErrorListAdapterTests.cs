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
			errorList.SetupGet(el => el.ErrorItems).Returns(errorItems.Object);
			errorItems.SetupGet(ei => ei.Count).Returns(() => errors.Count);
			errorItems.Setup(ei => ei.Item(It.IsAny<object>())).Returns((object index) => errors[(int)index - 1]);

			for (int i = 1; i < 4; i++)
			{
				var error = new Mock<ErrorItem>();
				error.SetupGet(e => e.Description).Returns("Error" + i);
				errors.Add(error.Object);
			}

			errorListAdapter = new ErrorListAdapter(errorList.Object);
		}

		[Fact]
		public void Test_Count()
		{
			// Act.
			int count = errorListAdapter.Count;

			// Assert.
			Assert.Equal(3, count);
		}

		[Fact]
		public void Test_Indexer()
		{
			// Act.
			var error1 = errorListAdapter[0];
			var error2 = errorListAdapter[1];
			var error3 = errorListAdapter[2];

			// Assert.
			Assert.Equal("Error1", error1.Description);
			Assert.Equal("Error2", error2.Description);
			Assert.Equal("Error3", error3.Description);
		}

		[Fact]
		public void Test_GetEnumerator()
		{
			// Act.
			var errorDescriptions = errorListAdapter.Select(e => e.Description);

			// Assert.
			Assert.Equal(new[] { "Error1", "Error2", "Error3" }, errorDescriptions.ToArray());
		}

		private readonly ErrorListAdapter errorListAdapter;

		private readonly List<ErrorItem> errors = new List<ErrorItem>();
		private readonly Mock<ErrorItems> errorItems = new Mock<ErrorItems> { DefaultValue = DefaultValue.Mock };
		private readonly Mock<ErrorList> errorList = new Mock<ErrorList> { DefaultValue = DefaultValue.Mock};
	}
}