/* CSE 598 - Assignment 2 - James & Thao Group Project 
 * Members: James Truong, Thao Vo
 * Class: Program
 * Responsible person: James
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotelBookingSystem
{
    class Program
    {
        public const int NumberOfHotels = 2;
        public const int NumberOfAngencies = 5; // TODO: to be changed back to 5
        private const int ORDER_BUFFER_SIZE = 3;
        private const int CONFIRMATION_ORDER_SIZE = 100;

        static void Main(string[] args)
        {
            // Create a single buffer, which Hotel and TravelAgency objects will store references to
            MultiCellBuffer orderBuffer = new MultiCellBuffer(ORDER_BUFFER_SIZE);
            MultiCellBuffer confirmationBuffer = new MultiCellBuffer(CONFIRMATION_ORDER_SIZE);

            // Create a number of Hotel's. Each Hotel object will have 1 thread instantiated
            Hotel[] hotels = new Hotel[NumberOfHotels];
            int[] star = new int[] { 5, 4, 3, 2, 1 };
            int[] totalRooms = new int[] { 10, 20, 30, 40, 50 };
            for (int i = 0; i < NumberOfHotels; i++)
            {
                hotels[i] = new HotelBookingSystem.Hotel(i + 100, star[i], totalRooms[i], orderBuffer, confirmationBuffer);
            }

            Thread[] hotelThreads = new Thread[NumberOfHotels];
            for (int i = 0; i < NumberOfHotels; i++)
            {
                hotelThreads[i] = new Thread(new ThreadStart(hotels[i].hotelFunction));
                hotelThreads[i].Name = "Hotel " + (i + 1).ToString();
                hotelThreads[i].Start(); //start hotel thread
            }

            // Create a number of TravelAgency's. Each TravelAgency object will have 1 thread instantiated
            TravelAgency[] agency = new TravelAgency[NumberOfAngencies];
            for (int i = 0; i < NumberOfAngencies; i++)
            {
                agency[i] = new HotelBookingSystem.TravelAgency(orderBuffer, confirmationBuffer);
            }

            // Subscribe all the TravelAgency objects to all the Hotel objects
            for (int i = 0; i < NumberOfHotels; i++)
            {
                for (int j = 0; j < NumberOfAngencies; j++)
                {
                    // Store the handler to the Hotel class
                    hotels[i].priceCut += new priceCutEvent(agency[j].hotelOnSaleHandler);
                    agency[j].registerHotelId(hotels[i].myId); // Register the hotels that the Agency subscribes to, used later to terminate Agency thread
                    hotels[i].terminateSubscribers += new terminateSubscriberEvent(agency[j].threadTerminateHandler);
                }
            }

            // Create Agency objects
            Thread[] agencyThreads = new Thread[NumberOfAngencies];
            for (int i = 0; i < NumberOfAngencies; i++)
            {
                //Start N Retailer threads
                agencyThreads[i] = new Thread(new ThreadStart(agency[i].agencyFunction));
                agencyThreads[i].Name = "TravelAgency " + (i + 1).ToString();
                agencyThreads[i].Start();
            }
            Console.ReadLine();     
        }
    }

    class DebugConsole
    {
        //Change DEBUG to true to initiate and see how the program is running
        //Change DEBUT back to false to keep a clean interface
        //FALSE will be our default value to keep a clean console result!
        public const bool DEBUG = false;
        public static void WriteLine(string str)
        {
            if (DEBUG)
                Console.WriteLine(str);
        }
    }
}
