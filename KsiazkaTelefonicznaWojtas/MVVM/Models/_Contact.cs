using System.ComponentModel;

namespace KsiazkaTelefonicznaWojtas.MVVM.Models;

public class _Contact : INotifyPropertyChanged
{
    public int? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public short? AreaCode { get; set; }
    public int Number { get; set; }
    public string DisplayNumber => $"+{AreaCode} {FormatNumber(System.Convert.ToString(Number))}";
    public string FullName => $"{FirstName} {LastName}";
    
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
    
    private string FormatNumber(string number)
    {
        return $"{number.Substring(0, 3)}-{number.Substring(3, 3)}-{number.Substring(6, 3)}";
    }
}