using System;
namespace SalesDataAnalyzer
{
    public class Sale
    {
        public string InvoiceNo;
        public string StockCode;
        public string Description;
        public int Quantity;
        public string InvoiceDate;
        public float UnitPrice;
        public string CustomerID;
        public string Country;
        public Sale (string invoiceNo, string stockCode, string description, int quantity, string invoiceDate, float unitPrice, string customerID, string country)
        {
            InvoiceNo = invoiceNo;
            StockCode = stockCode;
            Description = description;
            Quantity = quantity;
            InvoiceDate = invoiceDate;
            UnitPrice = unitPrice;
            CustomerID = customerID;
            Country = country;
        }

    }
}