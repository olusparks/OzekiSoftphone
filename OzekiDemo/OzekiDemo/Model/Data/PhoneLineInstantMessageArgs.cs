using System;
using Ozeki.VoIP;

namespace OzekiDemo.Model.Data
{
    public class PhoneLineInstantMessageArgs : EventArgs
    {
        public IPhoneLine PhoneLine { get; private set; }
        public InstantMessage Message { get; private set; }

        public PhoneLineInstantMessageArgs(IPhoneLine phoneLine, InstantMessage message)
        {
            PhoneLine = phoneLine;
            Message = message;
        }
    }
}
