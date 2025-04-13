namespace KsiazkaTelefonicznaWojtas.MVVM.Models;

public class _Contact
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public short AreaCode { get; set; }
    public int Number { get; set; }
    public string DisplayNumber => $"+{AreaCode} {Number}";
    public _Contact(String name, short areaCode, int number, int? id = null)
    {
        Id = id;
        Name = name;
        AreaCode = areaCode;
        Number = number;
    }
}