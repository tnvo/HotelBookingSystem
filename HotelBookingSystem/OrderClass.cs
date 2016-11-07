/* CSE 598 - Assignment 2 - James & Thao Group Project
 * Members: James Truong, Thao Vo
 * Class: OrderClass
 * Side: Client
 * Responsible person: James
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HotelBookingSystem
{
    public class OrderClass
    {
        private int senderId; // ID of the sender
        private UInt64 cardNo; // credit card number
        private int receiverId; // ID of the receiver
        private int amount; // number of rooms to order
        private Double price; // unit price of the room received from the hotel
        private string timeStamp; // Timestamp when order is created
        private bool accepted; // order is accepted?

        public int SenderId
        {
            get
            {
                return senderId;
            }

            set
            {
                senderId = value;
            }
        }

        public UInt64 CardNo
        {
            get
            {
                return cardNo;
            }

            set
            {
                cardNo = value;
            }
        }

        public int ReceiverId
        {
            get
            {
                return receiverId;
            }

            set
            {
                receiverId = value;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }

            set
            {
                amount = value;
            }
        }

        public double Price
        {
            get
            {
                return price;
            }

            set
            {
                price = value;
            }
        }

        public string TimeStamp
        {
            get
            {
                return timeStamp;
            }

            set
            {
                timeStamp = value;
            }
        }

        public bool Accepted
        {
            get
            {
                return accepted;
            }

            set
            {
                accepted = value;
            }
        }

    }
}
