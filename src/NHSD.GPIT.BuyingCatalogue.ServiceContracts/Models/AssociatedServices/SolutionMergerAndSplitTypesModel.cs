using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices
{
    public sealed class SolutionMergerAndSplitTypesModel
    {
        public SolutionMergerAndSplitTypesModel()
        {
        }

        public SolutionMergerAndSplitTypesModel(string solutionName, IEnumerable<PracticeReorganisationTypeEnum> servicesType)
        {
            SolutionName = solutionName;
            SelectedMergerAndSplitsServices = servicesType.ToList();
        }

        public string SolutionName { get; set; }

        public List<PracticeReorganisationTypeEnum> SelectedMergerAndSplitsServices { get; set; } = new List<PracticeReorganisationTypeEnum>();

        public bool IsValid => !IsNotValid;

        public bool IsNotValid => MultipleMergers || MultipleSplits || MultiplePracticeReorganisations || BadCombination;

        private bool MultipleMergers => SelectedMergerAndSplitsServices
            .Count(s => s == PracticeReorganisationTypeEnum.Merger) > 1;

        private bool MultipleSplits => SelectedMergerAndSplitsServices
            .Count(s => s == PracticeReorganisationTypeEnum.Split) > 1;

        private bool MultiplePracticeReorganisations => SelectedMergerAndSplitsServices
            .Count(s => s == (PracticeReorganisationTypeEnum.Split | PracticeReorganisationTypeEnum.Merger)) > 1;

        private bool BadCombination => SelectedMergerAndSplitsServices.Any(s => s == (PracticeReorganisationTypeEnum.Split | PracticeReorganisationTypeEnum.Merger))
                && (SelectedMergerAndSplitsServices.Any(s => s == PracticeReorganisationTypeEnum.Split)
                    || SelectedMergerAndSplitsServices.Any(s => s == PracticeReorganisationTypeEnum.Merger));

        public SolutionMergerAndSplitTypesModel With(PracticeReorganisationTypeEnum practiceReorganisationType)
        {
            var list = SelectedMergerAndSplitsServices.ToList();
            list.Add(practiceReorganisationType);
            return new SolutionMergerAndSplitTypesModel(SolutionName, list);
        }
    }
}
