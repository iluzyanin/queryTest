using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication
{
    public class Item
    {
        public Item() { }

        public Item(int id, string name, decimal price)
        {
            this.Id = id;
            this.Name = name;
            this.Price = price;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            connection.Execute("CREATE TABLE IF NOT EXISTS Items(Id INT, Name NVARCHAR(50), Price DECIMAL)");

            var items = new List<Item>
            {
                new Item(1, "Apple", 3m),
                new Item(2, "Banana", 1.4m),
                new Item(3, "Orange", 2.8m)
            };
            connection.Execute("INSERT INTO Items(Id, Name, Price) VALUES (@Id, @Name, @Price)", items);

            var dbItems = connection.Query("SELECT Id, Name, ROUND(Price,2) AS Price FROM Items")
                .Select(row => new Item(Convert.ToInt32(row.Id), row.Name, Convert.ToDecimal(row.Price)))
                .ToList();
            Console.WriteLine(dbItems.Count());
            Console.ReadLine();
        }
    }
}
