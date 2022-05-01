//=============================================================================
// Program's Item class
//=============================================================================

namespace Shop
{

    // TODO: Write a class called Item that contains 2 data members:
    //       A string called name and an int called cost.
    //       This class should have a default constructor that sets name to ""
    //       and cost to 0.
    //       It should also have an overloaded constructor.
    //
    //       Write accessors for each data member. They should be called GetName
    //       GetCost.
    //       Write mutators for each data member. They should be called SetName
    //       and SetCost.


    public class Item
    {
        private string name;
        private int cost;

        public Item()
        {
            name = "";
            cost = 0;
        }

        public Item(string name, int cost)
        {
             this.name = name;
             this.cost = cost;

        }

        public string GetName()
        {
            return name;
        }


        public int GetCost()
        {
            return cost;
        }

        public void SetName(string _name)
        {
            name = _name;
        }
        public void SetCost(int _cost)
        {
            cost = _cost;
        }
    }
}
