using System.ComponentModel.DataAnnotations;

namespace FocusFlow.Shared.Validation;

// Valid when the date is today or later. Empty values are left to [Required].
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value switch
    {
        null => true,
        DateOnly date => date >= DateOnly.FromDateTime(DateTime.Today),
        DateTime dateTime => dateTime.Date >= DateTime.Today,
        _ => false
    };
}
