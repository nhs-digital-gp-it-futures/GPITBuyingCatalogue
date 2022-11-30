using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Comparers.Ordering;

public class ContactEqualityComparer : IEqualityComparer<Contact>
{
    public bool Equals(Contact x, Contact y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.FirstName == y.FirstName
            && x.LastName == y.LastName
            && x.Email == y.Email
            && x.Phone == y.Phone
            && x.LastUpdated == y.LastUpdated;
    }

    public int GetHashCode(Contact obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        return HashCode.Combine(obj.FirstName, obj.LastName, obj.Email, obj.Phone);
    }
}
