/* CSE 598 - Assignment 2 - James & Thao Group Project
 * Members: James Truong, Thao Vo
 * Class: TravelAgency
 * Side: Client
 * Responsible person: James
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotelBookingSystem
{
    //Evaluates the price, generates an OrderObject (consisting of multiple values),
    //and sends the order to the Encoder to convert the order object into a plain String.
    public struct HotelSale
    {
        public Boolean orderNeeded;
        public double newLoweredPrice;
        public int hotelId;
    }

    class TravelAgency
    {
        private ConcurrentQueue<HotelSale> hotelSales;
        private MultiCellBuffer buffer; // copy of the buffer object
        private MultiCellBuffer confirmationBuffer; // copy of the buffer object
        private OrderClass orderObject;
        private int myId;
        static Random rng = new Random();
        private Dictionary<int, double> previousPrices;
        private HashSet<int> hotelIds;

        public TravelAgency(MultiCellBuffer orderBuffer, MultiCellBuffer confirmationBuffer)
        {
            this.buffer = orderBuffer;
            this.confirmationBuffer = confirmationBuffer;
            this.hotelSales = new ConcurrentQueue<HotelSale>();
            this.orderObject = new OrderClass();
            this.previousPrices = new Dictionary<int, double>();
            this.hotelIds = new HashSet<int>();
        }

        public void agencyFunction()
        {  //for starting thread
            myId = Thread.CurrentThread.ManagedThreadId;

            // Process until all the hotels, whom we subscribe to, end
            while (hotelIds.Count() > 0)
            {
                Thread.Sleep(200);
                HotelSale sale;

                // Take all the sales that were enqueued and create orders
                while (hotelSales.TryDequeue(out sale))
                {
                    // Process only hotels that still have threads still running
                    if (hotelIds.Contains(sale.hotelId))
                    {
                        orderObject = generateOrder(sale.newLoweredPrice, sale.hotelId);
                        string order = Encoder.EncodeOrder(orderObject);

                        // Try writing the new order to buffer; if failed due to timeout, put it back to the queue sales
                        if (!buffer.setOneCell(order))
                        {
                            hotelSales.Enqueue(sale);
                        }
                    }
                }

                string confirmation = "";
                do // Get all the cells that match myId
                {
                    confirmation = confirmationBuffer.getOneCell(myId);

                    if (confirmation != "")
                    {
                        OrderClass confirmationObject = Decoder.DecodedStr(confirmation);

                        if (confirmationObject.Accepted)
                        {
                            Console.WriteLine("ORDER CONFIRMATION: \n{0}\n", confirmation);
                        }
                        else
                        {
                            Console.WriteLine("DECLINED ORDER: \n{0}\n", confirmation);
                        }

                    }
                } while (confirmation != "");
            }
            Console.WriteLine("*** Agency thread {0} ends ***", myId);
        }

        // Register the hotels that I am subscribing to in order to terminate myself if the hotels are done
        public void registerHotelId(int hotelId)
        {
            hotelIds.Add(hotelId);
        }

        public void hotelOnSaleHandler(double newPrice, int hotelId)
        {  //event handler
            DebugConsole.WriteLine("--- Hotel " + hotelId + " is calling On-Sale handler of Agency " + myId + " with new lowered price " + newPrice);

            HotelSale sale = new HotelSale();
            sale.hotelId = hotelId;
            sale.newLoweredPrice = newPrice;

            lock (previousPrices)
            {
                hotelSales.Enqueue(sale);

                if (previousPrices.ContainsKey(hotelId))
                    previousPrices[hotelId] = newPrice;
                else
                    previousPrices.Add(hotelId, newPrice);
            }
        }

        // Event handler used to notify the Travel Agency that the specified hotel thread has ended
        public void threadTerminateHandler(int hotelId)
        {
            if (hotelIds.Contains(hotelId))
                hotelIds.Remove(hotelId);
        }

        private OrderClass generateOrder(double newPrice, int hotelId)
        {
            orderObject.SenderId = Thread.CurrentThread.ManagedThreadId;
            orderObject.ReceiverId = hotelId;
            orderObject.CardNo = Convert.ToUInt64(rng.Next(0, 99999999));
            orderObject.CardNo = orderObject.CardNo * 100000000 + Convert.ToUInt64(rng.Next(0, 99999999));
            orderObject.Amount = calculateNumberOfRoomsToOrder(newPrice, hotelId);
            orderObject.Price = newPrice;
            orderObject.TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff"); // Save the timestamp before sending the order to MulticellBuffer
            //Console.WriteLine("Agency {0} is generating order. SenderId {1}; CardNo {2}; ReceiverId {3}; Amount {4}; Price {5}",
            //    myId, orderObject.SenderId, orderObject.CardNo, orderObject.ReceiverId, orderObject.Amount, orderObject.Price);
            return orderObject;
        }

        private int calculateNumberOfRoomsToOrder(double newPrice, int hotelId)
        {
            // For every 5% difference in price, add a room. Initial number of rooms to order is 1
            int rooms = 1;
            double prevPrice = 0;
            if (previousPrices.TryGetValue(hotelId, out prevPrice))
            {
                double rateOfDifference = (prevPrice - newPrice) / prevPrice;
                rooms += Convert.ToInt32(rateOfDifference / 0.05);
            }
            return rooms;
        }
    }
}
