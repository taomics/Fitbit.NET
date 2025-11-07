using System;

namespace Fitbit.Models
{
    public enum HeartRateResolution
    {
        oneSecond = 1,
        oneMinute = 2,
        fiveMinute = 3,
        [Obsolete("No longer supported by Fitbit.")]
        fifteenMinute = 4
    }
}
