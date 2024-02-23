using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static VirtualShop.Program;

namespace VirtualShop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int ParameterShowAssortment = 1;
            const int ParameterShowBasket = 2;
            const int ParameterShop = 3;
            const int ParameterExit = 4;

            string userInput;
            int money = 500;
            bool isActive = true;

            Seller seller = new Seller();
            Client client = new Client(money);

            seller.BringProducts(new List<Product>
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
                Console.WriteLine($"Для того чтобы посмотреть корзину введите - {ParameterShowBasket}");
                Console.WriteLine($"Для того чтобы совершать покупки введите - {ParameterShop}");
                Console.WriteLine($"Для того чтобы выйти введите - {ParameterExit}");

                userInput = Console.ReadLine();

                if (int.TryParse(userInput, out int inputNumber))
                {
                    switch (inputNumber)
                    {
                        case ParameterShowAssortment:
                            ShowProducts(seller.Products);
                            break;

                        case ParameterShowBasket:
                            ShowProducts(client.Basket);

                            if (client.Basket.Count == 0)
                                Console.WriteLine("В корзине нет товара.\n");
                            break;

                        case ParameterShop:
                            FillBasket(client, seller);
                            client.BuyProduct();
                            break;

                        case ParameterExit:
                            isActive = false;
                            break;

                        default:
                            Console.WriteLine("Неверное значение.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Вы ввели неверное значение.");
                }

                Console.ReadKey();
            }
        }

        static private void ShowProducts(IReadOnlyCollection<Product> products)
        {
            foreach (var product in products)
                product.ShowProduct();
        }

        static private void FillBasket(Client client, Seller seller)
        {
            const string ParameterStopFilling = "Хватит";

            string userInput;
            bool isFiiling = true;

            while (isFiiling)
            {
                ShowProducts(seller.Products);

                Console.WriteLine("Если хотите положть товар в корзину введите имя товара.");
                Console.WriteLine($"Если хотите продолжить покупки введите - {ParameterStopFilling}");

                userInput = Console.ReadLine();

                if (userInput == ParameterStopFilling)
                {
                    isFiiling = false;
                }
                else
                {
                    try
                    {
                        client.AddInBasket(seller.Products.Where(product => product.Name.Contains(userInput)).First());

                        Console.WriteLine($"Вы положили в корзину {userInput}.");
                    }
                    catch
                    {
                        Console.WriteLine("Такого товара нет, либо неверно ввели название.");
                    }
                }

                Console.ReadKey();
            }
        }

    }

    public class Client
    {
        private int _money;
        private int _priceBasket;
        private List<Product> _basket;

        public Client(int money)
        {
            _money = money;
            _basket = new List<Product>();
        }

        public void AddInBasket(Product product) => _basket.Add(product);

        public void BuyProduct()
        {
            if (_basket.Count > 0)
            {
                foreach (var product in _basket)
                {
                    _priceBasket += product.Price;
                }

                if (_priceBasket <= _money)
                {
                    _money -= _priceBasket;
                }
                else
                {
                    _basket.Clear();

                    Console.WriteLine("У вас недостаточно денег чтоб купить товар.");
                }
            }
            else
            {
                Console.WriteLine("В вашей корзине нет товара.");
            }
        }

        public IReadOnlyCollection<Product> Basket => _basket;
    }

    public class Seller
    {
        private List<Product> _products;

        public Seller()
        {
            _products = new List<Product>();
        }

        public void BringProducts(List<Product> product) => _products.AddRange(product);

        public void SellProducts(Client client)
        {
            client.BuyProduct();

            if (client.Basket.Count > 0)
                _products.RemoveAll(product => client.Basket.Contains(product));
        }

        public IReadOnlyCollection<Product> Products => _products;
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
