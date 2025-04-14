using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KsiazkaTelefonicznaWojtas.MVVM.Models;
using KsiazkaTelefonicznaWojtas.MVVM.Models.Database;

namespace KsiazkaTelefonicznaWojtas.MVVM.Views;

public partial class ContactAddingView : ContentPage
{
    public ContactAddingView()
    {
        InitializeComponent();
    }

    public void Submit(Object sender, EventArgs e)
    {
        
        // int number = System.Convert.ToInt32(NumberEntry1.Text+NumberEntry2.Text+NumberEntry3.Text);
        _Contact x = new _Contact("Antoni","Wojtas", 1, 123231123); 
        SQLite.GetInstance().AddContacts(x);
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
            Regex rx = new Regex("^[A-Z,ŻŹĆĄĘŁÓŚ,a-z,ążźś]+$");
            if (!rx.IsMatch(entry.Text))
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
}