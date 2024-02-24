using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualShop
{
    internal class Program
    {
        private const int ParameterShowAssortment = 1;
        private const int ParameterShowBasket = 2;
        private const int ParameterShop = 3;
        private const int ParameterExit = 4;

        static void Main()
        {
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

                Console.WriteLine($"Для того чтобы посмотреть ассортимент магазина введите - {ParameterShowAssortment}");
                Console.WriteLine($"Для того чтобы посмотреть ваши вещи введите - {ParameterShowBasket}");
                Console.WriteLine($"Для того чтобы совершать покупки введите - {ParameterShop}");
                Console.WriteLine($"Для того чтобы выйти введите - {ParameterExit}");

                string userInput = Console.ReadLine();

                int.TryParse(userInput, out int inputNumber);

                switch (inputNumber)
                {
                    case ParameterShowAssortment:
                        seller.ShowProducts();
                        break;

                    case ParameterShowBasket:
                        client.ShowInventory();
                        break;

                    case ParameterShop:
                        market.MakeDeal();
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
    }

    public class Deal
    {
        private readonly List<Product> _basket;

        public Deal()
        {
            _basket = new List<Product>();
        }

        public void FillBasket(Product product) => _basket.Add(product);

        protected int PriceBasket => Basket.Sum(product => product.Price);

        protected void ClearBasket() => _basket.Clear();

        protected IReadOnlyCollection<Product> Basket => _basket;

    }

    public class Client : Deal
    {
        private readonly List<Product> _inventory;
        private int _money;

        public Client(int money)
        {
            _inventory = new List<Product>();
            _money = money;
        }

        public bool IsAbleToPay => PriceBasket <= _money;

        public void BuyProducts(IReadOnlyCollection<Product> basket)
        {
            if (basket.Count > 0)
            {               
                _money -= PriceBasket;

                _inventory.AddRange(basket);

                Console.WriteLine($"Вы купили: ");

                foreach (var product in basket)
                    Console.WriteLine($"{product.Name} по цене: {product.Price}.");
            }
            else
            {
                Console.WriteLine("В корзине клиента нет товара.");
            }
        }

        public void ShowInventory()
        {
            foreach (var product in _inventory)
                Console.WriteLine($"{product.Name} по цене: {product.Price}.");
        }
    }

    public class Seller : Deal
    {
        private int _earnedMoney;
        private List<Product> _products;

        public Seller()
        {
            _products = new List<Product>();
            _earnedMoney = 0;
        }

        public IReadOnlyCollection<Product> Products => _products;

        public void AddProducts(List<Product> products) => _products.AddRange(products);

        public void SellProducts(Client client)
        {            
            if (client.IsAbleToPay)
            {
                _earnedMoney += PriceBasket;

                _products = _products.Except(Basket).ToList();

                client.BuyProducts(Basket);

                ClearBasket();
            }
            else
            {
                Console.WriteLine("У клиента недостаточно денег, чтобы купить товар.");               
            }
        }

        public void ShowProducts()
        {
            foreach (var product in Products)
                Console.WriteLine($"{product.Name} по цене: {product.Price}.");
        }
    }

    public class Market
    {
        private const string ContinueChoppingCommand = "Купить";

        private readonly Seller _seller;
        private readonly Client _client;

        public Market(Seller seller, Client client)
        {
            _seller = seller;
            _client = client;
        }

        public void MakeDeal()
        {
            bool canPurchase = true;
            string userInput;

            while (canPurchase)
            {
                Console.WriteLine($"Для того чтобы положить продукт в корзину введите его название.");

                userInput = Console.ReadLine();

                PutInBasket(_seller, userInput);

                Console.WriteLine($"Если хотите продолжить покупки напишите '{ContinueChoppingCommand}' или нажмите 'Enter'.");

                userInput = Console.ReadLine();

                if (userInput.ToLower() == ContinueChoppingCommand.ToLower())
                    canPurchase = false;
            }

            _seller.SellProducts(_client);
           
        }

        private void PutInBasket(Seller seller, string userInput)
        {
            foreach (var product in seller.Products)
            {
                if (product.Name.ToLower() == userInput.ToLower())
                {
                    seller.FillBasket(product);

                    Console.WriteLine($"{product.Name} добавлен в корзину.");
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
