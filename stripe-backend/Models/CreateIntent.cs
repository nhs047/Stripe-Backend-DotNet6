namespace stripe_backend.Models
{
    public class CreateIntent
    {
        public long Amount { get; set; }
        public string Currency { get; set; }
        public List<string> PaymentMethodTypes { get; set; }
    }
}
