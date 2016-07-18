using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace Airplane
{
  public class FlightTest : IDisposable
  {
    public FlightTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=airplane_planner_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_AddCity_AddsCityToFlight()
    {
      //Arrange
      Flight testFlight = new Flight("Mow the lawn", new DateTime(2016, 5, 4));
      testFlight.Save();

      City testCity = new City("Home stuff");
      testCity.Save();

      //Act
      testFlight.AddDepartureCity(testCity);

      List<City> result = testFlight.GetDepartureCities();
      List<City> testList = new List<City>{testCity};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetCities_ReturnsAllFlightCities()
    {
      //Arrange
      Flight testFlight = new Flight("Mow the lawn", new DateTime(2016, 5, 4));
      testFlight.Save();

      City testCity1 = new City("Home stuff");
      testCity1.Save();

      City testCity2 = new City("Work stuff");
      testCity2.Save();

      //Act
      testFlight.AddDepartureCity(testCity1);
      List<City> result = testFlight.GetDepartureCities();
      List<City> testList = new List<City> {testCity1};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Delete_DeletesFlightAssociationsFromDatabase()
    {
      //Arrange
      City testCity = new City("Home stuff");
      testCity.Save();

      string testDescription = "Mow the lawn";
      DateTime testDuedate = new DateTime(2016, 5, 4);
      Flight testFlight = new Flight(testDescription, testDuedate);
      testFlight.Save();

      //Act
      testFlight.AddDepartureCity(testCity);
      testFlight.Delete();

      List<Flight> resultCityFlights = testCity.GetFlightsByDepartureCity();
      List<Flight> testCityFlights = new List<Flight> {};

      //Assert
      Assert.Equal(testCityFlights, resultCityFlights);
    }
    public void Dispose()
    {
      Flight.DeleteAll();
      City.DeleteAll();
    }
  }
}
