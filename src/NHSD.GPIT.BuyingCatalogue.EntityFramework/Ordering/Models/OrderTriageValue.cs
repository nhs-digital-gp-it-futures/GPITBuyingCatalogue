namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

public enum OrderTriageValue
{
    /// <summary>
    /// Direct award.
    /// </summary>
    Under40K = 0,

    /// <summary>
    /// On-catalogue competition.
    /// </summary>
    Between40KTo250K = 1,

    /// <summary>
    /// Off-catalogue competition.
    /// </summary>
    Over250K = 2,
    NotSure = 3,
}
