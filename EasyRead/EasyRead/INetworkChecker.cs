using System;
using System.Collections.Generic;
using System.Text;

namespace EasyRead
{
    public interface INetworkChecker
    {
        bool IsConnected { get; }
        void CheckInternetConnection();
    }
}
