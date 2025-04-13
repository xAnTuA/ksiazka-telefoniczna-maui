namespace KsiazkaTelefonicznaWojtas.MVVM.ViewsModels;

public class MainViewModel
{
    public List<Contact>  Contacts { get; set; }

    public MainViewModel(List<Contact>  contacts)
    {
        Contacts = contacts;
    }
}