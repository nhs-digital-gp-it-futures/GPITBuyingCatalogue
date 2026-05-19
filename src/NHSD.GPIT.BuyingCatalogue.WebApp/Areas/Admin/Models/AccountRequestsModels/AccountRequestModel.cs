using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AccountRequestsModels;

public class AccountRequestModel : NavBaseModel
{
    public AccountRequestModel()
    {
    }

    public AccountRequestModel(AccountRequest request)
    {
        FullName = request.FullName;
        Email = request.Email;
        OrganisationName = $"{request.Organisation.Name} - {request.Organisation.Id}";
        Status = request.Status;
        RequestDate = request.RequestedOn;
        DecisionDate = request.DecidedOn;
        RequestJustification = request.RequestJustification;
        DecisionJustification = request.DecisionJustification;
        DecidingUserName = request.DecidedByUser?.FullName;
        UserId = request.UserId;
        OptInUserResearch = request.HasOptedInUserResearch;
    }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string OrganisationName { get; set; }

    public AccountRequestStatus Status { get; set; }

    public DateTime RequestDate { get; set; }

    public DateTime? DecisionDate { get; set; }

    public string DecidingUserName { get; set; }

    public int? UserId { get; set; }

    public bool OptInUserResearch { get; set; }

    public AccountRequestStatus? SelectedStatus { get; set; }

    public List<SelectOption<AccountRequestStatus>> StatusOptions =>
    [
        new(nameof(AccountRequestStatus.Approved), AccountRequestStatus.Approved),
        new(nameof(AccountRequestStatus.Rejected), AccountRequestStatus.Rejected),
    ];

    public string RequestJustification { get; set; }

    [StringLength(1500)]
    public string DecisionJustification { get; set; }
}
