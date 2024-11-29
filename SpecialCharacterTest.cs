using PragueParkingClassLibrary;
namespace PragueParkingTests
{
    [TestClass]
    public class SpecialCharacterTest
    {
        [TestMethod]
        public void StringDoesContainSpecialCharacter()
        {

            var parkingGarage = new ParkingGarage();
            string stringToTest = "Test hej!";
            bool fromCall;


            fromCall = parkingGarage.ContainsSpecialCharacters(stringToTest);

            Assert.IsTrue(fromCall);
        }
        [TestMethod]
        public void StringDoesNotContainSpecialCharacter()
        {

            var parkingGarage = new ParkingGarage();
            string stringToTest = "HejHopp";
            bool fromCall;

            fromCall = parkingGarage.ContainsSpecialCharacters(stringToTest);

            Assert.IsFalse(fromCall);
        }
    }
}
