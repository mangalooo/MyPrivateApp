namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesImports
    {
        public required string Date { get; set; }
        public string? AccountNumber { get; set; }
        public string? TypeOfTransaction { get; set; }
        public required string CompanyOrInformation { get; set; }
        public required string NumberOfSharesString { get; set; }
        public required string PricePerShareString { get; set; }
        public required string AmountString { get; set; }
        public required string BrokerageString { get; set; }
        public string? Currency { get; set; }
        public required string ISIN { get; set; }
    }
}