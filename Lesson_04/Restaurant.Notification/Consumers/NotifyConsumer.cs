﻿using MassTransit;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
using Restaurant.Messages.Notification;

namespace Restaurant.Notification.Consumers
{
    public class NotifyConsumer : IConsumer<INotify>
    {
        private readonly Notifier _notifier;
        private readonly IInMemoryRepository<NotifyModel> _repository;

        public NotifyConsumer(
            Notifier notifier,
             IInMemoryRepository<NotifyModel> repository)
        {
            _notifier = notifier;
            _repository = repository;
        }

        public Task Consume(ConsumeContext<INotify> context)
        {
            Console.WriteLine($"[ OrderId: {context.Message.OrderId} ] Consume notify request");

            var model = _repository.Get().FirstOrDefault(i => i.OrderId == context.Message.OrderId);
            var t = model?.CheckMessage(context.MessageId.ToString());

            if (model != null && model.CheckMessage(context.MessageId.ToString()))
            {
                Console.WriteLine($"[ OrderId: {context.Message.OrderId} ] [ MessageID {context.MessageId} ] Second request");
                return context.ConsumeCompleted;
            }

            var requestModel = new NotifyModel
                (
                    context.Message.OrderId,
                    context.Message.ClientId,
                    context.Message.Message,
                    context.MessageId.ToString()
                );

            Console.WriteLine($"[ OrderId: {context.Message.OrderId} ] [ MessageID {context.MessageId} ] First request");

            var resultModel = model?.Update(requestModel, context.MessageId.ToString()) ?? requestModel;

            _repository.AddOrUpdate(resultModel);


            _notifier.Notify(context.Message.OrderId, context.Message.ClientId, context.Message.Message);

            return context.ConsumeCompleted;
        }
    }
}
