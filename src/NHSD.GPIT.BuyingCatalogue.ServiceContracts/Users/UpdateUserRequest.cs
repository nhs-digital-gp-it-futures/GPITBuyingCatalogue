using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

public record UpdateUserRequest
{
    public required int UserId { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public required bool Disabled { get; set; }

    public required string OrganisationFunction { get; set; }

    public required int OrganisationId { get; set; }

    public required DateTime? ReactivationDate { get; set; }
}
