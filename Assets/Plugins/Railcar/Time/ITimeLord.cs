using System;

namespace Railcar.Time
{
    public interface ITimeLord
    {
        bool DoesFlow { get; set; }
        event EventHandler<bool> FlowingUpdated;
    }
}