/* CSE 598 - Assignment 2 - James & Thao Group Project
 * Members: James Truong, Thao Vo
 * Class: Hotel
 * Side: Server
 * Responsible person: Thao
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotelBookingSystem
{
    public delegate void priceCutEvent(double pr, int hotelId); //define a delegate
    public delegate void terminateSubscriberEvent(int hotelId);

    class Hotel
    {
        public const int MAX_ORDER = 20;

        public Hotel(int id, int star, int totalRooms, MultiCellBuffer orderBuffer, MultiCellBuffer confirmationBuffer)
        {
            this.myId = id;
            this.buffer = orderBuffer;
            this.confirmationBuffer = confirmationBuffer;
            this.priceModel = new PricingModel(star, totalRooms);
            this.totalRooms = totalRooms;
            availableRooms = totalRooms;
        }

        private MultiCellBuffer buffer; // copy of the buffer object
        private MultiCellBuffer confirmationBuffer;
        public int myId;
        private int priceCutCount = 0;
        private PricingModel priceModel;

        public event priceCutEvent priceCut; //Link event to delegate
        public event terminateSubscriberEvent terminateSubscribers;
        private double hotelPrice = PricingModel.MAX_PRICE;
        private int totalRooms;
        private int availableRooms;

        public void pricingModel(double price)
        {
            if (price < hotelPrice)
            { //a price cutt
                Console.WriteLine("\t ---Hotel " + myId + " is having a price cut!! New: " + price + ". Old: " + hotelPrice + "---\n");

                if (priceCut != null) //at least a subscriber
                {
                    priceCut(price, myId); //emit event to subscribers = delegate
                    priceCutCount++;
                }
                hotelPrice = price;
            }
        }

        // Thread function
        public void hotelFunction()
        {
            // Terminate once a number of orders have been received or minimum possible price has been reached
            while ((priceCutCount <= MAX_ORDER) && !priceModel.isMinPriceReached())
            {
                Thread.Sleep(200);
                //take order from queue of orders;
                //decide the price based on the orders
                double price = priceModel.getPrice(availableRooms);

                pricingModel(price);

                // Get all the cells matching myId
                string str = "";
                do // Get all the cells that match myId
                {
                    str = buffer.getOneCell(myId);

                    if (str != "")
                    {
                        OrderClass orderObject = Decoder.DecodedStr(str);
                        OrderProcessing orderProccesing = new OrderProcessing(orderObject, confirmationBuffer);

                        Thread orderProcessingThread = new Thread(new ThreadStart(orderProccesing.orderProcessingFunction));
                        orderProcessingThread.Start(); //start order thread
                    }
                } while (str != "") ;
            }

            // All orders are done. Time to tell my subscribers that I am done
            if (terminateSubscribers != null)
                terminateSubscribers(myId);

            Console.WriteLine("\n*** Hotel thread {0} ends ***\n", myId);
        }
    }
}
