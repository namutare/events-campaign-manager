using System;
using System.Collections.Generic;
using System.Linq;

namespace EventsCampaignManager
{
    public class Event
    {
        public string Name { get; set; }
        public string City { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var events = new List<Event>{
                    new Event{ Name = "Phantom of the Opera", City = "New York"},
                    new Event{ Name = "Metallica", City = "Los Angeles"},
                    new Event{ Name = "Metallica", City = "New York"},
                    new Event{ Name = "Metallica", City = "Boston"},
                    new Event{ Name = "LadyGaGa", City = "New York"},
                    new Event{ Name = "LadyGaGa", City = "Boston"},
                    new Event{ Name = "LadyGaGa", City = "Chicago"},
                    new Event{ Name = "LadyGaGa", City = "San Francisco"},
                    new Event{ Name = "LadyGaGa", City = "Washington"}
                };

            var customer = new Customer { Name = "Mr. Fake", City = "New York" };

            // Question 1: Call our custom #SendEventsWithinCustomerCity method
            SendEventsWithinCustomerCity(customer, events);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /* This function takes as input a customer and a list of events that it matches
         * The criteria used here is that the events must be within the customer's city
         * When matches are found, the function will call #AddToEmail method to notify the customer
         */
        static void SendEventsWithinCustomerCity(Customer customer, List<Event> events)
        {
            //1. Find all events in the customer's location using a query with lambda expression

            // Check for nulls to prevent runtime exceptions
            string customerCity = customer != null ? customer.City : string.Empty;

            if (string.IsNullOrEmpty(customerCity))
            {
                Console.WriteLine("Customer city invalid!");
                return;
            }

            var eventsInCustomerLocation = events.Where(e => e.City == customerCity).ToList();

            //2. Loop through the list of events in the customer location in the filtered list above and for each item, call the #AddToEmail method

            for (int i = 0; i < eventsInCustomerLocation.Count; i++)
            {
                AddToEmailQ1(customer, eventsInCustomerLocation[i]);
            }
        }

        static void AddToEmailQ1(Customer c, Event e)
        {
            Console.WriteLine($"{c.Name}: {e.Name} in {e.City}");
        }

        // You do not need to know how these methods work
        static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}" + (distance > 0 ? $"({distance} miles away)" : "") + (price.HasValue? $" for ${price}" : ""));
        }
        static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }
        static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }
        private static int AlphebiticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }
    }
}

