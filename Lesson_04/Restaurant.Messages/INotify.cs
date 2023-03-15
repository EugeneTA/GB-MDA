﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public interface INotify
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
        public string Message { get; }
    }
}
