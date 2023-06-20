using System.Linq;
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

        ServiceRecipients = competition.Recipients.Any() ? TaskProgress.Completed : TaskProgress.NotStarted;
    }

    public int Id { get; }

    public string Name { get; }

    public string Description { get; }

    public TaskProgress SolutionSelection => TaskProgress.Completed;

    public TaskProgress ServiceRecipients { get; } = TaskProgress.CannotStart;

    public TaskProgress ContractLength { get; } = TaskProgress.CannotStart;
}
