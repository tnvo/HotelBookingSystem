/* CSE 598 - Assignment 2 - James & Thao Group Project
 * Members: James Truong, Thao Vo
 * Class: MultiCellBuffer
 * Responsible person: James & Thao
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;

namespace HotelBookingSystem
{
    //Used for the communication between the travel agencies (clients) and the hotel chains (servers)
    public class MultiCellBuffer
    {
        // Size of the Multi-Cell Buffer
        int size;
        private string[] cellBuffer;
        private int[] keys;

        // Semaphores to control read/write access
        Semaphore write;
        Semaphore read;

        private ReaderWriterLock rwLock;

        public MultiCellBuffer(int size)
        {
            this.size = size;
            this.cellBuffer = new string[size];
            this.keys = new int[size];
            this.rwLock = new ReaderWriterLock();

            write = new Semaphore(size, size);
            read = new Semaphore(size, size);
        }

        /// Mutator for the Multi-Cell Buffer. Uses locks, Monitors, and Semaphores to ensure synchronization between threads.

        public bool setOneCell(string order)
        {
            bool set = false;
            write.WaitOne();
            DebugConsole.WriteLine("THREAD: " + Thread.CurrentThread.Name + " Entered Write");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(order);

            XmlNode node = doc.DocumentElement.SelectSingleNode("ReceiverId");
            
            if (node != null)
            {
                int callerId = Convert.ToInt32(node.InnerText);
                    
                for (int i = 0; i < size; i++)
                {
                    rwLock.AcquireWriterLock(100);
                    try
                    {
                        if (keys[i] == 0)
                        {
                            cellBuffer[i] = order;
                            keys[i] = callerId;
                            set = true;

                            DebugConsole.WriteLine("WRITING: " + Thread.CurrentThread.Name +
                                " Multi -Cell Buffer\n " + order + "\nElements: " + i);
                            break;
                        }
                    }
                    finally
                    {
                        rwLock.ReleaseWriterLock();
                    }
                }
            }
            else
            {
                DebugConsole.WriteLine("THREAD: " + Thread.CurrentThread.Name + " Bad encoded order");
            }
            write.Release();
            DebugConsole.WriteLine("THREAD: " + Thread.CurrentThread.Name + " Leaving Write");

            //Monitor.Pulse(cellBuffer);
            //Thread.Sleep(100);
            //}

            return set;
        }

        /// Accessor for the Multi-Cell Buffer. Uses locks, Monitors, and Semaphores to ensure synchronization between threads.
        /// This will return the order from the TravelAgency
        public string getOneCell(int callerId)
        {
            read.WaitOne();
            DebugConsole.WriteLine("THREAD: " + Thread.CurrentThread.Name + " Entered Read");
            string cell = "";
            for (int i = 0; i < size; i++)
            {
                rwLock.AcquireWriterLock(100);
                try
                {
                    if (keys[i] == callerId)
                    {
                        cell = cellBuffer[i];
                        keys[i] = 0;
                        break;
                    }
                }
                finally
                {
                    rwLock.ReleaseWriterLock();
                }
            }
            read.Release();

            DebugConsole.WriteLine("THREAD: " + Thread.CurrentThread.Name + " Leaving Read");
            
            return cell;
        }
    }
}