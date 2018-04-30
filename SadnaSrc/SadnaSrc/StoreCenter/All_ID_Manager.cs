using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    /**
     * to answer the question: why this class exists?
     * I started to move the ID managment to to the slaves, but I found a problem.
     * in any given time in the system, we can have many managers of different stores.
     * let's take a look at example:
     * we have 2 shoppers that want to create a store, and they logged in at the same time.
     * if 2 are creating a store at the same time - without using one handler that will manage the ID
     * both will create an Store with the same ID (each have a local copy of the slave, and the slaves
     * start with the same ID value) - therefore the ID won't be uniqe.
     * 
     * same goes for all the other ID types.
     * 
     **/
    class All_ID_Manager
    {
        static All_ID_Manager instance;
        private int StoreIdCounter;
        private int globalProductID;
        private int globalDiscountCode;
        private int globalLotteryID;
        private int globalLotteryTicketID;
        public StoreDL DataLayer { get; }
        public static All_ID_Manager GetInstance()
        {
            if (instance == null)
            {
                instance = new All_ID_Manager();
                return instance;
            }
            return instance;
        }
        private All_ID_Manager()
        {
            DataLayer = StoreDL.Instance;
            StoreIdCounter = FindMaxStoreId();
            globalProductID = FindMaxProductId();
            globalDiscountCode = FindMaxDiscountId();
            globalLotteryID = FindMaxLotteryId();
            globalLotteryTicketID = FindMaxLotteryTicketId();
        }

        private static int FindMaxLotteryTicketId()
        {
            StoreDL DL = StoreDL.Instance;
            LinkedList<string> list = DL.getAllLotteryTicketIDs();
            int max = -5;
            int temp = 0;
            foreach (string s in list)
            {
                temp = Int32.Parse(s.Substring(1));
                if (temp>max)
                {
                    max = temp;
                }
            }
            return max;
        }

        private static int FindMaxLotteryId()
        {
            StoreDL DL = StoreDL.Instance;
            LinkedList<string> list = DL.getAllLotteryManagmentIDs();
            int max = -5;
            int temp = 0;
            foreach (string s in list)
            {
                temp = Int32.Parse(s.Substring(1));
                if (temp > max)
                {
                    max = temp;
                }
            }
            return max;
        }

        private static int FindMaxDiscountId()
        {
            StoreDL DL = StoreDL.Instance;
            LinkedList<string> list = DL.getAllDiscountIDs();
            int max = -5;
            int temp = 0;
            foreach (string s in list)
            {
                temp = Int32.Parse(s.Substring(1));
                if (temp > max)
                {
                    max = temp;
                }
            }
            return max;
        }

        private static int FindMaxProductId()
        {
            StoreDL DL = StoreDL.Instance;
            LinkedList<string> list = DL.getAllProductIDs();
            int max = -5;
            int temp = 0;
            foreach (string s in list)
            {
                temp = Int32.Parse(s.Substring(1));
                if (temp > max)
                {
                    max = temp;
                }
            }
            return max;
        }

        private static int FindMaxStoreId()
        {
            StoreDL DL = StoreDL.Instance;
            LinkedList<string> list = DL.getAllStoresIDs();
            int max = -5;
            int temp = 0;
            foreach (string s in list)
            {
                temp = Int32.Parse(s.Substring(1));
                if (temp > max)
                {
                    max = temp;
                }
            }
            return max;
        }

        public string GetProductID()
        {
            globalProductID++;
            return "P" + globalProductID;
        }
        public string GetDiscountCode()
        {
            globalDiscountCode++;
            return "D" + globalDiscountCode;
        }
        public string GetNextStoreId()
        {
            StoreIdCounter++;
            return "S" + StoreIdCounter;
        }
        public string GetLottyerID()
        {
            globalLotteryID++;
            return "L" + globalLotteryID;
        }
        public string GetLotteryTicketID()
        {
            globalLotteryTicketID++;
            return "T" + globalLotteryTicketID;
        }
    }
}
