using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared;

public static class DevCode
{
    public static bool IsPositiveInt(this int value)
    {
        return value > 0;
    }

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
}
