using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IPaymentService
    {
        //TODO: you shouldn't let the client get any interaction with this interface, no MarketAnswer is needed here
        MarketAnswer AttachExternalSystem();
        MarketAnswer ProccesPayment(int orderId, string address, List<string> details);
    }

    public enum WalleterStatus
    {
        Success,
        InvalidCreditCardSyntax,
        PaymentSystemError,
        NoPaymentSystem
    }

}
