//=============================================================================
// Program's Inventory class
//=============================================================================

using System;


namespace Shop
{
    class Inventory
    {
        // TODO: Define an int called mMaxSize, an array of Items called mItems, and an int called mGold.

        private int mMaxSize;
        private int mGold;
        private Item[] mItems;

        // TODO: Write a default constructor that assigns the mMaxSize to 10,
        //       mItems to a new array of Items with mMaxSize as the size,
        //       and mGold to 0.

        public Inventory()
        {
            mMaxSize = 10;
            mGold = 10;
            mItems = new Item[mMaxSize];
        }

        // TODO: Write a C# property for mGold called Gold.


        public int Gold
        {

            get { return mGold; }
            set { mGold = value; }


        }


        // TODO: Write a method called AddItem that returns a bool and takes an Item parameter.
        //       This method should iterate through the mItems array, looking for any Item that is null.  
        //       If a null Item is found, it should assign that array element to the Item passed in and 
        //       return true. Otherwise it should return false.

        public bool AddItem(Item item)
        {

            bool valid = false;
            for (int i = 0; i < mItems.Length ; i++)
            {
                if (mItems[i] == null)
                {
                    mItems[i] = item;
                    valid = true;
                    break;
                }
            }
            return valid;
        }

        // TODO: Write a method called RemoveItem that returns a bool and takes a string parameter.
        //       This method should iterate through the mItems array, looking for an Item that
        //       has the same name as the parameter. If it finds a match it should set that 
        //       element of the mItems array to null and return true. Otherwise return false.
        //       Do not forget to make sure the element is not null before checking its name.

        public bool RemoveItem(string itemname)
        {
            bool valid = false;
            for(int i = 0; i < mItems.Length; i++)
            {
                if (mItems[i] != null)
                {
                    if (itemname == mItems[i].GetName())
                    {
                        mItems[i] = null;
                        valid = true;
                        break;
                    }
                }
                
            }
            return valid;
        }

        // TODO: Write a method called GetItem that returns an Item and takes a string parameter.
        //       This method should iterate through the mItems array, looking for an Item that
        //       has the same name as the parameter. If it finds a match it should return that element 
        //       of the mItems array. Otherwise return null.
        //       Do not forget to make sure the element is not null before checking its name.

        public Item GetItem(string itemname)
        {
            Item newitem = null;
            for (int i = 0; i < mItems.Length; i++)
            {
                if (mItems[i] != null)
                {
                    if (itemname == mItems[i].GetName())
                    {
                        newitem = mItems[i];
                        break;
                    }
                }

            }

            return newitem;
        }

        // TODO: Uncomment the following code:
        
        public void DisplayInventory(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write("Item Name \tItem Cost");

            y = y + 1;
            Console.SetCursorPosition(x, y);

            for (int i = 0; i < mMaxSize; i++)
            {
                if (null != mItems[i])
                {
                    Console.Write(mItems[i].GetName() + " \t" + mItems[i].GetCost());

                    Console.SetCursorPosition(x, ++y);
                }
            }

            y += 2;
            Console.SetCursorPosition(x, y);
            Console.Write("Gold: " + Gold);
        }
        
    }
}
