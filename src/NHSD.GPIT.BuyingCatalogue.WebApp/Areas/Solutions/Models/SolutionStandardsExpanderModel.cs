using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionStandardsExpanderModel
    {
        public SolutionStandardsExpanderModel(
            string title,
            string content,
            string testId,
            IEnumerable<StandardComplianceModel> standards,
            IEnumerable<string> standardsWithWorkOffPlans,
            CatalogueItemId solutionId,
            bool showAction)
        {
            Title = title;
            Content = content;
            TestId = testId;
            Standards = standards;
            StandardsWithWorkOffPlans = standardsWithWorkOffPlans;
            SolutionId = solutionId;
            ShowAction = showAction;
        }

        public SolutionStandardsExpanderModel()
        {
        }

        public string Title { get; }

        public string Content { get; }

        public string TestId { get; }

        public IEnumerable<StandardComplianceModel> Standards { get; }

        public IEnumerable<string> StandardsWithWorkOffPlans { get; }

        public CatalogueItemId SolutionId { get; }

        public bool ShowAction { get; }
    }
}
