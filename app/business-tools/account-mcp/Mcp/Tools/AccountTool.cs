
[McpServerToolType]
public class AccountTool
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountTool> _logger;

    public AccountTool(IAccountService accountService, ILogger<AccountTool> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves account details and available payment methods for a specific account.
    /// </summary>
    /// <param name="accountId">The ID of the specific account.</param>
    /// <returns>A task representing the asynchronous operation, containing the account details.</returns>
    [McpServerTool(Name = "GetAccountDetails"), Description("Obtener detalles de la cuenta y métodos de pago disponibles.")]
    public async Task<Account> GetAccountDetailsAsync([Description("Identificador de la cuenta.")] string accountId)
    {
        _logger.LogInformation("Solicitud recibida para obtener detalles de la cuenta: {AccountId}", accountId);
        return await _accountService.GetAccountDetailsAsync(accountId);
    }

    /// <summary>
    /// Retrieves payment method details, including the available balance, for a specific account and payment method.
    /// </summary>
    /// <param name="accountId">The ID of the specific account.</param>
    /// <param name="methodId">The ID of the specific payment method available for the account.</param>
    /// <returns>A task representing the asynchronous operation, containing the payment method details.</returns>
    [McpServerTool(Name = "GetPaymentMethodDetails"), Description("Obtener detalle del método de pago con saldo disponible.")]
    public async Task<PaymentMethod> GetPaymentMethodDetailsAsync(
        [Description("Identificador de la cuenta.")] string accountId,
        [Description("Identificador del método de pago disponible para la cuenta.")] string methodId)
    {
        _logger.LogInformation("Solicitud recibida para obtener detalles del método de pago de la cuenta {AccountId} y método {MethodId}",
            accountId, methodId);
        return await _accountService.GetPaymentMethodDetailsAsync(methodId);
    }

    /// <summary>
    /// Retrieves the list of registered beneficiaries for a specific account.
    /// </summary>
    /// <param name="accountId">The ID of the specific account.</param>
    /// <returns>A task representing the asynchronous operation, containing the list of beneficiaries.</returns>
    [McpServerTool(Name = "GetBeneficiaryDetails"), Description("Obtener la lista de beneficiarios registrados para una cuenta específica.")]
    public async Task<List<Beneficiary>> GetBeneficiaryDetailsAsync([Description("Identificador de la cuenta.")] string accountId)
    {
        _logger.LogInformation("Solicitud recibida para obtener beneficiarios de la cuenta: {AccountId}", accountId);
        return await _accountService.GetRegisteredBeneficiaryAsync(accountId);
    }
}

