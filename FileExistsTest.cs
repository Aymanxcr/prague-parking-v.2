using PragueParkingClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PragueParkingTests
{
    [TestClass]
    public class ConfigFileTest
    {
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void FileDoesExist()
        {
            string filepath = "../../../";
            ParkingGarage pg = new ParkingGarage();
            string fileName = filepath + "Config.txt";

            // Log the file being checked
            TestContext?.WriteLine($"Checking for file: '{fileName}'");

            bool fromCall = pg.FileExists(fileName);

            if (fromCall)
            {
                Console.WriteLine($"[PASS] File '{fileName}' exists.");
                TestContext?.WriteLine($"[PASS] File '{fileName}' exists.");
            }
            else
            {
                Console.WriteLine($"[FAIL] File '{fileName}' does not exist.");
                TestContext?.WriteLine($"[FAIL] File '{fileName}' does not exist.");
            }

            // Assert the result
            Assert.IsTrue(fromCall);
        }

        [TestMethod]
        public void FileDoesNotExist()
        {
            string filePath = "../../../";
            ParkingGarage pg = new ParkingGarage();
            string fileName = filePath + "test.txt";

            // Log the file being checked
            TestContext?.WriteLine($"Checking for file: '{fileName}'");

            bool fromCall = pg.FileExists(fileName);

            if (!fromCall)
            {
                Console.WriteLine($"[PASS] File '{fileName}' does not exist.");
                TestContext?.WriteLine($"[PASS] File '{fileName}' does not exist.");
            }
            else
            {
                Console.WriteLine($"[FAIL] File '{fileName}' unexpectedly exists.");
                TestContext?.WriteLine($"[FAIL] File '{fileName}' unexpectedly exists.");
            }

            // Assert the result
            Assert.IsFalse(fromCall);
        }
    }
}
