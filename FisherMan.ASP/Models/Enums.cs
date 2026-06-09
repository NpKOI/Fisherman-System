using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FisherMan.ASP.Models;

public enum TicketValidityType
{
    [Display(Name = "Daily")]
    Daily = 1,

    [Display(Name = "Weekly")]
    Weekly = 2,

    [Display(Name = "Monthly")]
    Monthly = 3,

    [Display(Name = "Yearly")]
    Yearly = 4
}

public enum InspectionType
{
    [Display(Name = "Vessel")]
    Vessel = 1,

    [Display(Name = "Shop")]
    Shop = 2,

    [Display(Name = "Transport")]
    Transport = 3,

    [Display(Name = "Document check")]
    DocumentCheck = 4
}

public static class EnumDisplayExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }
}
