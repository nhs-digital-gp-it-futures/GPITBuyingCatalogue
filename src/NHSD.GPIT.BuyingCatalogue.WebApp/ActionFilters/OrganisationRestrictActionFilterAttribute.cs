using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;

public class OrganisationRestrictActionFilterAttribute : ActionFilterAttribute
{
    internal const string OrganisationIdKey = "organisationId";

    private readonly IOrganisationsService organisationsService;
    private readonly ILogger<OrganisationRestrictActionFilterAttribute> logger;

    public OrganisationRestrictActionFilterAttribute(
        IOrganisationsService organisationsService,
        ILogger<OrganisationRestrictActionFilterAttribute> logger)
    {
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is not Controller controller
            || !context.ActionArguments.TryGetValue(OrganisationIdKey, out var organisationIdValue)
            || organisationIdValue is not int organisationId)
        {
            await base.OnActionExecutionAsync(context, next);
            return;
        }

        var userPrimaryOrganisationId = controller.User.GetPrimaryOrganisationInternalIdentifier();
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(userPrimaryOrganisationId);

        if (organisation.Id != organisationId)
        {
            logger.LogWarning("User {UserId} tried to access Organisation {OrganisationId}", controller.User.UserId(), organisationId);
            context.Result = new NotFoundResult();

            return;
        }

        await base.OnActionExecutionAsync(context, next);
    }
}
