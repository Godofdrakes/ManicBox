using ManicBox.Services.Implementation;
using ManicBox.Services.Test.Extensions;
using ManicBox.Services.ViewModel;

namespace ManicBox.Services.Test;

[TestClass]
public class WindowMatchServiceTests
{
	[TestMethod]
	public void HasMatch()
	{
		var service = new WindowMatchService();

		var windowMock = new WindowHandleViewModel()
		{
			ProcessName = "ManicBox.Mock",
			WindowTitle = "ManicBox.Mock",
		};

		var windowWPF = new WindowHandleViewModel()
		{
			ProcessName = "ManicBox.WPF",
			WindowTitle = "ManicBox.WPF",
		};

		var windowPreview = new WindowHandleViewModel()
		{
			ProcessName = "ManicBox.Preview",
			WindowTitle = "ManicBox.Preview",
		};

		service.HasMatch( windowMock ).StartsWith( true );
		service.HasMatch( windowWPF ).StartsWith( false );
		service.HasMatch( windowPreview ).StartsWith( false );
	}
}