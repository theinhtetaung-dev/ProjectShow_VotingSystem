using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared;

public static class Enums
{
    public enum EventStatus
    {
        Upcoming,
        Active,
        Ended,
        Published
    }

    public enum ResultStatus
    {
        first_prize,
        second_prize,
        third_prize,
        normal
    }
}
