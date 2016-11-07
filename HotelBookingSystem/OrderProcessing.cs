/* CSE 598 - Assignment 2 - James & Thao Group Project
 * Members: James Truong, Thao Vo
 * Class: OrderProcessing
 * Side: Server
 * Responsible person: Thao
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

//Whenever an order needs to be processed, a new thread is instantiated from this class to process the order. 
//It checks for the validity of the credit card number, and send a confirmation.

namespace HotelBookingSystem
{
    class OrderProcessing
    {
        private OrderClass currentOrder;
        private MultiCellBuffer confirmationBuffer; // copy of the buffer object

        public OrderProcessing(OrderClass order, MultiCellBuffer confirmationBuffer)
        {
            this.currentOrder = order;
            this.confirmationBuffer = confirmationBuffer;
        }

        public void orderProcessingFunction()
        {
            
            if (currentOrder != null)
            {
                string confirmation = Encoder.EncodeOrder(currentOrder);

                // Switch the SenderId and ReceiverId
                // SenderId is now the Hotel ID and ReceiverId is now Agency ID
                int temp = currentOrder.SenderId;
                currentOrder.SenderId = currentOrder.ReceiverId;
                currentOrder.ReceiverId = temp;

                if (validateCardInfo())
                {
                    currentOrder.Accepted = true;
                }
                else
                {
                    currentOrder.Accepted = false;
                }

                //send the credit card confirmation result back
                confirmation = Encoder.EncodeOrder(currentOrder); 

                bool set = false;
                do
                {
                    set = confirmationBuffer.setOneCell(confirmation);
                    Thread.Sleep(100);
                }
                while (!set);
            }
            else
            {
                DebugConsole.WriteLine("Current Order: " + Thread.CurrentThread.Name + " No order received");

            }
        }

        //Check to see if the credit card is a valid card number
        private bool validateCardInfo()
        {
            bool validated = false;
            // Check for a valid credit card number
            if (currentOrder.CardNo <= (5 * 100000000))
            {
                DebugConsole.WriteLine("Sorry, your card was declined.");
                validated = false;
                //card declined
            }
            else if (currentOrder.CardNo >= (5 * 100000000))
            {
                validated = true;
                //card accepted
                DebugConsole.WriteLine("Successful transaction! Your card was accepted!");
            }

            return validated;
        }
    }
}
