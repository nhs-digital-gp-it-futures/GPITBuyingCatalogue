using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;

public class CompetitionTaskListModel
{
    public CompetitionTaskListModel(Competition competition)
    {
        Id = competition.Id;
        Name = competition.Name;
        Description = competition.Description;

        SetSectionOneStatuses(competition);
    }

    public int Id { get; }

    public string Name { get; }

    public string Description { get; }

    public TaskProgress SolutionSelection => TaskProgress.Completed;

    public TaskProgress ServiceRecipients { get; set; } = TaskProgress.CannotStart;

    public TaskProgress ContractLength { get; set; } = TaskProgress.CannotStart;

    private void SetSectionOneStatuses(Competition competition)
    {
        ServiceRecipients = competition.Recipients.Any()
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;

        if (ServiceRecipients is TaskProgress.NotStarted) return;

        ContractLength = competition.ContractLength.HasValue
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;
    }
}
