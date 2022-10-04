using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SalesDataAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ex. dotnet run salesData.csv salesReport.txt
            if (args.Length != 2) {
                Console.WriteLine("SalesDataAnalyzer <sales_data_file_path> <report_file_path>");
                Environment.Exit(1);
            }

            string salesFilePath = args[0];
            string reportFilePath = args[1];

            //Making the report:
            List<Sale> salesList = null;
            try
            {
                salesList = SalesLoader.Load(salesFilePath);

            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Environment.Exit(2);
            }
            try
            {
                using (var reportwriter = new StreamWriter(reportFilePath))
                {
                reportwriter.WriteLine("SALES DATA ANALYZER REPORT");
                reportwriter.WriteLine("----------");

                //1. List all the items sold to customers in Australia (stockCode and Description).
                var australiaItemSales = from sale in salesList where sale.Country == "Australia" select sale;
                reportwriter.WriteLine("\n1. The items sold to customers in Australia were:");
                foreach(var selected in australiaItemSales)
                    {
                        reportwriter.WriteLine("Stock Code: " + selected.StockCode + " |" + " Description: " + selected.Description);
                        reportwriter.WriteLine("---------");
                    }
                
                //2. How many individual sales were there? To determine this you have to count the unique invoice numbers. You should group by invoice number.
                var uniqueSales = (from sale in salesList orderby sale.InvoiceNo select sale.InvoiceNo).Distinct();
                int totalUniqueSales = uniqueSales.Count();
                reportwriter.WriteLine("\n2. Total individual sales: " + totalUniqueSales);
                
                //3. What is the sales total for invoice number 536365? Sales total for an invoice will be the sum of (Quantity * UnitPrice) for all products sold in the invoice.
                var invoiceNO536365 = from sale in salesList where sale.InvoiceNo == "536365" select sale;
                float invoiceTotal = 0;
                foreach (var selected in invoiceNO536365)
                {
                    invoiceTotal+= selected.UnitPrice * selected.Quantity;
                }
                reportwriter.WriteLine("\n3. Sales total for invoice number 536365: $" + Math.Round(invoiceTotal,2));

                //4. List the total sales by country?
                reportwriter.WriteLine("\n4. Total sales by country: ");
                var salesByCounty = from sale in salesList 
                                    group sale by sale.Country into newGroup 
                                    orderby newGroup.Key select newGroup;
                float countryTotal = 0;
                foreach (var selected in salesByCounty)
                {
                    foreach (var transanction in selected)
                    {
                        countryTotal+= transanction.UnitPrice * transanction.Quantity;
                    }
                    reportwriter.WriteLine("Country: " + selected.Key + " |" + " Total Sales: $" + Math.Round(countryTotal,2));
                    reportwriter.WriteLine("---------");
                }

                //5. Which customer has spent the most money during the period?
                var customerHighestSales = (from sale in salesList 
                                          group (sale.UnitPrice * sale.Quantity) by sale.CustomerID into saleGroup
                                          orderby saleGroup.Sum() descending
                                          select saleGroup).FirstOrDefault();
                reportwriter.WriteLine("\n5. Customer that has spent the most: " + customerHighestSales.Key);

                //6. What are the total sales to customer 15311?
                var customer15311 = from sale in salesList where sale.CustomerID == "15311" select sale;
                float customer153111Totals = 0;
                foreach(var selected in customer15311)
                {
                    customer153111Totals += selected.Quantity * selected.UnitPrice;
                }
                reportwriter.WriteLine("\n6. Total sales for customer 15311: $" + Math.Round(customer153111Totals,2));

                //7. How many units of “HAND WARMER UNION JACK” were sold in the dataset?
                var handWarmerTotals = from sale in salesList where sale.Description == "HAND WARMER UNION JACK" select sale;
                int handWarmers = 0;
                foreach(var selected in handWarmerTotals)
                {
                    handWarmers += selected.Quantity;
                }
                reportwriter.WriteLine("\n7. " + handWarmers + " Hand Warmer Union Jack were sold.");
                
                //8. What was the total value of the “HAND WARMER UNION JACK” sales in the dataset?
                float handWarmersValue = 0;
                foreach(var selected in handWarmerTotals)
                {
                    handWarmersValue += selected.Quantity * selected.UnitPrice;
                }
                reportwriter.WriteLine("\n8. Total value of Hand Warmer Union Jack sales: $" + Math.Round(handWarmersValue,2));

                //9. Which product has the highest UnitPrice (stockCode and Description).?
                var highestPrice = (from sale in salesList orderby sale.UnitPrice descending select sale).FirstOrDefault();
                reportwriter.WriteLine("\n9. The product with the highest unit price is:");
                reportwriter.WriteLine("Stock Code: " + highestPrice.StockCode + " |" + " Description: " + highestPrice.Description);
                reportwriter.WriteLine("---------");

                //10. What is the total sales for the entire dataset?
                var totalSales = from sale in salesList select sale;
                float count = 0;
                foreach(var selected in totalSales)
                {
                    count += selected.UnitPrice * selected.Quantity;
                }
                reportwriter.WriteLine("\n10. The total sales were $" + Math.Round(count,2) + ".");

                //11. Which invoice number had the highest sales?
                var invoiceHighestSales = (from sale in salesList 
                                          group (sale.UnitPrice * sale.Quantity) by sale.InvoiceNo into saleGroup
                                          orderby saleGroup.Sum() descending
                                          select saleGroup).FirstOrDefault();
                reportwriter.WriteLine("\n11. Invoice number with the highest sales: " + invoiceHighestSales.Key);

                //12. Which product sold the most units?
                var productMostSales = (from sale in salesList 
                                    group sale.Quantity by sale.Description into saleGroup
                                    orderby saleGroup.Sum() descending
                                    select saleGroup.Key).FirstOrDefault();             
                reportwriter.WriteLine("\n12. Product with the most units sold: "+ productMostSales);

                reportwriter.Close();

         
                }
            }catch(Exception e){
                Console.WriteLine(e.Message);
                Environment.Exit(3);
                //if report can't be created
            }
            finally
            {
                if(!File.Exists(reportFilePath))
                {
                    Console.WriteLine("Couldn't create that report file... Try naming it something else.");
                }
                else
                    Console.WriteLine("Sales report was successfully created in the current directory!");
            }
    
        }
    }
}