using System;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsConfirmationBanner;

public class NhsConfirmationBannerModel
{
    public enum BannerColour
    {
        Blue,
        Grey,
    }

    public string Title { get; set; }

    public string Body { get; set; }

    public BannerColourType Colour { get; set; } = BannerColour.Blue;

    public struct BannerColourType
    {
        private readonly BannerColour colour;

        public BannerColourType(BannerColour colour)
        {
            this.colour = colour;
        }

        public static implicit operator BannerColourType(BannerColour colour) => new BannerColourType(colour);

        public string ToColourStyle() => colour switch
        {
            BannerColour.Blue => "govuk-panel--confirmation--blue",
            BannerColour.Grey => "govuk-panel--confirmation--grey",
            _ => throw new ArgumentOutOfRangeException(nameof(colour)),
        };
    }
}
