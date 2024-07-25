using Microsoft.EntityFrameworkCore;

namespace NHSD.GPIT.BuyingCatalogue.Identity.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options);
