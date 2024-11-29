namespace PragueParkingClassLibrary
{
    public class ParkingSpot
    {
        // List to store vehicles parked in this spot
        public List<Vehicle> parkingSpot { get; set; }

        // Maximum allowed size for this parking spot (fixed at 4)
        public int MaxSize { get; }

        // Current size occupied by vehicles in this parking spot
        public int CurrentSize { get; set; }

        // Constructor to initialize the parking spot with its current size
        public ParkingSpot(int currentSize)
        {
            MaxSize = 4; // Each parking spot has a maximum size of 4
            parkingSpot = new List<Vehicle>(); // Initialize the list of vehicles
            CurrentSize = currentSize; // Set the current size
        }

        // Method to check if a vehicle can be added to the parking spot
        // If the vehicle fits, add it to the list and update the current size
        public bool TakeVehicle(Vehicle vehicle)
        {
            if (CurrentSize + vehicle.Size <= MaxSize) // Check if the vehicle fits
            {
                parkingSpot.Add(vehicle); // Add vehicle to the parking spot
                CurrentSize += vehicle.Size; // Update the current size
                return true; // Indicate that the vehicle was successfully added
            }
            return false; // Vehicle could not be added due to lack of space
        }

        // Method to check if a vehicle with the given registration number is already in this parking spot
        public bool ContainsVehicle(string regNumber)
        {
            // Return true if a vehicle with the specified registration number exists in the list
            return parkingSpot.Any(vehicle => vehicle.RegNumber == regNumber);
        }
    }
}
