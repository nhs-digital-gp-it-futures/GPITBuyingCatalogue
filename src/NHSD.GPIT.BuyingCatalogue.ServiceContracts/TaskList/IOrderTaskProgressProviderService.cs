namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList
{
    public interface IOrderTaskProgressProviderService
    {
        ITaskProgressProvider ProviderFor(OrderTaskListStatus status);
    }
}
