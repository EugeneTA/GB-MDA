namespace Restaurant.Notification
{
    public class Notifier
    {
        public void Notify(Guid orderId, Guid clientId, string message)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine($"[ OrderId: {orderId} ] Уважаемый клиент [ {clientId} ]! {message}");

        }
    }
}
