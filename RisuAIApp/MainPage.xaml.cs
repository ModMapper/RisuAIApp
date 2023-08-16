namespace RisuAIApp;

using System.Web;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
        WebContent.Navigated += WebContent_Navigated;
        WebContent.Source = MauiProgram.Server.URL;
	}

    private void WebContent_Navigated(object sender, WebNavigatedEventArgs e) {
		WebContent.Navigated -= WebContent_Navigated;
		string url = HttpUtility.JavaScriptStringEncode(MauiProgram.Server.URL, true);
		WebContent.EvaluateJavaScriptAsync("alert(" + url + ");");
    }

    protected override bool OnBackButtonPressed() {
        if(WebContent.CanGoBack) {
			WebContent.GoBack();
			return true;
		}
		return base.OnBackButtonPressed();
    }
}

