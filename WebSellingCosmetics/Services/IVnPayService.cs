using WebSellingCosmetics.ViewModel;

namespace WebSellingCosmetics.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModels PaymentExecute(IQueryCollection collections);
    }
}
