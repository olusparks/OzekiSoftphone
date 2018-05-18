using Ozeki.VoIP;
using OzekiDemo.Model.Data;
using System.Collections.Generic;

namespace OzekiDemo.Model
{
    public class TransferModel
    {
        public TransferMode TransferMode { get; set; }
        public string BlindTransferTarget { get; set; }
        public IPhoneCall AttendedTransferTarget { get; set; }
        public IEnumerable<IPhoneCall> PhoneCalls { get; private set; }
        public IPhoneCall Transferee { get; private set; }

        public TransferModel(IEnumerable<IPhoneCall> phoneCalls, IPhoneCall transferee)
        {
            PhoneCalls = phoneCalls;
            TransferMode = TransferMode.Blind;
            Transferee = transferee;
        }
    }
}
