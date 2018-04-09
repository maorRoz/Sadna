using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IPaymentService
    {
      /*  MarketAnswer AttachExternalSystem();
        MarketAnswer ProccesPayment(int orderId, string address, List<string> details);*/
        void BreakExternal();
    }

    public enum WalleterStatus
    {
        Success,
        InvalidCreditCardSyntax,
        PaymentSystemError,
        NoPaymentSystem
    }

}
