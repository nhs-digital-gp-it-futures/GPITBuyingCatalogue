namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed partial class SupplierContact
    {
        public string Description
        {
            get
            {
                var output = FullName;

                if (!string.IsNullOrWhiteSpace(Department))
                {
                    if (string.IsNullOrWhiteSpace(output))
                    {
                        output = Department;
                    }
                    else
                    {
                        output += $" ({Department})";
                    }
                }

                return output;
            }
        }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string NameOrDepartment => !string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName)
            ? FullName
            : Department;
    }
}
