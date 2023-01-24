using System;
using System.Collections.Generic;

internal class Program
{
    static void Main(string[] args)
    {
        Autoservice autoservice = new Autoservice();
        autoservice.Work();
    }
}

class Autoservice
{
    private Storage _storage = new Storage();
    private Queue<Client> _clients = new Queue<Client>();
    private int _money = 50000;
    private int _costWork = 1500;
    private int _costFine = 700;

    public Autoservice()
    {
        AddClients();
    }

    public void Work()
    {
        const string CommandRefuse = "refuse";
        const string CommandRepair = "repair";
        bool isRepair = false;
        bool isExit = false;

        while (_clients.Count > 0)
        {
            Client client = _clients.Dequeue();
            ShowInfoRepair(client.Defect);

            if (_storage.FindPart(client.Defect))
            {
                Console.WriteLine("Данная деталь есть на складе");
            }
            else
            {
                Console.WriteLine("У нас нет нужной детали на складе");
            }

            Console.WriteLine("Отказать клиенту и выплатить штраф - " + CommandRefuse + ", заменить деталь - " + CommandRepair);

            while (isExit == false)
            {
                string userChoice = Console.ReadLine();

                if (userChoice == CommandRefuse)
                {
                    Console.WriteLine("Клиент недоволен, что потерял своё время, благо есть штраф, который немного его успокоит...");
                    PayFine();
                    client.TakeMoney(_costFine);
                    isExit = true;
                }
                else if (userChoice == CommandRepair)
                {
                    Console.WriteLine("Выберете деталь со склада, для замены");
                    _storage.ShowParts();
                    Repair(client, _storage.ChoosePart());
                    isRepair = true;
                    isExit = true;
                }
            }

            if (isRepair == true)
            {
                if (client.VerifyPart())
                {
                    int costRepair = CalculateCostRepair(client.NewPart);
                    Console.WriteLine("Деталь успешна заменена, клиент доволен");
                    client.Pay(costRepair);
                    TakeMoney(costRepair);
                }
                else
                {
                    int compensation = CalculateCompensation(client.Defect, client.NewPart);
                    Console.WriteLine("Мастера хотели схитрить или просто запарились в работе и заменили не ту деталь, теперь придется выплачивать компенсацию");
                    PayCompensation(compensation);
                    client.TakeMoney(compensation);
                }
            }

            Console.ReadKey();
            Console.Clear();
            isExit = false;
            isRepair = false;
        }
    }

    private void Repair(Client client, Part part)
    {
        client.AgreeRepair(part);
        _storage.RemovePart(part);
    }

    private void ShowInfoRepair(Part part)
    {
        int costRepair = part.Cost + _costWork;
        Console.WriteLine("У клиента проблема с " + part.Name + ", стоимость работы будет - " + costRepair);
    }

    private void AddClients()
    {
        _clients.Enqueue(new Client());
        _clients.Enqueue(new Client());
        _clients.Enqueue(new Client());
        _clients.Enqueue(new Client());
        _clients.Enqueue(new Client());
        _clients.Enqueue(new Client());
        _clients.Enqueue(new Client());
    }

    private int CalculateCostRepair(Part part)
    {
        return _costWork + part.Cost;
    }

    private int CalculateCompensation(Part oldPart, Part newPart)
    {
        return oldPart.Cost + newPart.Cost;
    }

    private void TakeMoney(int costRepair)
    {
        _money += costRepair;
    }

    private void PayCompensation(int compensation)
    {
        _money -= compensation;
    }

    private void PayFine()
    {
        _money -= _costFine;
    }
}

class Storage
{
    private List<Part> _parts = new List<Part>();

    public Storage()
    {
        AddParts();
    }

    public bool FindPart(Part defect)
    {
        foreach (Part part in _parts)
        {
            if (defect.Name == part.Name)
                return true;
        }

        return false;
    }

    public void RemovePart(Part part)
    {
        _parts.Remove(part);
    }

    public Part ChoosePart()
    {
        return _parts[GetNumber(_parts.Capacity)];
    }

    public Part CreateDefect(int index)
    {
        return new Part(_parts[index].Name, _parts[index].Cost);
    }

    public void ShowParts()
    {
        int numberPart = 0;

        foreach (Part part in _parts)
        {
            Console.WriteLine(numberPart + " - " + part.Name + " стоимость: " + part.Cost);
            numberPart++;
        }
    }

    public int GetCount()
    {
        return _parts.Count;
    }

    public void AddParts()
    {
        _parts.Add(new Part("Амортизатор", 3500));
        _parts.Add(new Part("Стабилизатор", 2500));
        _parts.Add(new Part("Сайлентблок", 500));
        _parts.Add(new Part("Ступица", 4250));
        _parts.Add(new Part("Пыльник", 1200));
        _parts.Add(new Part("Рессор", 4500));
        _parts.Add(new Part("Редуктор", 14300));
        _parts.Add(new Part("Рулевой наконечник", 1250));
        _parts.Add(new Part("Лобовое стекло", 35000));
        _parts.Add(new Part("Брызговик", 350));
        _parts.Add(new Part("Дворники", 2000));
        _parts.Add(new Part("Фара", 32000));
    }

    private int GetNumber(int listCapacity)
    {
        bool isParse = false;
        int numberForReturn = 0;

        while (isParse == false)
        {
            string userNumber = Console.ReadLine();
            int.TryParse(userNumber, out numberForReturn);

            if (numberForReturn <= 0 || numberForReturn > listCapacity)
            {
                Console.WriteLine("Вводи одну из предложенных цифр, а не из космоса =_=");
            }
            else
            {
                isParse = true;
            }
        }

        return numberForReturn;
    }
}

class Client
{
    private static Random _random = new Random();
    private int _money = 50000;

    public Client()
    {
        FindDefect();
    }

    public Part Defect { get; private set; }
    public Part NewPart { get; private set; }

    public void TakeMoney(int compensation)
    {
        _money += compensation;
    }

    public void Pay(int costRepair)
    {
        _money -= costRepair;
    }

    public void AgreeRepair(Part part)
    {
        NewPart = part;
    }

    public bool VerifyPart()
    {
        return Defect.Name == NewPart.Name;
    }

    private void FindDefect()
    {
        Storage storage = new Storage();
        int firstIndex = 0;
        int lastIndex = storage.GetCount();
        Defect = storage.CreateDefect(_random.Next(firstIndex, lastIndex));
    }
}

class Part
{
    public Part(string name, int cost)
    {
        Name = name;
        Cost = cost;
    }

    public string Name { get; private set; }
    public int Cost { get; private set; }
}