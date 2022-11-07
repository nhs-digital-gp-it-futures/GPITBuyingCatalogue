namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public class UserStatusModel : NavBaseModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public int UserId { get; init; }

        public string UserEmail { get; init; }

        public bool IsActive { get; init; }

        public string ControllerName { get; set; }
    }
}
