﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public interface IBookingExpire
    {
        public Guid OrderId { get; }
    }
}
