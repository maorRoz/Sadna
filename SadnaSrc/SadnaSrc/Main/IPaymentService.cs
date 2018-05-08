using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IPaymentService
    {
        void BreakExternal();

        void FixExternal();

    }

    public enum WalleterStatus
    {
        InvalidCreditCardSyntax,
        PaymentSystemError,
        NoPaymentSystem,
        InvalidData
    }

}
