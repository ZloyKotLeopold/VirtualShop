using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualShop
{
    internal class Program
    {
        static void Main()
        {
            const string parameterShowAssortment = "1";
            const string parameterShowBasket = "2";
            const string parameterShop = "3";
            const string parameterExit = "4";

            int money = 500;
            bool isActive = true;

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

            while (isActive)
            {
                Console.Clear();

                Console.WriteLine($"Для того чтобы посмотреть ассортимент магазина введите - {parameterShowAssortment}");
                Console.WriteLine($"Для того чтобы посмотреть ваши вещи введите - {parameterShowBasket}");
                Console.WriteLine($"Для того чтобы совершать покупки введите - {parameterShop}");
                Console.WriteLine($"Для того чтобы выйти введите - {parameterExit}");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case parameterShowAssortment:
                        seller.ShowProducts();
                        break;

                    case parameterShowBasket:
                        client.ShowProducts();
                        break;

                    case parameterShop:
                        market.MakeDeal();
                        break;

                    case parameterExit:
                        isActive = false;
                        break;

                    default:
                        Console.WriteLine("Некорректный ввод.");
                        break;
                }

                Console.ReadKey();
            }
        }
    }

    public class Deal
    {
        protected int Money;
        protected List<Product> Products;

        public Deal()
        {
            Products = new List<Product>();
        }

        public IReadOnlyCollection<Product> GetProducts => Products;

        public void FillProducts(Product product) => Products.Add(product);

        public virtual void ShowProducts()
        {
            foreach (var product in Products)
                Console.WriteLine($"{product.Name} по цене: {product.Price}.");
        }

        protected int PriceBasket => Products.Sum(product => product.Price);

    }

    public class Client : Deal
    {
        public Client(int money)
        {
            Money = money;
        }

        public bool IsAbleToPay(int price) => price <= Money;

        public void Buy(Product product)
        {
            Money -= PriceBasket;

            Products.Add(product);

            Console.WriteLine($"Вы купили: {product.Name} за {product.Price} условных попугаев.");           
        }

    }

    public class Seller : Deal
    {
        public void AddProducts(List<Product> products) => Products.AddRange(products);

        public (bool, Product) TryGetProduct(string productName)
        {
            foreach (var product in Products)
                if (product.Name.ToLower() == productName.ToLower())
                    return (true, product);

            Console.WriteLine($"Такого товару нету...");

            return (false, null);
        }

        public void Sell(Product product)
        {
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

        public void MakeDeal()
        {
            string userInput;

            Console.WriteLine($"Введите название товара который хотите купить.");

            userInput = Console.ReadLine();

            var TryGetProduct = _seller.TryGetProduct(userInput);

            if (TryGetProduct.Item1)
            {
                if (_client.IsAbleToPay(TryGetProduct.Item2.Price))
                {
                    _seller.Sell(TryGetProduct.Item2);

                    _client.Buy(TryGetProduct.Item2);                   
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
