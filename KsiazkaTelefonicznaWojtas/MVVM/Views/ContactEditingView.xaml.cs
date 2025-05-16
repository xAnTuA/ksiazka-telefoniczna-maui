using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KsiazkaTelefonicznaWojtas.MVVM.Models;
using KsiazkaTelefonicznaWojtas.MVVM.Models.Database;

namespace KsiazkaTelefonicznaWojtas.MVVM.Views;

public partial class ContactEditingView : ContentPage
{
    private _Contact Contact;
    public ContactEditingView(_Contact contact)
    {
        InitializeComponent();
        Contact = contact;
        FirstName.Text = contact.FirstName?? "";
        LastName.Text = contact.LastName ?? "";
        AreaCodeEntry.Text = System.Convert.ToString(contact.AreaCode)?? "";
        NumberEntry1.Text = contact.Number.ToString().Substring(0, 3)??"";
        NumberEntry2.Text = contact.Number.ToString().Substring(3, 3) ?? "";
        NumberEntry3.Text = contact.Number.ToString().Substring(6, 3) ?? "";
    }
    
    public async void Submit(Object sender, EventArgs e)
    {
        Regex rx = new Regex("^[A-Z,ŻŹĆĄĘŁÓŚ,a-z,ćążźśęół]+$");
        if (!rx.IsMatch(FirstName.Text ?? ""))
        {
            await DisplayAlert("Warning", $"Firstname doesnt pass standards, try again", "Ok");
            return;
        }

        if (!rx.IsMatch(LastName.Text ?? ""))
        {
            await DisplayAlert("Warning", $"Lastname doesnt pass standards, try again", "Ok");
            return;
        }
        
        Regex areacoderx = new Regex(@"^\d{0,4}$");
        if (!areacoderx.IsMatch(AreaCodeEntry.Text ?? ""))
        {
            await DisplayAlert("Warning", $"Area Code doesnt pass standards, try again", "Ok");
            return;
        }
        
        string numberstring = NumberEntry1.Text + NumberEntry2.Text + NumberEntry3.Text;
        Regex numberrx = new Regex("^[0-9]{9}$");
        if (!numberrx.IsMatch(numberstring ?? ""))
        {
            await DisplayAlert("Warning", $"Number doesnt pass standards, try again", "Ok");
            return;
        }
        string firstname = (FirstName.Text??"").ToLower();
        string formattedFirstName = char.ToUpper(firstname[0]) + firstname.Substring(1);
        string lastname = (LastName.Text??"").ToLower();
        string formattedLastName = char.ToUpper(lastname[0]) + lastname.Substring(1);
        int number = System.Convert.ToInt32(numberstring);
        short areacode = System.Convert.ToInt16(AreaCodeEntry.Text);
        _Contact updatedContact = new _Contact(formattedFirstName,formattedLastName, areacode, number, Contact.Id);

        SQLite.GetInstance().UpdateContact(updatedContact);
        await Navigation.PopAsync();
        
    }
    
    private void AutoJump_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is Entry current)
        {
            if (current.Text?.Length == current.MaxLength)
            {
                if (current == AreaCodeEntry) NumberEntry1.Focus();
                else if (current == NumberEntry1) NumberEntry2.Focus();
                else if (current == NumberEntry2) NumberEntry3.Focus();
            }
            else if (current.Text?.Length == 0)
            {
                if (current == NumberEntry1) AreaCodeEntry.Focus();
                else if (current == NumberEntry2) NumberEntry1.Focus();
                else if (current == NumberEntry3) NumberEntry2.Focus();
            }
            NumberUnfocused(sender, e);
        }
    }

    private void StringUnfocused(object sender, FocusEventArgs e)
    {
        if(sender is Entry entry)
        {
            Regex rx = new Regex("^[A-Z,ŻŹĆĄĘŁÓŚ,a-z,ćążźśęół]+$");
            if (!rx.IsMatch(entry.Text??""))
            {
                entry.TextColor = Color.FromRgb(255,0,0);
            }
            else
            {
                entry.TextColor = Color.FromRgb(0, 255, 0);
            }
        }
    }

    private void NumberUnfocused(object sender, EventArgs e)
    {
        if(sender is Entry entry)
        {
            Regex rx = new Regex("^[0-9]+$");
            if (entry.MaxLength == 3 && entry.Text?.Length < entry.MaxLength)
            {
                entry.TextColor = Color.FromRgb(255,0,0);
            }
            else if (!rx.IsMatch(entry.Text??""))
            {
                entry.TextColor = Color.FromRgb(255,0,0);
            }
            else
            {
                entry.TextColor = Color.FromRgb(0, 255, 0);
            }
        }
    }

    private void AreaCodeEntry_OnCompleted(object? sender, EventArgs e)
    {
        NumberEntry1.Focus();
    }
}