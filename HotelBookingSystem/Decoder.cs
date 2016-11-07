/* CSE 598 - Assignment 2 - James & Thao Group Project
 * Members: James Truong, Thao Vos
 * Class: Encoder
 * Side: Server
 * Responsible person: Thao
 */

using System.IO;
using System.Xml.Serialization;


namespace HotelBookingSystem
{
    class Decoder
    {
        //On the server side
        //Receive a string and send the order back as an object to the hotel
        public static OrderClass DecodedStr(string encodedStr)
        {
            XmlSerializer converter = new XmlSerializer(typeof(OrderClass));

            using (TextReader reader = new StringReader(encodedStr))
            {
                return (OrderClass)converter.Deserialize(reader);
            }
        }
    }
}
