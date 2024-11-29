using System.Text.Json;
using PragueParkingClassLibrary;
using Spectre.Console;

namespace Prague_Parking_V2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filepath = "../../../";
            ParkingGarage pragueParking = new ParkingGarage();

            // Load parking spots from JSON
            ParkingSpot[] parkingSpots = pragueParking.ReadParkingSpotsFromJson();

            // Adjust garage size based on configuration
            parkingSpots = pragueParking.GarageSizeChange(parkingSpots);

            // Save the initial state of parking spots
            SaveParkingSpots();
            bool exit = false;

            while (!exit)
            {
                // Display program banner
                FigletPagrueParking();

                // Display parking spaces status
                ShowParkingSpaces();

                // Display price menu
                TablePriceMeny();

                // Main menu for user interaction
                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(7)
                        .AddChoices(new[] {
                            "Park Vehicle",
                            "Get Vehicle",
                            "Move Vehicle",
                            "Find Vehicle",
                            "Clear Garage",
                            "Reload Config File",
                            "Close Program",
                        }));

                // Handle menu selection
                switch (selection)
                {
                    case "Park Vehicle":
                        ParkVehicle();
                        break;
                    case "Get Vehicle":
                        GetVehicle();
                        break;
                    case "Move Vehicle":
                        MoveVehicle();
                        break;
                    case "Find Vehicle":
                        FindVehicle();
                        break;
                    case "Reload Config File":
                        ReloadConfigFile();
                        break;
                    case "Clear Garage":
                        ClearGarage();
                        break;
                    case "Close Program":
                        exit = true;
                        break;
                }

                if (!exit)
                {
                    // Prompt user to return to main menu
                    var table1 = new Table();
                    table1.AddColumn("[yellow]Press enter to return to Main Menu.[/]");
                    AnsiConsole.Write(table1);
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            // Function to park a vehicle
            void ParkVehicle()
            {
                int type = ChooseVehicleType();

                if (type == 1) // Vehicle type is Car
                {
                    string regNumber = GetRegNumber();
                    if (regNumber == "error")
                    {
                        return;
                    }

                    DateTime parkingTime = DateTime.Now;
                    Car newCar = new Car(regNumber, parkingTime);

                    bool tryPark = newCar.ParkVehicle(parkingSpots);
                    if (!tryPark)
                    {
                        Console.WriteLine("Parking Garage at capacity!");
                    }
                    SaveParkingSpots();
                }
                else if (type == 2) // Vehicle type is Mc
                {
                    string regNumber = GetRegNumber();
                    if (regNumber == "error")
                    {
                        return;
                    }

                    DateTime parkingTime = DateTime.Now;
                    Mc newMc = new Mc(regNumber, parkingTime);

                    bool tryPark = newMc.ParkVehicle(parkingSpots);
                    if (!tryPark)
                    {
                        Console.WriteLine("Parking Garage at capacity!");
                    }
                    SaveParkingSpots();
                }
            }

            // Function to choose vehicle type
            int ChooseVehicleType()
            {
                int type = 0;
                var typeChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(4)
                        .AddChoices(new[] { "Car", "Mc" }));

                if (typeChoice == "Car") type = 1;
                else if (typeChoice == "Mc") type = 2;

                return type;
            }

            // Function to get registration number of a vehicle
            string GetRegNumber()
            {
                while (true)
                {
                    Console.Write("Enter vehicle registration number: ");
                    string regNumber = Console.ReadLine()?.Trim();

                    // Input validation
                    if (string.IsNullOrEmpty(regNumber) || regNumber.Length < 1 || regNumber.Length > 10 || pragueParking.ContainsSpecialCharacters(regNumber))
                    {
                        Console.WriteLine("\nInvalid input, please try again.");
                        continue;
                    }

                    // Check if the registration number already exists
                    bool regNumberExists = parkingSpots.Any(spot => spot.ContainsVehicle(regNumber));

                    if (regNumberExists)
                    {
                        Console.WriteLine("Vehicle is already parked. Returning to main menu.");
                        return "error";
                    }
                    else
                    {
                        return regNumber;
                    }
                }
            }

            // Function to retrieve and remove a vehicle
            void GetVehicle()
            {
                string regNumber;

                do
                {
                    Console.Write("Enter Registration Number: ");
                    regNumber = Console.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(regNumber))
                    {
                        var table2 = new Table();
                        table2.AddColumn("[yellow]Vehicle not found. Returning to main menu.[/]");
                        AnsiConsole.Write(table2);
                        return;
                    }
                } while (string.IsNullOrEmpty(regNumber));

                // Locate the vehicle
                ParkingSpot currentSpot = null;
                Vehicle vehicleToRemove = null;
                int currentSpotIndex = -1;

                for (int i = 1; i < parkingSpots.Length; i++)
                {
                    var spot = parkingSpots[i];
                    vehicleToRemove = spot.parkingSpot.FirstOrDefault(v => v.RegNumber == regNumber);

                    if (vehicleToRemove != null)
                    {
                        currentSpot = spot;
                        currentSpotIndex = i;
                        break;
                    }
                }

                if (currentSpot == null || vehicleToRemove == null)
                {
                    var table2 = new Table();
                    table2.AddColumn("[yellow]Vehicle not found. Returning to main menu.[/]");
                    AnsiConsole.Write(table2);
                    return;
                }

                // Calculate parking cost
                DateTime currentTime = DateTime.Now;
                TimeSpan parkingDuration = currentTime - vehicleToRemove.ParkingTime;
                double price = CalculateParkingCost(vehicleToRemove, parkingDuration);

                Console.WriteLine($"Parking duration: {parkingDuration.TotalMinutes:F1} minutes.");
                Console.WriteLine($"Parking cost: {price:F2} CZK");

                // Confirm removal of vehicle
                Console.WriteLine("Do you want to retrieve and remove the vehicle?");
                var confirm = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .PageSize(4).AddChoices(new[] { "Yes", "No" }));
                if (confirm == "Yes")
                {
                    currentSpot.parkingSpot.Remove(vehicleToRemove);
                    currentSpot.CurrentSize -= vehicleToRemove.Size;

                    Console.WriteLine($"Vehicle {regNumber} has been retrieved from spot {currentSpotIndex}.");

                    // Save updated parking spots
                    SaveParkingSpots();
                }
            }
            void MoveVehicle()
            {
                // Prompt the user to enter the vehicle's registration number
                string regNumber;
                do
                {
                    Console.Write("Enter registration number: ");
                    regNumber = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(regNumber))
                    {
                        var table2 = new Table();
                        table2.AddColumn("[yellow]Vehicle not found. Returning to main menu.[/]");
                        AnsiConsole.Write(table2);
                        return;
                    }
                } while (string.IsNullOrEmpty(regNumber));

                // Variables to track the current parking spot and vehicle
                ParkingSpot currentSpot = null;
                Vehicle vehicleToMove = null;
                int currentSpotIndex = -1;

                // Loop through parking spots to locate the vehicle
                for (int i = 1; i < parkingSpots.Length; i++)
                {
                    var spot = parkingSpots[i];
                    vehicleToMove = spot.parkingSpot.FirstOrDefault(vehicle => vehicle.RegNumber == regNumber);
                    if (vehicleToMove != null)
                    {
                        currentSpot = spot;
                        currentSpotIndex = i;
                        break;
                    }
                }

                // Check if the vehicle was found
                if (currentSpot == null)
                {
                    var table3 = new Table();
                    table3.AddColumn($"[yellow]Vehicle with registration number {regNumber} not found.[/]");
                    AnsiConsole.Write(table3);
                    return;
                }

                Console.WriteLine($"Vehicle '{regNumber}' is registered to spot '{currentSpotIndex}'");
                int newSpotIndex;
                bool isValidtoCheckOut = true;

                // Prompt the user for a new parking spot and attempt to move the vehicle
                do
                {
                    Console.Write("Enter new parking spot number: ");

                    if (int.TryParse(Console.ReadLine(), out newSpotIndex) && newSpotIndex > 0 && newSpotIndex < parkingSpots.Length)
                    {
                        var newSpot = parkingSpots[newSpotIndex];

                        // Check if the new spot has enough space for the vehicle
                        if (newSpot.CurrentSize + vehicleToMove.Size <= newSpot.MaxSize)
                        {
                            // Remove the vehicle from the current spot
                            currentSpot.parkingSpot.Remove(vehicleToMove);
                            currentSpot.CurrentSize -= vehicleToMove.Size;

                            // Move the vehicle to the new spot
                            newSpot.parkingSpot.Add(vehicleToMove);
                            newSpot.CurrentSize += vehicleToMove.Size;
                            Console.WriteLine($"Vehicle '{regNumber}' moved to spot '{newSpotIndex}'");

                            // Save updated parking spots
                            SaveParkingSpots();
                            isValidtoCheckOut = false;
                        }
                        else
                        {
                            Console.WriteLine("Not enough space in the new spot.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid parking spot number. Please try again.");
                    }
                } while (isValidtoCheckOut);
            }

            void FindVehicle()
            {
                // Prompt the user to enter the registration number
                Console.Write("Enter registration number: ");
                string regnumber = Console.ReadLine()?.Trim();
                bool found = false;

                // Loop through parking spots to locate the vehicle
                for (int i = 1; i < parkingSpots.Length; i++)
                {
                    var spot = parkingSpots[i];
                    var vehicle = spot?.parkingSpot.FirstOrDefault(v => v.RegNumber == regnumber);

                    if (vehicle != null)
                    {
                        // Calculate parking duration and cost
                        DateTime currentTime = DateTime.Now;
                        TimeSpan duration = currentTime - vehicle.ParkingTime;
                        double price = CalculateParkingCost(vehicle, duration);

                        // Display vehicle information
                        Console.WriteLine($"Vehicle '{regnumber}' is registered to spot '{i}'");
                        Console.WriteLine($"Park Duration: {duration.TotalMinutes:F1} minutes");
                        Console.WriteLine($"Parking Cost: {price:F2} CZK");
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Vehicle not found.");
                }
            }

            double CalculateParkingCost(Vehicle vehicle, TimeSpan duration)
            {
                // Free parking duration in minutes
                const double freeTime = 10;
                double rate = 0;

                if (duration.TotalMinutes <= freeTime)
                {
                    return 0; // No cost for parking within the free time limit
                }
                else
                {
                    // Determine the parking rate based on vehicle size
                    if (vehicle.Size == 2)
                    {
                        rate = pragueParking.McPrize;
                    }
                    else if (vehicle.Size == 4)
                    {
                        rate = pragueParking.CarPrize;
                    }
                }

                // Calculate total cost based on parking duration and rate
                return ((duration.TotalMinutes - freeTime) / 60) * rate;
            }

            void ShowParkingSpaces()
            {
                // Counters for empty, half-full, and full parking spots
                int emptyCount = -1;
                int halfFullCount = 0;
                int fullCount = 0;

                // Iterate through parking spots to calculate status
                foreach (var spot in parkingSpots)
                {
                    if (spot.CurrentSize == 0)
                    {
                        emptyCount++;
                    }
                    else if (spot.CurrentSize < spot.MaxSize)
                    {
                        halfFullCount++;
                    }
                    else if (spot.CurrentSize == spot.MaxSize)
                    {
                        fullCount++;
                    }
                }

                // Display parking space status using a breakdown chart
                var chart = new BreakdownChart()
                    .FullSize()
                    .AddItem("Empty", emptyCount, Color.Green)
                    .AddItem("Half Full", halfFullCount, Color.Yellow)
                    .AddItem("Full", fullCount, Color.Red);

                AnsiConsole.Write(new Markup("[grey bold]Parking Space[/]\n"));
                AnsiConsole.Write(chart);
            }

            void SaveParkingSpots()
            {
                // Serialize and save the parking spots array to a JSON file
                string updatedParkingArrayJsonString = JsonSerializer.Serialize(parkingSpots, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filepath + "ParkingArray.json", updatedParkingArrayJsonString);
            }

            void ReloadConfigFile()
            {
                // Reload the configuration file and adjust garage size
                pragueParking.ReloadConfigTxt();
                parkingSpots = pragueParking.GarageSizeChange(parkingSpots);

                // Save updated parking spots
                SaveParkingSpots();
            }

            #region Top Main Menu Design
            // Displays a styled title banner for the application
            void FigletPagrueParking()
            {
                AnsiConsole.Write(
                    new FigletText("PRAGUE PARKING")
                        .Centered()
                        .Color(Color.Red)); // Title in red color, centered
                AnsiConsole.WriteLine("\n");
                var table = new Table()
                    .Centered()
                    .AddColumn("[bold green]Welcome to Prague Parking System[/]")
                    .AddRow("[dim]Manage your parking efficiently with style![/]");
                AnsiConsole.Write(table);
                Console.WriteLine();
            }

            // Displays a table showing the status of parking spots
            void TableStatusVehicle()
            {
                var table = new Table()
                    .Centered()
                    .BorderColor(Color.Blue); // Adds a blue border for a better appearance

                table.AddColumns(
                    "[green bold]EMPTY SPOT[/]",
                    "[yellow bold]HALF FULL[/]",
                    "[red bold]FULL SPOT[/]"
                );

                table.AddRow("[green]Available Spaces[/]", "[yellow]Moderate Usage[/]", "[red]No Space Left[/]");
                AnsiConsole.WriteLine("\n[underline bold blue]PARKING STATUS[/]\n");
                AnsiConsole.Write(table);
            }

            // Displays a table with the pricing information for vehicle types
            void TablePriceMeny()
            {
                var table = new Table()
                    .Centered()
                    .Border(TableBorder.Rounded) // Rounded borders for a sleek design
                    .BorderColor(Color.Green) // Green border to match the theme
                    .Title("[bold blue]Pricing Information[/]");

                table.AddColumn("[bold yellow]Vehicle Type[/]");
                table.AddColumn(new TableColumn("[bold yellow]Price Per Hour[/]").Centered());

                // Add rows for each vehicle type
                table.AddRow("[bold white]Free Parking[/]", "[dim]First 10 minutes[/]");
                table.AddRow("[bold green]Motorcycle[/]", $"{pragueParking.McPrize} CZK");
                table.AddRow("[bold cyan]Car[/]", $"{pragueParking.CarPrize} CZK");

                AnsiConsole.Write(table);
            }
            #endregion

            // Clears all vehicles from the garage
            void ClearGarage()
            {
                // Confirm user decision to clear the garage
                Console.WriteLine("[red bold]Are you sure you want to empty the entire garage?[/]");
                var confirm = AnsiConsole.Prompt(new SelectionPrompt<string>()
                     .PageSize(4)
                     .AddChoices(new[] { "[bold green]Yes[/]", "[bold red]No[/]" }));

                if (confirm == "Yes")
                {
                    // Clear each parking spot
                    for (int i = 1; i < parkingSpots.Length; i++)
                    {
                        parkingSpots[i].parkingSpot.Clear(); // Clear the vehicles
                        parkingSpots[i].CurrentSize = 0; // Reset the spot's current size
                    }

                    // Save the updated state of the garage
                    SaveParkingSpots();
                    AnsiConsole.Write(new Markup("[green bold]The garage has been cleared successfully![/]"));
                }
                else
                {
                    // User chose not to clear the garage
                    AnsiConsole.Write(new Markup("[yellow bold]Garage clearing cancelled.[/]"));
                    return;
                }
            }
        }
    }
}
