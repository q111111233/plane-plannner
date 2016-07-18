using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Airplane
{
  public class CityTest : IDisposable
  {
    public CityTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=airplane_planner_test;Integrated Security=SSPI;";
    }
    [Fact]
    public void Test_CitiesEmptyAtFirst()
    {
      int result = City.GetAll().Count;
      Assert.Equal(0, result);
    }
    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      City firstCity = new City("Household chores");
      City secondCity = new City("Household chores");
      Assert.Equal(firstCity, secondCity);
    }
    [Fact]
    public void Test_Save_SavesCityToDatabase()
    {
      City testCity = new City("Household chores");
      testCity.Save();
      List<City> result = City.GetAll();
      List<City> testList = new List<City>{testCity};
      Assert.Equal(testList, result);
    }
    [Fact]
    public void Test_Save_AssignsIdToCityObject()
    {
      City testCity = new City("Household chores");
      testCity.Save();
      City savedCity = City.GetAll()[0];
      int result = savedCity.GetId();
      int testId = testCity.GetId();
      Assert.Equal (testId, result);
    }
    [Fact]
    public void Test_Find_FindsCityInDatabase()
    {
      City testCity = new City("Household chores");
      testCity.Save();
      City foundCity = City.Find(testCity.GetId());
      Assert.Equal(testCity, foundCity);
    }
    [Fact]
    public void Test_AddFlight_AddsFlightToCity()
    {
      //Arrange
      City testCity = new City("Household chores");
      testCity.Save();

      Flight testFlight = new Flight("Mow the lawn", new DateTime(2016, 5, 4));
      testFlight.Save();

      Flight testFlight2 = new Flight("Water the garden", new DateTime(2016, 5, 4));
      testFlight2.Save();

      //Act
      testCity.AddFlight(testFlight);
      testCity.AddFlight(testFlight2);
      List<Flight> result = testCity.GetFlightsByDepartureCity();
      List<Flight> testList = new List<Flight>{testFlight, testFlight2};
      Console.WriteLine(result);

      //Assert
      Assert.Equal(testList, result);
    }
    [Fact]
    public void Test_Delete_DeletesCityAssociationsFromDatabase()
    {
      //Arrange
      Flight testFlight = new Flight("Mow the lawn", new DateTime(2016, 5, 4));
      testFlight.Save();

      string testName = "Home stuff";
      City testCity = new City(testName);
      testCity.Save();

      //Act
      testCity.AddFlight(testFlight);
      testCity.Delete();

      List<City> resultFlightCities = testFlight.GetDepartureCities();
      List<City> testFlightCities = new List<City> {};

      //Assert
      Assert.Equal(testFlightCities, resultFlightCities);
    }
    public void Dispose()
    {
      Flight.DeleteAll();
      City.DeleteAll();
    }
  }
}
