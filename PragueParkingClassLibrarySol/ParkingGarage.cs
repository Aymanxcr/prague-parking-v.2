using System.Text.Json;
using System.Text.RegularExpressions;

namespace PragueParkingClassLibrary
{
    public class ParkingGarage
    {
        // Properties to store the price for motorcycles, cars, and the garage size
        public int McPrize { get; set; }
        public int CarPrize { get; set; }
        public int GarageSize { get; set; }

        // Constructor to initialize the parking garage by reading configuration values from a file
        public ParkingGarage()
        {
            string filepath = "../../../";
            var configValues = new Dictionary<string, int>();

            // Read configuration file and extract key-value pairs
            foreach (var line in File.ReadLines(filepath + "Config.txt"))
            {
                if (string.IsNullOrEmpty(line) || line.TrimStart().StartsWith("#")) continue;

                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim().Split('#')[0].Trim();
                    configValues[key] = int.Parse(value);
                }
            }

            // Assign values to properties based on the configuration file
            configValues.TryGetValue("McPrize", out int configMcPrize);
            configValues.TryGetValue("CarPrize", out int configCarPrize);
            configValues.TryGetValue("GarageSize", out int configGarageSize);

            this.McPrize = configMcPrize;
            this.CarPrize = configCarPrize;
            this.GarageSize = configGarageSize;
        }

        // Method to reload configuration values while the program is running
        public void ReloadConfigTxt()
        {
            string filepath = "../../../";
            var configValues = new Dictionary<string, int>();

            // Read configuration file and extract key-value pairs
            foreach (var line in File.ReadLines(filepath + "Config.txt"))
            {
                if (string.IsNullOrEmpty(line) || line.TrimStart().StartsWith("#")) continue;

                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim().Split('#')[0].Trim();
                    configValues[key] = int.Parse(value);
                }
            }

            // Update property values based on the reloaded configuration file
            configValues.TryGetValue("McPrize", out int configMcPrize);
            configValues.TryGetValue("CarPrize", out int configCarPrize);
            configValues.TryGetValue("GarageSize", out int configGarageSize);

            this.McPrize = configMcPrize;
            this.CarPrize = configCarPrize;
            this.GarageSize = configGarageSize;
        }

        // Method to update the size of the parking garage
        // Ensures the garage size can only be reduced when the garage is empty
        public ParkingSpot[] GarageSizeChange(ParkingSpot[] input)
        {
            bool isEmpty = true;
            ParkingSpot[] output;

            // Check if the garage is empty
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].CurrentSize > 0)
                {
                    isEmpty = false;
                    break;
                }
            }

            // Handle garage size adjustment based on conditions
            if (this.GarageSize < input.Length && isEmpty == false)
            {
                Console.WriteLine("Garage not empty, number of spots remains the same. \n" +
                                  "Please empty the garage before decreasing its size.");
                return input;
            }
            else if (this.GarageSize < input.Length && isEmpty == true)
            {
                output = new ParkingSpot[this.GarageSize];
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = new ParkingSpot(0);
                }
            }
            else
            {
                output = new ParkingSpot[this.GarageSize];
                Array.Copy(input, output, input.Length);
                for (int i = input.Length; i < output.Length; i++)
                {
                    output[i] = new ParkingSpot(0);
                }
            }
            return output;
        }

        // Method to load parking spots from a JSON file at the start of the program
        public ParkingSpot[] ReadParkingSpotsFromJson()
        {
            string filepath = "../../../";
            ParkingSpot[] parkingSpots;

            if (File.Exists(filepath + "ParkingArray.json"))
            {
                // Deserialize JSON to create the array of parking spots
                string parkingJsonString = File.ReadAllText(filepath + "ParkingArray.json");
                parkingSpots = JsonSerializer.Deserialize<ParkingSpot[]>(parkingJsonString);
            }
            else
            {
                // Initialize parking spots with default values if JSON does not exist
                DateTime testDateTime = DateTime.Now;
                parkingSpots = new ParkingSpot[this.GarageSize];
                for (int i = 0; i < parkingSpots.Length; i++)
                {
                    parkingSpots[i] = new ParkingSpot(0);
                }
                Car testCar = new Car("test123", testDateTime);
                Mc testMc = new Mc("test456", testDateTime);
            }

            return parkingSpots;
        }

        // Method to validate if a registration number contains special characters
        public bool ContainsSpecialCharacters(string regNumber)
        {
            return Regex.IsMatch(regNumber, @"[^\p{L}\p{N}]");
        }

        // Method to check if a file exists (used in tests)
        public bool FileExists(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            return File.Exists(fileName);
        }
    }
}
