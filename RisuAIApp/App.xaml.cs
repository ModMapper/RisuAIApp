namespace RisuAIApp;

using EmbedIO;
using EmbedIO.WebApi;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        MainPage = new MainPage();
	}
}
