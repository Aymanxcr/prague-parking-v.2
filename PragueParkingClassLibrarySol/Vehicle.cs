namespace PragueParkingClassLibrary
{
    public class Vehicle
    {
        // The registration number of the vehicle
        public string RegNumber { get; set; }

        // The time when the vehicle was parked
        public DateTime ParkingTime { get; set; }

        // The size of the vehicle, can be overridden by derived classes (e.g., Car, Mc)
        public virtual int Size { get; set; }

        // Constructor to initialize the vehicle with its registration number and parking time
        public Vehicle(string regNumber, DateTime parkingTime)
        {
            RegNumber = regNumber; // Assign the registration number
            ParkingTime = parkingTime; // Assign the parking time
        }

        // Method to park the vehicle in the first available parking spot
        // Returns true if the vehicle was successfully parked, false otherwise
        public bool ParkVehicle(ParkingSpot[] parkingSpots)
        {
            // Iterate through the array of parking spots
            for (int i = 1; i < parkingSpots.Length; i++)
            {
                // Attempt to park the vehicle in the current spot
                if (parkingSpots[i].TakeVehicle(this))
                {
                    // Log the successful parking of the vehicle
                    Console.WriteLine("Vehicle '{0}' registered to spot '{1}'", this.RegNumber, i);
                    return true; // Return true if parking was successful
                }
            }

            // Return false if no suitable parking spot was found
            return false;
        }
    }
}
