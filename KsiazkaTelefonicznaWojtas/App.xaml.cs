using KsiazkaTelefonicznaWojtas.MVVM.Views;

namespace KsiazkaTelefonicznaWojtas;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new MainView());
    }
}