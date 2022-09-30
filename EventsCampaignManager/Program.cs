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
            var nullCustomer = new Customer();

            // Question 1: Call #GetEventsWithinCustomerCity to get the list of events to send out
            Console.WriteLine("Events within customer city:");
            try
            {
                var eventsWithinCustomerCity = GetEventsWithinCustomerCity(customer, events);

                // Call #SendEmails that abstracts away the email sending logic
                SendEmails(customer, eventsWithinCustomerCity);
            }
            catch (Exception)
            {
                Console.WriteLine($"Error processing request, please check call stack for exception details.");
            }

            // Question 2: Call #GetFiveClosentsEvents to get the list of five closest events
            Console.WriteLine("Five closest events:");
            try
            {
                var fiveClosestEvents = GetFiveClosestEvents(customer, events);

                // Call #SendEmails that abstracts away the email sending logic
                SendEmails(customer, fiveClosestEvents);
            }
            catch (Exception)
            {
                Console.WriteLine($"Error processing request, please check call stack for exception details.");
            }

            // Question 4: Call #GetMostPlausibleEvents to get the list of the most plausible events the customer can attend
            Console.WriteLine("Most plausible events:");
            try
            {
                var mostPlausibleEvents = GetMostPlausibleEvents(customer, events);

                // Call #SendEmails that abstracts away the email sending logic
                SendEmails(customer, mostPlausibleEvents);
            }
            catch (Exception)
            {
                Console.WriteLine($"Error processing request, please check call stack for exception details.");
            }

            // Question 5: Call #GetFiveMostAffordableEvents to get the list of five closed
            Console.WriteLine("Five most affordable events:");
            try
            {
                var fiveMostAffordableEvents = GetFiveMostAffordableEvents(events);

                // Call #SendEmails that abstracts away the email sending logic
                SendEmails(customer, fiveMostAffordableEvents);
            }
            catch (Exception)
            {
                Console.WriteLine($"Error processing request, please check call stack for exception details.");
            }
        }

        // This fuction loops through a list of events calling #AddToEmail for each
        static void SendEmails(Customer customer, List<Event> events, int? price = null)
        {
            for(int i = 0; i < events.Count; i++ )
            {
                AddToEmail(customer, events[i], price);
            }
        }

        // This function validates a customer object
        static (string, string) ValidateCustomer(Customer customer)
        {
            if(string.IsNullOrEmpty(customer.Name))
            {
                throw new ArgumentException(message: $"{nameof(customer.Name)} is null!");
            }
            else if(string.IsNullOrEmpty(customer.City))
            {
                throw new ArgumentException(message: $"{nameof(customer.City)} is null!");
            }

            return (customer.Name, customer.City);
        }

        // This function validates an event object
        static (string, string) ValidateEvent(Event coolEvent)
        {
            if(string.IsNullOrEmpty(coolEvent.Name))
            {
                throw new ArgumentException(message: $"{nameof(coolEvent.Name)} is null!");
            }
            else if(string.IsNullOrEmpty(coolEvent.City))
            {
                throw new ArgumentException(message: $"{nameof(coolEvent.City)} is null!");
            }

            return (coolEvent.Name, coolEvent.City);
        }

        // This function returns events by city using a linq query that uses a lambda expression
        static List<Event> EventsByCity(string city, List<Event> events)
        {
        return events.Where(e => e.City == city).ToList();
        }

        // This function returns events by name and city using a linq query that uses a lambda expression
        static List<Event> EventsByNameAndCity(string[] nameAndCity, List<Event> events)
        {
            string name = nameAndCity[0], city = nameAndCity[1];
            return events.Where(e => e.Name == name && e.City == city).ToList();
        }

        // This function filters a list of events and returns all events occuring within the customer's city
        static List<Event> GetEventsWithinCustomerCity(Customer customer, List<Event> events)
        {
            try
            {
                // There might be null values passed in, we need to validate
                (string, string) validCustomer = ValidateCustomer(customer);

                // Call events by city that abstracts the filtering
                return EventsByCity(validCustomer.Item2, events);
            }
            catch (Exception ex)
            {
                if(ex.Message.EndsWith("is null!"))
                {
                    Console.WriteLine($"An error occured during customer validation. Error message: {ex.Message}");
                }
                else
                {
                    Console.WriteLine($"An error occured while querying events by city. Error message: {ex.Message}");
                }
            }
        }

        // This function returns five events happening around the customer, including in nearby cities
        static List<Event> GetFiveClosestEvents(Customer customer, List<Event> events)
        {
            // Call #GetDistance storing the distances to the various cities in an ordered dictionary
            SortedDictionary<int, string> eventDistances = new SortedDictionary<int, string>();

            // Validate customer city
            string fromCity = string.Empty;

            try
            {
                (string, string) customerCity = ValidateCustomer(customer);
                fromCity = customerCity.Item2;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured during customer validation. Error message: {ex.Message}");
                throw;
            }

            foreach(Event coolEvent in events)
            {
                try
                {
                    // Validate event city
                    (string, string) eventCity = ValidateEvent(coolEvent);
                    string toCity = eventCity.Item2;
                    int distance = GetDistance(fromCity, toCity);

                    if(!eventDistances.ContainsKey(distance))
                    {
                        eventDistances.Add(distance, eventCity.Item2);
                    }           
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while attempting to calculate city distances. Error message: {ex.Message}");
                    throw;
                }
            } 

            // Loop through the ordered dictionary retrieving event city names for events lookup
            List<Event> orderedEvents = new List<Event>();

            foreach(KeyValuePair<int, string> entry in eventDistances)
            {
                var orderedCityEvents = EventsByCity(entry.Value, events);
                orderedEvents.AddRange(orderedCityEvents);
            }
      
            return orderedEvents.Take(5).ToList();
        }

        static List<Event> GetMostPlausibleEvents(Customer customer, List<Event> events)
        {
            // The priority is to get the top 5 closest events
            try
            {
                return GetFiveClosestEvents(customer, events);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while retrieving events by proximity. Error message: {ex.Message}");
                throw;
            }
            finally
            {
                // We can't get 5 closet events including those in other cities, so default to events within the same city
                GetEventsWithinCustomerCity(customer, events);
            }
        }

        // This functions returns five most affordable events irrespective of the distance from the customer
        static List<Event> GetFiveMostAffordableEvents(List<Event> events)
        {
            // For each event, populate their prices in a sorted list, then use it to lookup the most affordable events
            // We are using a sorted list because several events may have the same prices
            SortedList<int, string> sortedEventPrices = new SortedList<int, string>();

            // Loop through the list of events populating the dictionary
            foreach(var coolEvent in events)
            {
                try
                {
                    // Validate event
                    (string, string) eventName = ValidateEvent(coolEvent);
                    string thisEventName = eventName.Item1, thisEventCity = eventName.Item2;
                    int eventPrice = GetPrice(coolEvent);

                    sortedEventPrices.Add(eventPrice, $"{thisEventName}|{thisEventCity}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while attempting to calculate event price. Error message: {ex.Message}");
                    throw;
                }
            }

            // Loop through the sorted list retrieving event city names for events lookup
            List<Event> orderedPrices = new List<Event>();

            foreach(KeyValuePair<int, string> entry in sortedEventPrices)
            {
                string[] eventDetails = entry.Value.Split('|');
                var orderedPriceEvents = EventsByNameAndCity(eventDetails, events);

                orderedPrices.AddRange(orderedPriceEvents);
            }
      
            return orderedPrices.Take(5).ToList();
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

