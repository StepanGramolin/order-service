using Refit;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public sealed class PaymentsClient
{
    private readonly IPaymentsApi _paymentsApi;

    public PaymentsClient()
    {
        _paymentsApi = RestService.For<IPaymentsApi>("http://payment-service:8080");
    }

    public async Task CreatePaymentAsync(CreatePaymentRequest request, CancellationToken ct)
    {
        try
        {
            await _paymentsApi.CreatePaymentAsync(request);
        }
        catch (ApiException ex)
        {
            // Получаем содержимое ответа через HttpContent
            string responseContent = ex.Content;
            var statusCode = ex.StatusCode;

            throw new Exception(
                $"Ошибка при создании платежа. Статус: {statusCode}. Сообщение: {responseContent}",
                ex);
        }
        catch (HttpRequestException httpEx)
        {
            throw new Exception("Произошла ошибка при выполнении HTTP-запроса", httpEx);
        }
        catch (Exception ex)
        {
            throw new Exception("Неизвестная ошибка при создании платежа", ex);
        }
    }
}

// Интерфейс API
[Headers("Content-Type: application/json")]
public interface IPaymentsApi
{
    [Post("/api/payments/create")]
    Task CreatePaymentAsync([Body] CreatePaymentRequest request);
}

// Модель запроса
public sealed record CreatePaymentRequest(long OrderId, decimal Price);