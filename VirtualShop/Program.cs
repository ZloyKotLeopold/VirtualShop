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

            Market market = Market.Initialize();
            PurchaseHandler purchaseHandler = PurchaseHandler.Initialize();

            market.AddProducts(new List<Product>
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
                        client.ShowProducts();
                        break;

                    case ParameterShop:
                        purchaseHandler.Buy(seller, client);
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

    public interface IShower
    {
        void ShowProducts();
    }

    public class PurchaseHandler
    {
        private const string ContinueChopping = "Купить";

        static public PurchaseHandler HandlerSingle = null;

        protected PurchaseHandler() { }

        static public PurchaseHandler Initialize()
        {
            if (HandlerSingle == null)
                HandlerSingle = new PurchaseHandler();

            return HandlerSingle;
        }

        public void Buy(Seller seller, Client client)
        {
            bool canPurchase = true;
            string userInput;

            while (canPurchase)
            {
                Console.WriteLine($"Для того чтобы положить продукт в корзину введите его название.");

                userInput = Console.ReadLine();

                PutInBasket(seller, client, userInput);

                Console.WriteLine($"Если хотите продолжить покупки напишите '{ContinueChopping}' или нажмите 'Enter'.");

                userInput = Console.ReadLine();

                if (userInput.ToLower() == ContinueChopping.ToLower())                
                    canPurchase = false;               
            }

            client.BuyProducts(seller);
        }

        private void PutInBasket(Seller seller, Client client, string userInput)
        {
            foreach (var product in seller.Products)
            {
                if (product.Name.ToLower() == userInput.ToLower())
                {
                    client.AddToBasket(product);

                    Console.WriteLine($"{product.Name} добавлен в корзину.");
                }
            }
        }

    }

    public class Client : Market, IShower
    {
        private readonly List<Product> _inventory;
        private int _money;

        public Client(int money)
        {
            _inventory = new List<Product>();
            _money = money;
        }

        public int Money => _money;

        public IReadOnlyCollection<Product> Inventory => _inventory;

        public void AddToBasket(Product product) => FillBasket(product);

        public void BuyProducts(Seller seller)
        {
            int priceBasket = Basket.Sum(product => product.Price);

            if (_money >= priceBasket)
            {
                seller.SellProducts(priceBasket);

                _money -= priceBasket;

                _inventory.AddRange(Basket);

                ClearBasket();
            }
            else
            {
                Console.WriteLine("У клиента недостаточно денег, чтобы купить товар.");
            }
        }

        public void ShowProducts()
        {
            foreach (var product in _inventory)
                Console.WriteLine($"{product.Name} по цене: {product.Price}.");
        }
    }

    public class Seller : Market, IShower
    {
        private int _earnedMoney;

        public Seller()
        {
            _earnedMoney = 0;
        }

        public int EarnedMoney => _earnedMoney;

        public void SellProducts(int priceBasket)
        {
            if (priceBasket > 0)
            {
                _earnedMoney += priceBasket;

                RemoveProducts();
            }
            else
            {
                Console.WriteLine("В корзине клиента нет товара.");
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
        static public Market MarketSingle = null;
        static private List<Product> _basket;
        static private List<Product> _products;

        protected Market() { }
        
        static public Market Initialize()
        {
            if (MarketSingle == null)            
                MarketSingle = new Market();
            
            _products = new List<Product>();
            _basket = new List<Product>();

            return MarketSingle;
        }

        public void AddProducts(List<Product> products) => _products.AddRange(products);

        public IReadOnlyCollection<Product> Products => _products;

        protected void ClearBasket() => _basket.Clear();

        protected IReadOnlyCollection<Product> Basket => _basket;

        protected void RemoveProducts() => _products = _products.Except(_basket).ToList();

        protected void FillBasket(Product product) => _basket.Add(product);

    }

    public class Product
    {
        public string Name { get; private set; }
        public int Price { get; private set; }

        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public void ShowProduct()
        {
            Console.WriteLine($"{Name}\nЦена: {Price}\n");
        }
    }
}
