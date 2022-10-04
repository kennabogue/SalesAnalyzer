using System;
using System.Collections.Generic;
using System.IO;

namespace SalesDataAnalyzer
{
    public static class SalesLoader
    {
        //Change back to 8
        private static int NumItemsInRow = 8;

        public static List<Sale> Load(string txtDataFilePath) {
            List<Sale> salesList = new List<Sale>();

            try
            {
                using (var reader = new StreamReader(txtDataFilePath))
                {
                    int lineNumber = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        lineNumber++;
                        if (lineNumber == 1) continue;

                        var values = line.Split(',');

                        if (values.Length != NumItemsInRow)
                        {
                            throw new Exception($"Row {lineNumber} contains {values.Length} values. It should contain {NumItemsInRow}.");
                        }
                        try
                        {
                            string invoiceNo = values[0];
                            string stockCode = values[1];
                            string description = values[2];
                            int quantity = Int32.Parse(values[3]);
                            string invoiceDate = values[4];
                            float unitPrice = Convert.ToSingle(values[5]);
                            string customerID = values[6];
                            string country = values[7];
                            
                            Sale sale = new Sale (invoiceNo, stockCode, description, quantity, invoiceDate, unitPrice, customerID, country);
                            salesList.Add(sale);
                        }
                        catch (FormatException e)
                        {
                            throw new Exception($"Row {lineNumber} contains invalid data. ({e.Message})");
                        }
                    }
                }
            } catch (Exception e){
                throw new Exception($"Unable to open {txtDataFilePath} ({e.Message}).");
            }

            return salesList;
        }
    }
}