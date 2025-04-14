using System.ComponentModel;

namespace KsiazkaTelefonicznaWojtas.MVVM.Models;

public class _Contact : INotifyPropertyChanged
{
    public int? Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public short AreaCode { get; set; }
    public int Number { get; set; }
    public string DisplayNumber => $"+{AreaCode} {Number}";
    
    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            OnPropertyChanged(nameof(IsExpanded));
        }
    }
    public _Contact(String firstname,string lastname, short areaCode, int number, int? id = null)
    {
        Id = id;
        FirstName = firstname;
        LastName = lastname;
        AreaCode = areaCode;
        Number = number;
        IsExpanded = false;
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}