﻿namespace Restaurant.Booking.Models
{
    public interface IOrder
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
    }
}
