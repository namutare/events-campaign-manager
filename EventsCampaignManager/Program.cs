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

            /*Question 1: Call #GetEventsWithinCustomerCity that abstracts the business logic for getting events within the customer's city
             * To send the events to the customer, call #SendEvents that abstracts the event sending business logic
             */
            Console.WriteLine("Question 1 solution:");
            var eventsWithinCustomerCity = GetEventsWithinCustomerCity(customer, events);
            SendEvents(customer, eventsWithinCustomerCity);

            /*Question 2: Call #GetFiveClosestEvents that abstracts the business logic for getting top 5 events closest to the customer
             * To send the events to the customer, call #SendEvents that abstracts the event sending business logic
             */
            Console.WriteLine("Question 2 solution:");
            var fiveClosestEvents = GetFiveClosestEvents(customer, events);
            SendEvents(customer, fiveClosestEvents);

            // Question 3: Check dictionary implementation in #GetFiveClosestEvents

            /*Question 4: Call #GetMostPlausibleEvents that abstracts the business logic for getting the most plausible events closest to the customer
             * This is in the event the #GetDistance function fails
            * To send the events to the customer, call #SendEvents that abstracts the event sending business logic
            */
            Console.WriteLine("Question 4 solution:");
            var mostPlausibleEvents = GetMostPlausibleEvents(customer, events);
            SendEvents(customer, mostPlausibleEvents);

            /*Question 5: Call #GetFiveMostAffordableEvents that abstracts the business logic for getting top 5 most affordable events
             * To send the events to the customer, call #SendEvents that abstracts the event sending business logic
             */
            Console.WriteLine("Question 5 solution:");
            var fiveMostAffordableEvents = GetFiveMostAffordableEvents(customer, events);
            SendEvents(customer, fiveMostAffordableEvents);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        // This function validates the customer city
        static string ValidateCustomerCity(Customer customer)
        {
            // Check for nulls to prevent runtime exceptions
            string customerCity = customer != null ? customer.City : string.Empty;

            if (string.IsNullOrEmpty(customerCity))
            {
                return "Invalid";
            }
            else
            {
                return customerCity;
            }
        }

        // This function returns events by city name using a linq query with lambda expression
        static List<Event> EventsByCity(string city, List<Event> events)
        {
            return events.Where(e => e.City == city).ToList();
        }

        // This function loops through events, calling #AddToEmail to send the notification to the customer
        static void SendEvents(Customer customer, List<Event> events)
        {
            // Loop through the list of events and for each item, call the #AddToEmail method

            for (int i = 0; i < events.Count; i++)
            {
                AddToEmail(customer, events[i]);
            }
        }

        /* This function takes as input a customer and a list of events that it matches
         * The criteria used here is that the events must be within the customer's city
         * And returns all the matches found
         */
        static List<Event> GetEventsWithinCustomerCity(Customer customer, List<Event> events)
        {
            // Call #EventsByCity to find all events in the customer's location 
            string customerCity = ValidateCustomerCity(customer);

            if (string.IsNullOrEmpty(customerCity))
            {
                Console.WriteLine("Customer city invalid!");
                return new List<Event>();
            }

            return EventsByCity(customerCity, events);
        }

        /* This function takes as input a customer and a list of events that it matches
         * It calls #GetDistance to get the distance between the customer and the city within which the event will occur
         * It then filters and returns the 5 closest events
         */
        static List<Event> GetFiveClosestEvents(Customer customer, List<Event> events)
        {
            //1. From the customer's city, calculate the distance to the event and store the values in a dictionary, and the keys in an array
            // The dictionary is part of the optimization that answers Question 3
            string customerCity = ValidateCustomerCity(customer);

            if (string.IsNullOrEmpty(customerCity))
            {
                Console.WriteLine("Customer city invalid!");
                new List<Event>();
            }

            Dictionary<int, string> eventDistances = new Dictionary<int, string>();
            int[] distances = new int[events.Count];

            for (int i = 0; i < events.Count; i++)
            {
                int distance = GetDistance(customerCity, events[i].City);
                distances[i] = distance;

                if (!eventDistances.ContainsKey(distance))
                {
                    eventDistances.Add(distance, events[i].City);
                }
            }

            //2. Sort the distances in ascending order, then loop through them to pull top 5 events from the cities in the dictionary
            Array.Sort(distances);

            int eventCount = 0;
            string lastCity = string.Empty;
            List<Event> closestEvents = new List<Event>();
            for (int i = 0; i < distances.Length; i++)
            {
                int sortedDistance = distances[i];
                string city = eventDistances[sortedDistance];

                if (lastCity != city)
                {
                    List<Event> thisCityEvents = EventsByCity(city, events);

                    eventCount += thisCityEvents.Count;
                    closestEvents.AddRange(thisCityEvents);

                    if (eventCount >= 5)
                    {
                        break;
                    }
                }

                lastCity = city;
            }

            //3. Return the top 5 events from the closest events list
            return closestEvents.Take<Event>(5).ToList();
        }

        /* The #GetFiveClosestEvents implements #GetDistance, if for some reason the #GetDistance function fails
         * This function wraps #GetFiveClosestEvents into a try catch block, and will default to events within the customer's city if #GetFiveClosestEvents fails
         * Due to the upstream error in #GetDistance
         */
        static List<Event> GetMostPlausibleEvents(Customer customer, List<Event> events)
        {
            // The priority is to get the top 5 closest events
            try
            {
                return GetFiveClosestEvents(customer, events);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while retrieving events by proximity by distance. Error message: {ex.Message}");
                throw;
            }
            finally
            {
                // We can't get 5 closet events including those in other cities, so default to events within the same city
                GetEventsWithinCustomerCity(customer, events);
            }
        }

        /* This function sorts the events by price and returns the top 5 most affordable events to the customer
         */
        static List<Event> GetFiveMostAffordableEvents(Customer customer, List<Event> events)
        {
            // 1. Loop through the events calling #GetPrice and then store the values in a dictionary against each event
            string customerCity = ValidateCustomerCity(customer);

            if (string.IsNullOrEmpty(customerCity))
            {
                Console.WriteLine("Customer city invalid!");
                return new List<Event>();
            }

            Dictionary<int, string> eventPrices = new Dictionary<int, string>();
            int[] prices = new int[events.Count];

            for (int i = 0; i < events.Count; i++)
            {
                int price = GetPrice(events[i]);
                prices[i] = price;

                if (!eventPrices.ContainsKey(price))
                {
                    eventPrices.Add(price, events[i].Name);
                }
            }

            //2. Sort the prices in ascending order, then loop through them to pull top 5 events from the event names in the dictionary
            Array.Sort(prices);

            int eventCount = 0;
            List<Event> mostAffordableEvents = new List<Event>();
            for (int i = 0; i < prices.Length; i++)
            {
                int sortedPrice = prices[i];
                string eventName = eventPrices[sortedPrice];
                List<Event> thisNameEvents = events.Where(e => e.Name == eventName).ToList();

                eventCount += thisNameEvents.Count;
                mostAffordableEvents.AddRange(thisNameEvents);

                if (eventCount >= 5)
                {
                    break;
                }
            }

            //3. Return the top 5 events from the most affordable events list
            return mostAffordableEvents.Take<Event>(5).ToList();
        }

        /* =================================================================================================================================
         * You do not need to know how these methods work
         * =================================================================================================================================
         */
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
            //return 0 / 3;
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

