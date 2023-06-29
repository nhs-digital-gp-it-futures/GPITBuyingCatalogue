using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public static class AdvancedTelephonyConstants
{
    public const string BabbleVoice = "BabbleVoice";
    public const string BuyersGuide = "BuyersGuide";
    public const string CheckCloud = "CheckCloud";
    public const string DaisyPatientLine = "DaisyPatientLine";
    public const string LouisComm = "Louiscomm";
    public const string Redcentric = "Redcentric";
    public const string RpmPatientContact = "RPMPatientContact";
    public const string SurgeryConnect = "SurgeryConnect";
    public const string ThinkHealthcare = "ThinkHealthcare";
    public const string WaveNet = "Wavenet";

    private static readonly Dictionary<string, string> FileMappings = new()
    {
        { BabbleVoice, "Babblevoice" },
        { BuyersGuide, "Buyer's Guide for Advanced Cloud-based Telephony-Jan 2023" },
        { CheckCloud, "Check Cloud" },
        { DaisyPatientLine, "Daisy Patient Line" },
        { LouisComm, "Louiscomm" },
        { Redcentric, "Redcentric" },
        { RpmPatientContact, "RPM Patient Contact" },
        { SurgeryConnect, "Surgery Connect" },
        { ThinkHealthcare, "Think Healthcare" },
        { WaveNet, "Wavenet" },
    };

    public static bool TryGetFileMapping(string file, out string value) => FileMappings.TryGetValue(file, out value);
}
