using Microsoft.AspNetCore.Mvc;
using Stripe;
using stripe_backend.Models;

namespace stripe_backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PaymentController : ControllerBase
    {
        public PaymentController(IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = configuration["StripeSecretKey"];
        }


        [HttpPost()]
        public PaymentIntentResponse CreateIntent([FromBody] CreateIntent paymentModel)
        {
            var email = "testaccount@yopmail.com";
            var optionsForFetchingCustomer = new CustomerListOptions
            {
                Email = email
            };
            var customerService = new CustomerService();
            var desiredCustomer = customerService.List(optionsForFetchingCustomer);
            var customerId = "";
            if (desiredCustomer.Data.Count > 0)
            {
                customerId = desiredCustomer.Data[0].Id;
            } else
            {
                var customerCreationOptions = new CustomerCreateOptions
                {
                    Email = email
                };

                var customerCreationService = new CustomerService();
                var createdCustomer = customerCreationService.Create(customerCreationOptions);
                customerId = createdCustomer.Id;
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = paymentModel.Amount * 100,
                Currency = paymentModel.Currency,
                PaymentMethodTypes = paymentModel.PaymentMethodTypes,
                CaptureMethod = "manual",
                Customer = customerId,
                Description = "Buy this one",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", "123" }
                },
                PaymentMethodOptions = new PaymentIntentPaymentMethodOptionsOptions
                {
                    Card = new PaymentIntentPaymentMethodOptionsCardOptions
                    {
                        RequestThreeDSecure = "any"
                    }
                },
                ReceiptEmail = "nobi.hossain@selise.ch",
                SetupFutureUsage = "off_session"
            };
            var service = new PaymentIntentService();
            var intent = service.Create(options);
            return new PaymentIntentResponse
            {
                ClientSecret = intent.ClientSecret
            };
        }

        [HttpPost()]
        public string PaymentWithAR()
        {

            var options = new PaymentMethodListOptions
            {
                Customer = "cus_L29woyznnTtvSJ",
                Type = "card",
            };
            var service = new PaymentMethodService();
            StripeList<PaymentMethod> paymentmethods = service.List(options);

            try
            {
                var service2 = new PaymentIntentService();
                var options2 = new PaymentIntentCreateOptions
                {
                    Amount = 1099,
                    Currency = "usd",
                    Customer = "cus_L29woyznnTtvSJ",
                    PaymentMethod = "pm_1KM5cNALv0faE7iRnxuk5Xxe",
                    Confirm = true,
                    OffSession = true,
                };
                var ddd = service2.Create(options2);
            }
            catch (StripeException e)
            {
                switch (e.StripeError.Error)
                {
                    case "card_error":
                        // Error code will be authentication_required if authentication is needed
                        Console.WriteLine("Error code: " + e.StripeError.Code);
                        var paymentIntentId = e.StripeError.PaymentIntent.Id;
                        var service3 = new PaymentIntentService();
                        var paymentIntent = service.Get(paymentIntentId);

                        Console.WriteLine(paymentIntent.Id);
                        break;
                    default:
                        break;
                }
            }
                return "";
        }
        [HttpPost()]
        public PaymentIntentResponse createPaymentMethods([FromBody] CreatePaymentMethod createPaymentMethod)
        {

            var options3 = new CustomerCreateOptions
            {
                Email = createPaymentMethod.Email
            };

            var service3 = new CustomerService();
            var customer = service3.Create(options3);

            var options2 = new SetupIntentCreateOptions
            {
                Customer = customer.Id,
                PaymentMethodTypes = createPaymentMethod.PaymentMethodTypes,

            };

            var service2 = new SetupIntentService();
            var intent2 = service2.Create(options2);
            return new PaymentIntentResponse
            {
                ClientSecret = intent2.ClientSecret
            };
        }

        [HttpPost()]
        public dynamic ValidatePayment([FromBody] ValidatePayment validatePayment)
        {
            try
            {
                var options = new PaymentIntentCaptureOptions
                {

                };

                var service = new PaymentIntentService();
                var paymentIntent = service.Capture(validatePayment.ClientSecret, options);
                return paymentIntent;
            } 
            catch (Exception ex)
            {
                return ex.Message;
            }
     
        }

        [HttpGet()]
        public dynamic GetCards([FromQuery] string email)
        {

            try
            {
                var options = new CustomerListOptions
                {
                    Email = email
                };
                var service3 = new CustomerService();
                var customer = service3.List(options);

                var options2 = new PaymentMethodListOptions
                {
                    Customer = customer.Data[0].Id,
                    Type = "card"
                };

                var service = new PaymentMethodService();
                var paymentIntent = service.List(options2);
                return paymentIntent;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
