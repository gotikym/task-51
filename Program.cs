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
        while (_clients.Count > 0)
        {
            Client client = _clients.Dequeue();
            ShowInfoRepair(client.Defect);

            HavePart(client.Defect);            

            if (IsRepair(client))
            {
                if (client.IsSamePart())
                {
                    CloseDeal(client);
                }
                else
                {
                    PayCompensation(client);
                }
            }

            Console.ReadKey();
            Console.Clear();
        }
    }

    private void HavePart(Part defect)
    {
        if (_storage.HavePart(defect))
        {
            Console.WriteLine("Данная деталь есть на складе");
        }
        else
        {
            Console.WriteLine("У нас нет нужной детали на складе");
        }
    }

    private bool IsRepair(Client client)
    {
        const string CommandRefuse = "refuse";
        const string CommandRepair = "repair";
        bool isRepair = false;
        bool isExit = false;
        Console.WriteLine("Отказать клиенту и выплатить штраф - " + CommandRefuse + ", заменить деталь - " + CommandRepair);

        while (isExit == false)
        {
            string userChoice = Console.ReadLine();

            if (userChoice == CommandRefuse)
            {
                Refuse(client);
                isExit = true;
            }
            else if (userChoice == CommandRepair)
            {
                Repair(client);
                isRepair = true;
                isExit = true;
            }
        }

        return isRepair;
    }

    private void Refuse(Client client)
    {
        Console.WriteLine("Клиент недоволен, что потерял своё время, благо есть штраф, который немного его успокоит...");
        PayFine();
        client.TakeMoney(_costFine);
    }

    private void Repair(Client client)
    {        
        Console.WriteLine("Выберете деталь со склада, для замены");
        _storage.ShowParts();
        Part part = _storage.ChoosePart();
        client.AgreeRepair(part);
        _storage.RemovePart(part);
    }

    private void CloseDeal(Client client)
    {
        int costRepair = CalculateCostRepair(client.NewPart);
        Console.WriteLine("Деталь успешна заменена, клиент доволен");
        client.Pay(costRepair);
        TakeMoney(costRepair);
    }

    private void PayCompensation(Client client)
    {
        int compensation = CalculateCompensation(client.Defect, client.NewPart);
        Console.WriteLine("Мастера хотели схитрить или просто запарились в работе и заменили не ту деталь, теперь придется выплачивать компенсацию");
        PayCompensation(compensation);
        client.TakeMoney(compensation);
    }

    private void ShowInfoRepair(Part part)
    {
        int costRepair = part.Cost + _costWork;
        Console.WriteLine("У клиента проблема с " + part.Name + ", стоимость работы будет - " + costRepair);
    }

    private void AddClients()
    {
        _clients.Enqueue(new Client(_storage.CreateDefect()));
        _clients.Enqueue(new Client(_storage.CreateDefect()));
        _clients.Enqueue(new Client(_storage.CreateDefect()));
        _clients.Enqueue(new Client(_storage.CreateDefect()));
        _clients.Enqueue(new Client(_storage.CreateDefect()));
        _clients.Enqueue(new Client(_storage.CreateDefect()));
        _clients.Enqueue(new Client(_storage.CreateDefect()));
        _clients.Enqueue(new Client(_storage.CreateDefect()));
        _clients.Enqueue(new Client(_storage.CreateDefect()));
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
    private static Random _random = new Random();
    private List<Part> _parts = new List<Part>();

    public Storage()
    {
        AddParts();
    }

    public bool HavePart(Part defect)
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
        return _parts[GetNumber(_parts.Count)];
    }

    public Part CreateDefect()
    {
        int firstIndex = 0;
        int lastIndex = _parts.Count;
        int index = _random.Next(firstIndex, lastIndex);
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

    private void AddParts()
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

    private int GetNumber(int listCount)
    {
        bool isParse = false;
        int numberForReturn = 0;

        while (isParse == false)
        {
            string userNumber = Console.ReadLine();
            int.TryParse(userNumber, out numberForReturn);

            if (numberForReturn < 0 || numberForReturn >= listCount)
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
    
    private int _money = 50000;

    public Client(Part defect)
    {
        Defect = defect;
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

    public bool IsSamePart()
    {
        return Defect.Name == NewPart.Name;
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
