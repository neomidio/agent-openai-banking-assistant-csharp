public class TransactionService : ITransactionService
{
    private Dictionary<string, List<Transaction>> LastTransactions { get; } = new();
    private Dictionary<string, List<Transaction>> AllTransactions { get; } = new();

    public TransactionService()
    {
        LastTransactions["1010"] = new List<Transaction>
        {
            new Transaction("11", "Pago de la factura 334398", "outcome", "Servicios Acme", "0001", "1010", "BankTransfer", 100.00m, DateTime.Parse("2024-4-01T12:00:00Z")),
            new Transaction("22", "Pago de la factura 4613", "outcome", "Finanzas Contoso", "0002", "1010", "CreditCard", 200.00m, DateTime.Parse("2024-3-02T12:00:00Z")),
            new Transaction("33", "Pago de la factura 724563", "outcome", "Distribuciones Duff", "0003", "1010", "BankTransfer", 300.00m, DateTime.Parse("2023-10-03T12:00:00Z")),
            new Transaction("43", "Pago de la factura 8898943", "outcome", "Industrias Wayne", "0004", "1010", "DirectDebit", 400.00m, DateTime.Parse("2023-8-04T12:00:00Z")),
            new Transaction("53", "Pago de la factura 19dee", "outcome", "Oscorp Latam", "0005", "1010", "BankTransfer", 500.00m, DateTime.Parse("2023-4-05T12:00:00Z"))
        };

        AllTransactions["1010"] = new List<Transaction>
        {
            new Transaction("11", "pago de la factura con id 0001", "outcome", "Servicios Acme", "A012TABTYT156!", "1010", "BankTransfer", 100.00m, DateTime.Parse("2024-4-01T12:00:00Z")),
            new Transaction("21", "Pago de la factura 4200", "outcome", "Servicios Acme", "0002", "1010", "BankTransfer", 200.00m, DateTime.Parse("2024-1-02T12:00:00Z")),
            new Transaction("31", "Pago de la factura 3743", "outcome", "Servicios Acme", "0003", "1010", "DirectDebit", 300.00m, DateTime.Parse("2023-10-03T12:00:00Z")),
            new Transaction("41", "Pago de la factura 8921", "outcome", "Servicios Acme", "0004", "1010", "Transfer", 400.00m, DateTime.Parse("2023-8-04T12:00:00Z")),
            new Transaction("51", "Pago de la factura 7666", "outcome", "Servicios Acme", "0005", "1010", "CreditCard", 500.00m, DateTime.Parse("2023-4-05T12:00:00Z")),

            new Transaction("12", "Pago de la factura 5517", "outcome", "Finanzas Contoso", "0001", "1010", "CreditCard", 100.00m, DateTime.Parse("2024-3-01T12:00:00Z")),
            new Transaction("22", "Pago de la factura 682222", "outcome", "Finanzas Contoso", "0002", "1010", "CreditCard", 200.00m, DateTime.Parse("2023-1-02T12:00:00Z")),
            new Transaction("32", "Pago de la factura 94112", "outcome", "Finanzas Contoso", "0003", "1010", "Transfer", 300.00m, DateTime.Parse("2022-10-03T12:00:00Z")),
            new Transaction("42", "Pago de la factura 23122", "outcome", "Finanzas Contoso", "0004", "1010", "Transfer", 400.00m, DateTime.Parse("2022-8-04T12:00:00Z")),
            new Transaction("52", "Pago de la factura 171443", "outcome", "Finanzas Contoso", "0005", "1010", "Transfer", 500.00m, DateTime.Parse("2020-4-05T12:00:00Z"))
        };
    }

    public List<Transaction> GetTransactionsByRecipientName(string accountId, string name)
    {
        ValidateAccountId(accountId);

        if (!AllTransactions.TryGetValue(accountId, out var transactions))
            return new List<Transaction>();

        return transactions
            .Where(t => t.RecipientName.ToLower().Contains(name.ToLower()))
            .ToList();
    }

    public List<Transaction> GetLastTransactions(string accountId)
    {
        ValidateAccountId(accountId);

        return LastTransactions.TryGetValue(accountId, out var transactions)
            ? transactions
            : new List<Transaction>();
    }

    public void NotifyTransaction(string accountId, Transaction transaction)
    {
        ValidateAccountId(accountId);

        if (!AllTransactions.TryGetValue(accountId, out var allTransactionsList))
            throw new InvalidOperationException($"No se encontraron todas las transacciones para la cuenta: {accountId}");

        if (!LastTransactions.TryGetValue(accountId, out var lastTransactionsList))
            throw new InvalidOperationException($"No se encontraron las últimas transacciones para la cuenta: {accountId}");

        allTransactionsList.Add(transaction);
        lastTransactionsList.Add(transaction);
    }

    private void ValidateAccountId(string accountId)
    {
        if (string.IsNullOrEmpty(accountId))
            throw new ArgumentException("El identificador de cuenta está vacío o es nulo");

        if (!int.TryParse(accountId, out _))
            throw new ArgumentException("El identificador de cuenta no es un número válido");
    }
}
