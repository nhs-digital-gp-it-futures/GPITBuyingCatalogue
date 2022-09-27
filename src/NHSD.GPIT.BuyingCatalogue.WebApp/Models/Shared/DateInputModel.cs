using System;
using System.Globalization;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared
{
    public class DateInputModel : NavBaseModel
    {
        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public DateTime? Date
        {
            get
            {
                if (!IsComplete)
                {
                    return null;
                }

                if (!DateTime.TryParseExact($"{Day}/{Month}/{Year}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
                {
                    return null;
                }

                return date.ToUniversalTime();
            }
        }

        public bool IsComplete => !string.IsNullOrWhiteSpace(Day)
            && !string.IsNullOrWhiteSpace(Month)
            && !string.IsNullOrWhiteSpace(Year)
            && Year.Length == 4;

        public bool IsValid => Date != null;

        protected void SetDateFields(DateTime? date)
        {
            if (date == null)
            {
                return;
            }

            Day = $"{date.Value.Day:00}";
            Month = $"{date.Value.Month:00}";
            Year = $"{date.Value.Year:0000}";
        }
    }
}
