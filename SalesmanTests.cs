using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;

namespace QueryTest
{
    [TestFixture]
    public class SalesmanTests
    {
        private IDbConnection connection;
        private IList<Salesman> salesmen;
        private IList<Sale> sales;

        [SetUp]
        public void SetUp()
        {
            this.salesmen = new List<Salesman>
            {
                new Salesman { Id = 1, FirstName = "Tony", LastName = "Stark" },
                new Salesman { Id = 2, FirstName = "Peter", LastName = "Parker" },
                new Salesman { Id = 3, FirstName = "Steve", LastName = "Rodgers" }
            };

            this.sales = new List<Sale>
            {
                new Sale { Id = 1, SalesmanId = 1, Amount = 250 },
                new Sale { Id = 2, SalesmanId = 1, Amount = 200 },
                new Sale { Id = 3, SalesmanId = 1, Amount = 250 },
                new Sale { Id = 4, SalesmanId = 2, Amount = 500 },
                new Sale { Id = 5, SalesmanId = 2, Amount = 100 },
                new Sale { Id = 6, SalesmanId = 3, Amount = 50 },
                new Sale { Id = 7, SalesmanId = 3, Amount = 250 },
            };

            this.connection = new SqliteConnection("Data Source=:memory:");
            this.connection.Open();

            PrepareDatabase();
        }

        [Test]
        public void When_executing_AllSalesmen_query_Should_return_all_salesmen()
        {
            // Arrange
            var allSalesmenQuery = File.ReadAllText(@"Queries/AllSalesmen.sql");

            // Act
            var allSalesmen = connection.Query<Salesman>(allSalesmenQuery).ToList();

            // Assert
            allSalesmen.ShouldBeEquivalentTo(this.salesmen);
        }

        [Test]
        public void When_executing_BestSalesmen_query_Should_return_correct_salesmen()
        {
            // Arrange
            var bestSalesmenQuery = File.ReadAllText(@"Queries/BestSalesmen.sql");
            var expectedBestSalesmen = new List<Salesman> { this.salesmen[0], this.salesmen[1] };

            // Act
            var minimumAmount = 500;
            var bestSalesmen = connection.Query<Salesman>(bestSalesmenQuery, new { amount = minimumAmount }).ToList();

            // Assert
            bestSalesmen.ShouldBeEquivalentTo(expectedBestSalesmen);
        }

        private void PrepareDatabase()
        {
            this.connection.Execute("CREATE TABLE IF NOT EXISTS Salesmen(Id INT, FirstName NVARCHAR(50), LastName NVARCHAR(50))");
            this.connection.Execute("CREATE TABLE IF NOT EXISTS Sales(Id INT, SalesmanId INT, Amount DECIMAL)");

            this.connection.Execute("INSERT INTO Salesmen(Id, FirstName, LastName) VALUES (@Id, @FirstName, @LastName)", this.salesmen);
            this.connection.Execute("INSERT INTO Sales(Id, SalesmanId, Amount) VALUES (@Id, @SalesmanId, @Amount)", this.sales);
        }
    }
}