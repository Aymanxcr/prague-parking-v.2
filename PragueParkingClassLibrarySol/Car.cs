namespace PragueParkingClassLibrary
{
    // Represents a Car, which is a specific type of Vehicle
    public class Car : Vehicle
    {
        // The size of the car, fixed at 4 (e.g., representing parking space units)
        private int size = 4;

        // Override of the Size property from the base Vehicle class
        // Provides the specific size of a car
        public override int Size
        {
            get { return size; } // Returns the size of the car
        }

        // Constructor for the Car class
        // Takes a registration number and parking time as parameters
        public Car(string regNumber, DateTime parkingTime)
            : base(regNumber, parkingTime) // Calls the base class (Vehicle) constructor
        {
            // No additional logic needed here; functionality is inherited from the base class
        }
    }
}
