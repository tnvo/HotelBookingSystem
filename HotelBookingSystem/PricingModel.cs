/* CSE 598 - Assignment 2 - James & Thao Group Project
 * Members: James Truong, Thao Vo
 * Class: PricingModel
 * Side: Server
 * Responsible person: Thao
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingSystem
{
    class PricingModel
    {
        //inititate minimum and maximum price for the hotel
        public const int MAX_PRICE = 500;
        public const int MIN_PRICE = 50;

        //initiate some variables that could affect the hotel price
        private int star;
        private int totalRooms;
        private bool minReached;
        private static Random rng = new Random(DateTime.Now.Millisecond); // generate random numbers

        public PricingModel(int star, int totalRooms)
        {
            this.star = star;
            // Make sure total of rooms is not 0
            if (totalRooms == 0)
                this.totalRooms = 1;
            else
                this.totalRooms = totalRooms;
            minReached = false;
        }

        public double getPrice(int availableRooms)
        { // Price is from $50 to $500
            double price = 0;

            //generate random price for the higher price range hotels
            if ((star >= 2) && (star <= 5))
            {
                price = rng.Next(star * 100 - 100, star * 100);
            }
            else //the lower price ranges due to lower star rating
            {
                price = rng.Next(MIN_PRICE, 100);
            }

            // If less than 10% rooms are available, be greedy and increase price by 50%.
            if (availableRooms / totalRooms < 0.1)
            {
                price = Math.Min(price * 1.5, MAX_PRICE); // limit to the price of less than $500
            }


            if ((star >= 2) && (star <= 5))
            {
                if (price == (star * 100 - 100))
                    minReached = true;
            }
            else
            {
                if (price == MIN_PRICE)
                    minReached = true;
            }
            return price;
        }

        public bool isMinPriceReached()
        {
            return minReached;
        }
    }
}
