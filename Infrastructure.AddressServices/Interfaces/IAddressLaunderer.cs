﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;

namespace Infrastructure.AddressServices.Interfaces
{
    interface IAddressLaunderer
    {
        Address Launder(string inputAddress);
        Address Launder(Address inputAddress);
    }
}
