﻿using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public class TimeUnit : EnumerationBase
    {
        public static readonly TimeUnit PerMonth = new(1, "month", "per month", 12);
        public static readonly TimeUnit PerYear = new(2, "year", "per year", 1);

        public TimeUnit(int id, string name, string description, int amountInYear)
            : base(id, name)
        {
            AmountInYear = amountInYear;
            Description = description;
        }

        public string Description { get; private set; }

        public int AmountInYear { get; private set; }

        public static TimeUnit Parse(string name)
        {
            if (name.Equals(nameof(PerMonth), System.StringComparison.InvariantCultureIgnoreCase))
                return PerMonth;
            else if (name.Equals(nameof(PerYear), System.StringComparison.InvariantCultureIgnoreCase))
                return PerMonth;

            throw new ArgumentException("Invalid TimeUnit", nameof(name));
        }
    }
}
