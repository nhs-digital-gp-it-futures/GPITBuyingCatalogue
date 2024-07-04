namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed partial class Solution : IAudited
    {
        public ApplicationTypeDetail EnsureAndGetApplicationType()
        {
            return ApplicationTypeDetail ?? new ApplicationTypeDetail();
        }
    }
}
