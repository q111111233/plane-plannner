using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Airplane
{
  public class City
  {
    private int _id;
    private string _name;

    public City(string Name, int Id = 0)
    {
      _id= Id;
      _name = Name;
    }

    public override bool Equals(System.Object otherCity)
    {
      if (!(otherCity is City))
      {
        return false;
      }
      else
      {
        City newCity = (City) otherCity;
        bool idEquality = this.GetId() == newCity.GetId();
        bool nameEquality = this.GetName() == newCity.GetName();
        return (idEquality && nameEquality);
      }
    }
    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public static List<City> GetAll()
    {
      List<City> allCities = new List<City>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM cities", conn);
      rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        int cityId = rdr.GetInt32(0);
        string cityName = rdr.GetString(1);
        City newCity = new City(cityName, cityId);
        allCities.Add(newCity);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allCities;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO cities (name) OUTPUT INSERTED.id VALUES (@CityName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@CityName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }
    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM cities;", conn);
      cmd.ExecuteNonQuery();
    }
    public static City Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM cities WHERE id = @CityId;", conn);
      SqlParameter cityIdParameter = new SqlParameter();
      cityIdParameter.ParameterName = "CityId";
      cityIdParameter.Value = id.ToString();
      cmd.Parameters.Add(cityIdParameter);
      rdr = cmd.ExecuteReader();

      int foundCityId = 0;
      string foundCityName = null;

      while(rdr.Read())
      {
        foundCityId = rdr.GetInt32(0);
        foundCityName = rdr.GetString(1);
      }
      City foundCity = new City(foundCityName, foundCityId);
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCity;
    }
    public void AddFlight(Flight newFlight)
   {
     SqlConnection conn = DB.Connection();
     conn.Open();

     SqlCommand cmd = new SqlCommand("INSERT INTO cities_flights (departureCity_id, flight_id) VALUES (@CityId, @FlightId)", conn);
     SqlParameter cityIdParameter = new SqlParameter();
     cityIdParameter.ParameterName = "@CityId";
     cityIdParameter.Value = this.GetId();
     cmd.Parameters.Add(cityIdParameter);

     SqlParameter flightIdParameter = new SqlParameter();
     flightIdParameter.ParameterName = "@FlightId";
     flightIdParameter.Value = newFlight.GetId();
     cmd.Parameters.Add(flightIdParameter);

     cmd.ExecuteNonQuery();

     if (conn != null)
     {
       conn.Close();
     }
   }
   public List<Flight> GetFlightsByDepartureCity()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT flight_id FROM cities_flights WHERE departureCity_id = @departureCity_id;", conn);
      SqlParameter cityIdParameter = new SqlParameter();
      cityIdParameter.ParameterName = "@departureCity_id";
      cityIdParameter.Value = this.GetId();
      cmd.Parameters.Add(cityIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> flightIds = new List<int> {};
      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        flightIds.Add(flightId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Flight> flights = new List<Flight> {};
      foreach (int flightId in flightIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand flightQuery = new SqlCommand("SELECT * FROM flights WHERE id = @FlightId;", conn);

        SqlParameter flightIdParameter = new SqlParameter();
        flightIdParameter.ParameterName = "@FlightId";
        flightIdParameter.Value = flightId;
        flightQuery.Parameters.Add(flightIdParameter);

        queryReader = flightQuery.ExecuteReader();
        while(queryReader.Read())
        {
          int thisFlightId = queryReader.GetInt32(0);
          string flightStatus = queryReader.GetString(1);
          DateTime flightDepartureTime = queryReader.GetDateTime(2);

          Flight foundFlight = new Flight(flightStatus, flightDepartureTime, thisFlightId);
          flights.Add(foundFlight);
        }
        if (queryReader != null)
        {
          queryReader.Close();
        }
      }
      if (conn != null)
      {
        conn.Close();
      }
      return flights;
    }

    public List<Flight> GetFlightsByArrivalCity()
     {
       SqlConnection conn = DB.Connection();
       SqlDataReader rdr = null;
       conn.Open();

       SqlCommand cmd = new SqlCommand("SELECT flight_id FROM cities_flights WHERE arrivalCity_id = @darrivalCity_id;", conn);
       SqlParameter cityIdParameter = new SqlParameter();
       cityIdParameter.ParameterName = "@arrivalCity_id";
       cityIdParameter.Value = this.GetId();
       cmd.Parameters.Add(cityIdParameter);

       rdr = cmd.ExecuteReader();

       List<int> flightIds = new List<int> {};
       while(rdr.Read())
       {
         int flightId = rdr.GetInt32(0);
         flightIds.Add(flightId);
       }
       if (rdr != null)
       {
         rdr.Close();
       }

       List<Flight> flights = new List<Flight> {};
       foreach (int flightId in flightIds)
       {
         SqlDataReader queryReader = null;
         SqlCommand flightQuery = new SqlCommand("SELECT * FROM flights WHERE id = @FlightId;", conn);

         SqlParameter flightIdParameter = new SqlParameter();
         flightIdParameter.ParameterName = "@FlightId";
         flightIdParameter.Value = flightId;
         flightQuery.Parameters.Add(flightIdParameter);

         queryReader = flightQuery.ExecuteReader();
         while(queryReader.Read())
         {
           int thisFlightId = queryReader.GetInt32(0);
           string flightStatus = queryReader.GetString(1);
           DateTime flightDepartureTime = queryReader.GetDateTime(2);

           Flight foundFlight = new Flight(flightStatus, flightDepartureTime, thisFlightId);
           flights.Add(foundFlight);
         }
         if (queryReader != null)
         {
           queryReader.Close();
         }
       }
       if (conn != null)
       {
         conn.Close();
       }
       return flights;
     }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM cities WHERE id = @CityId; DELETE FROM cities_flights WHERE departureCity_id = @CityId;", conn);
      SqlParameter cityIdParameter = new SqlParameter();
      cityIdParameter.ParameterName = "@CityId";
      cityIdParameter.Value = this.GetId();

      cmd.Parameters.Add(cityIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
