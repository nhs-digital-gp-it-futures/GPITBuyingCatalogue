using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

public class AccountRequest
{
    public AccountRequest()
    {
    }

    public AccountRequest(
        string firsTName,
        string lastName,
        string email,
        string odsCode,
        string requestJustification,
        bool hasOptedInUserResearch)
    {
        FirstName = firsTName;
        LastName = lastName;
        Email = email;
        OdsCode = odsCode;
        RequestJustification = requestJustification;
        HasOptedInUserResearch = hasOptedInUserResearch;
        RequestedOn = DateTime.UtcNow;
    }

    public Guid Id { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; }

    [StringLength(100)]
    public string LastName { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [StringLength(256)]
    public string Email { get; set; }

    [StringLength(10)]
    public string OdsCode { get; set; }

    public AccountRequestStatus Status { get; set; } = AccountRequestStatus.Pending;

    [StringLength(1500)]
    public string RequestJustification { get; set; }

    [StringLength(1500)]
    public string DecisionJustification { get; set; }

    public bool HasOptedInUserResearch { get; set; }

    public DateTime RequestedOn { get; set; }

    public DateTime? DecidedOn { get; set; }

    public int? UserId { get; set; }

    public int? DecidedBy { get; set; }

    public bool IsConfirmed { get; set; }

    public AspNetUser DecidedByUser { get; set; }

    public OdsOrganisation Organisation { get; set; }
}
