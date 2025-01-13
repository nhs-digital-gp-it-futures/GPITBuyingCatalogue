using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering
{
    public static class ContactTests
    {
        [Fact]
        public static void GetFullNameFromContact()
        {
            var contact = new Contact() { FirstName = "Bob", LastName = "Jones" };

            contact.FullName.Should().Be("Bob Jones");
        }

        [Fact]
        public static void GetNameOrDepartment_NameWithDepartmentSet()
        {
            var contact = new Contact() { FirstName = "Bob", LastName = "Jones", Department = "Sales" };

            contact.NameOrDepartment.Should().Be("Bob Jones");
        }

        [Fact]
        public static void GetNameOrDepartment_DepartmentAsNoNameSet()
        {
            var contact = new Contact() { Department = "Sales" };

            contact.NameOrDepartment.Should().Be("Sales");
        }
    }
}
