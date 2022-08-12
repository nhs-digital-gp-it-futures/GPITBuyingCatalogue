namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

public class EmailDomain
{
    public EmailDomain()
    {
    }

    public EmailDomain(string domain)
    {
        Domain = domain;
    }

    public int Id { get; set; }

    public string Domain { get; set; }
}
