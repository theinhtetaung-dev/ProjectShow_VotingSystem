using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared;

public static class DevCode
{
    public static int ChangeToInt(this string value)
    {
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        else
        {
            throw new ArgumentException("Invalid integer value.");
        }
    }

    public static bool CanChangeToInt(this string value)
    {
        return int.TryParse(value, out _);
    }

    public static bool IsValidRequiredString(this string? value, int maxLength)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Length <= maxLength;
    }

    public static bool IsValidOptionalString(this string? value, int maxLength)
    {
        return value == null || value.Length <= maxLength;
    }

    public static bool IsPositive<T>(this T value) where T : INumber<T>
    {
        return value > T.Zero;
    }

    public static bool IsNonNegative<T>(this T value) where T : INumber<T>
    {
        return value >= T.Zero;
    }

    public static bool IsValidEmail(this string? value, int maxLength)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length <= maxLength
            && MailAddress.TryCreate(value, out var email)
            && string.Equals(email.Address, value, StringComparison.OrdinalIgnoreCase);
    }

    public static Enums.EventStatus? ToEventStatus(this string? status)
    {
        return Enum.TryParse<Enums.EventStatus>(status, false, out var eventStatus)
            && Enum.IsDefined(eventStatus)
            && string.Equals(eventStatus.ToString(), status, StringComparison.Ordinal)
                ? eventStatus
                : null;
    }

    public static bool IsValidEventStatus(this Enums.EventStatus? status)
    {
        return status.HasValue && Enum.IsDefined(status.Value);
    }

    public static bool IsValidEventDate(this DateTime eventEndAt, DateTime eventStartAt)
    {
        return eventEndAt > eventStartAt;
    }

    public static bool IsValidVotingDate(this DateTime votingEndAt, DateTime votingStartAt)
    {
        return votingEndAt > votingStartAt;
    }

    public static bool IsVotingInsideEvent(
        this DateTime votingStartAt,
        DateTime votingEndAt,
        DateTime eventStartAt,
        DateTime eventEndAt)
    {
        return votingStartAt >= eventStartAt && votingEndAt <= eventEndAt;
    }

    public static bool IsValidResultShowDate(this DateTime? resultShowEndAt, DateTime? resultShowStartAt)
    {
        return !resultShowStartAt.HasValue
            || !resultShowEndAt.HasValue
            || resultShowEndAt > resultShowStartAt;
    }
}
