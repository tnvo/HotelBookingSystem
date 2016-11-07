/* CSE 598 - Assignment 2 - James & Thao Group Project
 * Members: James Truong, Thao Vo
 * Class: Encoder
 * Side: Client
 * Responsible person: James
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace HotelBookingSystem
{
    class Encoder
    {
        //On the client side
        //Receives the order object from TravelAgency and send the order back as a string
        public static string EncodeOrder(OrderClass order)
        {
            XmlSerializer serializer = new XmlSerializer(order.GetType());

            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, order);
                return sw.ToString();
            }
        }
    }
}

