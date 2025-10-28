
/// <summary>
/// Represents a tool for processing payment requests.
/// </summary>
[McpServerToolType]
public class PaymentTool
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentTool> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentTool"/> class.
    /// </summary>
    /// <param name="paymentService">The payment service to process payments.</param>
    /// <param name="logger">The logger to log information and errors.</param>
    public PaymentTool(IPaymentService paymentService, ILogger<PaymentTool> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Submits a payment request asynchronously.
    /// </summary>
    /// <param name="payment">Payment information.</param>
    [McpServerTool(Name = "SubmitPayment"), Description("Enviar una solicitud de pago.")]
    public async Task<string> SubmitPaymentAsync([Description("Pago a enviar.")]Payment payment)
    {

        _logger.LogInformation("Received payment request: {Payment}", payment);

        try
        {
            await _paymentService.ProcessPaymentAsync(payment);
            return "Pago procesado correctamente.";
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogWarning(ex, "Solicitud de pago inválida");
            return "Solicitud de pago inválida.";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogError(ex, "Error al procesar el pago");
            return "Error al procesar el pago.";
        }
    }
}