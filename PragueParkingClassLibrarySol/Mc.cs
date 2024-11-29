namespace PragueParkingClassLibrary
{
    // Represents a Motorcycle (Mc), which is a specific type of Vehicle
    public class Mc : Vehicle
    {
        // The size of the motorcycle, fixed at 2 (e.g., representing parking space units)
        private int size = 2;

        // Override of the Size property from the base Vehicle class
        // Provides the specific size of a motorcycle
        public override int Size
        {
            get { return size; } // Returns the size of the motorcycle
        }

        // Constructor for the Mc class
        // Takes a registration number and parking time as parameters
        public Mc(string regNumber, DateTime parkingTime)
            : base(regNumber, parkingTime) // Calls the base class (Vehicle) constructor
        {
            // No additional logic needed here; functionality is inherited from the base class
        }
    }
}
