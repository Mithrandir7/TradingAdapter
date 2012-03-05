using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderInformation
{
    public enum OrderState
    {
        WaitingSubmit,
        GiveUp,
        Submitted,
        OrderFailed,
        WaitingFilled, //Canceable,
        CancealOrder,
        Canceled,
        Filled,
        WaitingClose, // CloseSubmitted
        ClosedFailed,
        Closed,
        Unknown
    }
}
