using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KsiazkaTelefonicznaWojtas.MVVM.Models;
using System.Windows.Input;
using System.ComponentModel;
using KsiazkaTelefonicznaWojtas.MVVM.Models.Database;

namespace KsiazkaTelefonicznaWojtas.MVVM.Views;

public partial class MainView : ContentPage, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ICommand ContactClickedCommand { get; set; }
    public ObservableCollection<_Contact> Contacts { get; set; } = new ObservableCollection<_Contact>();
    private SQLite Database { get; set; }
    
    private CancellationTokenSource? _cts;
    private readonly TimeSpan _debounceDelay = TimeSpan.FromMilliseconds(100);
    public MainView()
    {
        Database = SQLite.GetInstance();
        Contacts.Clear();
        foreach (var contact in Database.GetContacts())
        {
            Contacts.Add(contact);
        }
        InitializeComponent();
        ContactClickedCommand = new Command<object>(ContactClicked);
        BindingContext = this;
    }
    public async void ChangeToContactView()
    {
        await Navigation.PushAsync(new ContactView());
    }
    public async void ChangeToCallView(Object? sender,  EventArgs args)
    {
        await Navigation.PushAsync(new ContactAddingView());
    }

    private async void OnSearchBArTextChanged(Object sender, TextChangedEventArgs e)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        try
        {
            await Task.Delay(_debounceDelay, token);
            if (token.IsCancellationRequested) return;

            string text = e.NewTextValue?.ToLower() ?? "";

            var results = Database.GetContacts(text);

            Contacts.Clear();
            foreach (var contact in results)
            {
                Contacts.Add(contact);
            }
        }
        catch (TaskCanceledException){ }
    }
    public async void ContactClicked(object parameter)
    {
        if (parameter is _Contact clickedContact)
        {
            foreach (var contact in Contacts)
            {
                contact.IsExpanded = (contact == clickedContact) ? !contact.IsExpanded : false;
            }
        }
        // await Navigation.PushAsync(new ContactView());
    }
}