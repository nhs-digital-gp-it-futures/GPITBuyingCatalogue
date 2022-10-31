namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderTaskListService
    {
        OrderTaskListModel GetTaskListStatuses(OrderWrapper orderWrapper);
    }
}
