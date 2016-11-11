using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connection = SetupDatabase();
            InsertTestData(connection);

            var allSalesmenQuery = File.ReadAllText(@"Queries\AllSalesmen.sql");
            var allSalesmen = connection.Query(allSalesmenQuery).ToList();

            Console.WriteLine("All salesmen:");
            foreach (var salesman in allSalesmen)
            {
                Console.WriteLine($"{salesman.Id} {salesman.FirstName} {salesman.LastName}");
            }

            var bestSalesmenQuery = File.ReadAllText(@"Queries\BestSalesmen.sql");
            var minimumAmount = 500;
            var bestSalesmen = connection.Query(bestSalesmenQuery, new { amount = minimumAmount }).ToList();

            Console.WriteLine($"Salesmen with overall sales more than {minimumAmount}:");
            foreach (var salesman in bestSalesmen)
            {
                Console.WriteLine($"{salesman.Id} {salesman.FirstName} {salesman.LastName}");
            }
        }

        private static IDbConnection SetupDatabase()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            connection.Execute("CREATE TABLE IF NOT EXISTS Salesmen(Id INT, FirstName NVARCHAR(50), LastName NVARCHAR(50))");
            connection.Execute("CREATE TABLE IF NOT EXISTS Sales(Id INT, SalesmanId INT, Amount DECIMAL)");

            return connection;
        }

        private static void InsertTestData(IDbConnection connection)
        {
            var salesmen = new List<Salesman>
            {
                new Salesman { Id = 1, FirstName = "Tony", LastName = "Stark" },
                new Salesman { Id = 2, FirstName = "Peter", LastName = "Parker" },
                new Salesman { Id = 3, FirstName = "Steve", LastName = "Rodgers" }
            };

            var sales = new List<Sale>
            {
                new Sale { Id = 1, SalesmanId = 1, Amount = 250 },
                new Sale { Id = 2, SalesmanId = 1, Amount = 200 },
                new Sale { Id = 3, SalesmanId = 1, Amount = 250 },
                new Sale { Id = 4, SalesmanId = 2, Amount = 500 },
                new Sale { Id = 5, SalesmanId = 2, Amount = 100 },
                new Sale { Id = 6, SalesmanId = 3, Amount = 50 },
                new Sale { Id = 7, SalesmanId = 3, Amount = 250 },
            };

            connection.Execute("INSERT INTO Salesmen(Id, FirstName, LastName) VALUES (@Id, @FirstName, @LastName)", salesmen);
            connection.Execute("INSERT INTO Sales(Id, SalesmanId, Amount) VALUES (@Id, @SalesmanId, @Amount)", sales);
        }
    }
}
