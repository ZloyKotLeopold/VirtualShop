using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualShopm
{
    internal class Program
    {
        static void Main()
        {
            int money = 500;

            Seller seller = new Seller();
            Client client = new Client(money);
            Market market = new Market(seller, client);

            seller.AddProducts(new List<Product>
            {
                new Product("Молоко", 55),
                new Product("Хлеб", 27),
                new Product("Сахар", 80),
                new Product("Чай", 65),
                new Product("Кофе", 100),
                new Product("Печеньки", 95),
                new Product("Мясо", 155),
                new Product("Помидоры", 85),
                new Product("Сыр", 255),
                new Product("Чипсы", 75),
            });

            market.StartGame();
        }
    }

    public class Human
    {
        protected int Money;
        protected List<Product> Products;

        public Human()
        {
            Products = new List<Product>();
        }

        public void ShowProducts()
        {
            foreach (var product in Products)
                Console.WriteLine($"{product.Name} по цене: {product.Price}.");
        }
    }

    public class Client : Human
    {
        public Client(int money)
        {
            Money = money;
        }

        public bool CanPay(int price) => price <= Money;

        public void Buy(Product product)
        {
            Money -= product.Price;

            Products.Add(product);

            Console.WriteLine($"Вы купили: {product.Name} за {product.Price} условных попугаев.");
        }
    }

    public class Seller : Human
    {
        public void AddProducts(List<Product> products) => Products.AddRange(products);

        public bool TryGetProduct(out Product foundProduct)
        {
            Console.WriteLine($"Введите название товара который хотите купить.");

            string productName = Console.ReadLine();

            foreach (var product in Products)
            {
                if (product.Name.ToLower() == productName.ToLower())
                {
                    foundProduct = product;

                    return true;
                }
            }

            Console.WriteLine($"Такого товару нету...");

            foundProduct = null;

            return false;
        }

        public void Sell(Product product)
        {
            Money += product.Price;

            Products.Remove(product);
        }
    }

    public class Market
    {
        private readonly Seller _seller;
        private readonly Client _client;

        public Market(Seller seller, Client client)
        {
            _seller = seller;
            _client = client;
        }

        public void StartGame()
        {
            const string ParameterShowAssortment = "1";
            const string ParameterShowBasket = "2";
            const string ParameterShop = "3";
            const string ParameterExit = "4";

            bool isActive = true;

            while (isActive)
            {
                Console.Clear();

                Console.WriteLine($"Для того чтобы посмотреть ассортимент магазина введите - {ParameterShowAssortment}");
                Console.WriteLine($"Для того чтобы посмотреть ваши вещи введите - {ParameterShowBasket}");
                Console.WriteLine($"Для того чтобы совершать покупки введите - {ParameterShop}");
                Console.WriteLine($"Для того чтобы выйти введите - {ParameterExit}");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case ParameterShowAssortment:
                        _seller.ShowProducts();
                        break;

                    case ParameterShowBasket:
                        _client.ShowProducts();
                        break;

                    case ParameterShop:
                        MakeDeal();
                        break;

                    case ParameterExit:
                        isActive = false;
                        break;

                    default:
                        Console.WriteLine("Некорректный ввод.");
                        break;
                }

                Console.ReadKey();
            }
        }

        private void MakeDeal()
        {
            if (_seller.TryGetProduct(out Product product))
            {
                if (_client.CanPay(product.Price))
                {
                    _seller.Sell(product);

                    _client.Buy(product);
                }
                else
                {
                    Console.WriteLine("Недостаточно денег.");
                }
            }
        }
    }

    public class Product
    {
        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; private set; }
        public int Price { get; private set; }

        public void ShowProduct()
        {
            Console.WriteLine($"{Name}\nЦена: {Price}\n");
        }
    }
}
