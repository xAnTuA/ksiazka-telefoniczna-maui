using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KsiazkaTelefonicznaWojtas.MVVM.Models;
using System.Windows.Input;
using System.ComponentModel;
using KsiazkaTelefonicznaWojtas.Enums;
using KsiazkaTelefonicznaWojtas.MVVM.Models.Database;

namespace KsiazkaTelefonicznaWojtas.MVVM.Views;

public partial class MainView : ContentPage, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ICommand ContactClickedCommand { get; set; }
    public ICommand ContactDeleteClickedCommand { get; set; }
    public ICommand ContactEditClickedCommand { get; set; }
    
    public ObservableCollection<_Contact> Contacts { get; set; } = new ObservableCollection<_Contact>();
    
    private SQLite Database { get; set; }
    
    public bool DescendingSearch { get; set; } = false;

    public bool MultiDeleteActive { get; set; } = false;
    public bool MultiDeleteActiveNegated => !MultiDeleteActive;
    
    public List<_Contact> MultideleteSelectedIds { get; set; } = new List<_Contact>();

    private CancellationTokenSource? _cts;
    private readonly TimeSpan _debounceDelay = TimeSpan.FromMilliseconds(200);
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
        ContactDeleteClickedCommand = new Command<object>(DeleteClicked);
        ContactEditClickedCommand = new Command<object>(EditClicked);
        SortByList.ItemsSource = Enum.GetValues(typeof(OrderBy)).Cast<OrderBy>().ToList();
        BindingContext = this;
        
    }

    private async void ChangeToCallView(Object? sender,  EventArgs args)
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

            string text = (e.NewTextValue?.ToLower().Replace("-", "")) ?? "";
            OrderBy? selectedOrderBy = SortByList.SelectedItem as OrderBy?;
            var results = Database.GetContacts(text, selectedOrderBy, DescendingSearch);

            Contacts.Clear();
            foreach (var contact in results)
            {
                Contacts.Add(contact);
            }
        }
        catch (TaskCanceledException){ }
    }
    private void ContactClicked(object parameter)
    {
        if (parameter is _Contact clickedContact)
        {
            foreach (var contact in Contacts)
            {
                contact.IsExpanded = (contact == clickedContact) && !contact.IsExpanded;
            }
        }
    }

    private async void DeleteClicked(object parameter)
    {
        
        if (parameter is _Contact clickedContact)
        {
            bool answer = await DisplayAlert("Warning", $"Do you want to delete contact to {clickedContact.FullName}", "Yes", "Cancel");
            if (answer)
            {
                Database.DeleteContact(clickedContact);
                RefreshList();
            }
        }
    }

    private async void EditClicked(object parameter)
    {
        if (parameter is _Contact clickedContact)
        {
            await Navigation.PushAsync(new ContactEditingView(clickedContact));
        }
    }

    private void RefreshList()
    {
        var results = Database.GetContacts();

        Contacts.Clear();
        foreach (var contact in results)
        {
            Contacts.Add(contact);
        }
    }
    private void OnSortDirectionChanged(object sender, ToggledEventArgs e)
    {
        RefreshFilteredList();
    }
    private void OnSortByChanged(object sender, EventArgs e)
    {
        RefreshFilteredList();
    }
    private void RefreshFilteredList()
    {
        string text = (SearchBarControl.Text.ToLower().Replace("-", "")) ?? "";
        
        OrderBy? selectedOrderBy = SortByList.SelectedItem as OrderBy?;
        var results =  Database.GetContacts(text, selectedOrderBy, DescendingSearch);

        Contacts.Clear();
        foreach (var contact in results)
        {
            Contacts.Add(contact);
        }

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshList();
    }

    private void MultiDelete(object sender, EventArgs e)
    {
        MultiDeleteActive = !MultiDeleteActive;
        OnPropertyChanged(nameof(MultiDeleteActive));
        OnPropertyChanged(nameof(MultiDeleteActiveNegated));
    }

    private async void DeleteSelected(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Warning", $"Do you want to delete {MultideleteSelectedIds.Count} contacts?", "Yes", "Cancel");
        if (answer)
        {
            foreach (_Contact contact in MultideleteSelectedIds)
            {
                Database.DeleteContact(contact);
            }
            MultideleteSelectedIds.Clear();
            RefreshList();
        }
        MultiDeleteActive = false;
        OnPropertyChanged(nameof(MultiDeleteActive));
        OnPropertyChanged(nameof(MultiDeleteActiveNegated));
    }

    private void MultideleteItemChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkbox && checkbox.BindingContext is _Contact contact)
        {
            if (e.Value)
            {
                if (!MultideleteSelectedIds.Contains(contact))  { MultideleteSelectedIds.Add(contact); }
            }
            else
            {
                if (MultideleteSelectedIds.Contains(contact)) { MultideleteSelectedIds.Remove(contact); }
            }
        }
    }
}