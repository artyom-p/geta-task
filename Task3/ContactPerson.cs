public class ContactPerson
{
    public ContactPerson(string municipality, string county, string email)
    {
        Municipality = municipality;
        County = county;
        Email = email;
    }

    public string Municipality { get; }

    public string County { get; }

    public string Email { get; }
}