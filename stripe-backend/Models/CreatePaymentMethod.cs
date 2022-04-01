namespace stripe_backend.Models
{
    public class CreatePaymentMethod
    {
        public string Email { get; set; }
        public List<string> PaymentMethodTypes { get; set; }
    }
}
