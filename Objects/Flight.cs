using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Airplane
{
  public class Flight
  {
    private int _id;
    private string _status;
    private DateTime _departureTime;


    public Flight(string Status, DateTime departureTime, int Id = 0)
    {
      _id = Id;
      _status = Status;
      _departureTime = departureTime;
    }

    public override bool Equals(System.Object otherFlight)
    {
        if (!(otherFlight is Flight))
        {
          return false;
        }
        else {
          Flight newFlight = (Flight) otherFlight;
          bool idEquality = this.GetId() == newFlight.GetId();
          bool statusEquality = this.GetStatus() == newFlight.GetStatus();
          return (idEquality && statusEquality);
        }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetStatus()
    {
      return _status;
    }
    public void SetStatus(string newStatus)
    {
      _status = newStatus;
    }
    public DateTime GetFlightDepartureTime()
    {
      return _departureTime;
    }
    public void SetFlightDepartureTime(DateTime newDepartureTime)
    {
      _departureTime = newDepartureTime;
    }
    public static List<Flight> GetAll()
    {
      List<Flight> AllFlights = new List<Flight>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM flights ORDER BY Departure_time", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        string flightStatus = rdr.GetString(1);
        DateTime flightDepartureTime = rdr.GetDateTime(2);
        Flight newFlight = new Flight(flightStatus, flightDepartureTime, flightId);
        AllFlights.Add(newFlight);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return AllFlights;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO flights (status, Departure_time) OUTPUT INSERTED.id VALUES (@FlightStatus, @FlightDepartureTime)", conn);

      SqlParameter statusParam = new SqlParameter();
      statusParam.ParameterName = "@FlightStatus";
      statusParam.Value = this.GetStatus();

      cmd.Parameters.Add(statusParam);


      SqlParameter FlightDepartureTimeParam = new SqlParameter();
      FlightDepartureTimeParam.ParameterName = "@FlightDepartureTime";
      FlightDepartureTimeParam.Value = this.GetFlightDepartureTime();

      cmd.Parameters.Add(FlightDepartureTimeParam);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM flights;", conn);
      cmd.ExecuteNonQuery();
    }

    public static Flight Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM flights WHERE id = @FlightId", conn);
      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = id.ToString();
      cmd.Parameters.Add(flightIdParameter);
      rdr = cmd.ExecuteReader();

      int foundFlightId = 0;
      string foundFlightStatus = null;

      DateTime foundFlightDepartureTime = DateTime.MinValue;

      while(rdr.Read())
      {
        foundFlightId = rdr.GetInt32(0);
        foundFlightStatus = rdr.GetString(1);

        foundFlightDepartureTime = rdr.GetDateTime(2);
      }
      Flight foundFlight = new Flight(foundFlightStatus, foundFlightDepartureTime, foundFlightId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundFlight;
    }

    public void AddDepartureCity(City newDepartureCity)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO Cities_flights (departureCity_id, flight_id) VALUES (@DepartureCityId, @FlightId);", conn);

      SqlParameter departureCityIdParameter = new SqlParameter();
      departureCityIdParameter.ParameterName = "@DepartureCityId";
      departureCityIdParameter.Value = newDepartureCity.GetId();
      cmd.Parameters.Add(departureCityIdParameter);

      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();
      cmd.Parameters.Add(flightIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void AddArrivalCity(City newArrivalCity)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO Cities_flights (arrivalCity_id, flight_id) VALUES (@ArrivalCityId, @FlightId);", conn);

      SqlParameter arrivalCityIdParameter = new SqlParameter();
      arrivalCityIdParameter.ParameterName = "@ArrivalCityId";
      arrivalCityIdParameter.Value = newArrivalCity.GetId();
      cmd.Parameters.Add(arrivalCityIdParameter);

      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();
      cmd.Parameters.Add(flightIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<City> GetDepartureCities()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT departureCity_id FROM cities_flights WHERE flight_id = @FlightId;", conn);

      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();
      cmd.Parameters.Add(flightIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> departureCityIds = new List<int> {};

      while (rdr.Read())
      {
        int departureCityId = rdr.GetInt32(0);
        departureCityIds.Add(departureCityId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<City> cities = new List<City> {};

      foreach (int departureCityId in departureCityIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand departureCityQuery = new SqlCommand("SELECT * FROM cities WHERE id = @CityId;", conn);

        SqlParameter departureCityIdParameter = new SqlParameter();
        departureCityIdParameter.ParameterName = "@CityId";
        departureCityIdParameter.Value = departureCityId;
        departureCityQuery.Parameters.Add(departureCityIdParameter);

        queryReader = departureCityQuery.ExecuteReader();
        while (queryReader.Read())
        {
          int thisCityId = queryReader.GetInt32(0);
          string departureCityName = queryReader.GetString(1);
          City foundCity = new City(departureCityName, thisCityId);
          cities.Add(foundCity);
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
      return cities;
    }

    public List<City> GetArrivalCities()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT arrivalCity_id FROM cities_flights WHERE flight_id = @FlightId;", conn);

      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();
      cmd.Parameters.Add(flightIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> arrivalCityIds = new List<int> {};

      while (rdr.Read())
      {
        int arrivalCityId = rdr.GetInt32(0);
        arrivalCityIds.Add(arrivalCityId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<City> cities = new List<City> {};

      foreach (int arrivalCityId in arrivalCityIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand arrivalCityQuery = new SqlCommand("SELECT * FROM cities WHERE id = @CityId;", conn);

        SqlParameter arrivalCityIdParameter = new SqlParameter();
        arrivalCityIdParameter.ParameterName = "@CityId";
        arrivalCityIdParameter.Value = arrivalCityId;
        arrivalCityQuery.Parameters.Add(arrivalCityIdParameter);

        queryReader = arrivalCityQuery.ExecuteReader();
        while (queryReader.Read())
        {
          int thisCityId = queryReader.GetInt32(0);
          string arrivalCityName = queryReader.GetString(1);
          City foundCity = new City(arrivalCityName, thisCityId);
          cities.Add(foundCity);
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
      return cities;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM flights WHERE id = @FlightId; DELETE FROM cities_flights WHERE flight_id = @FlightId;", conn);
      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();

      cmd.Parameters.Add(flightIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
    public void Update(string newStatus)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE flights SET status = @NewName OUTPUT INSERTED.status WHERE id = @CategoryId;", conn);

      SqlParameter newNameParameter = new SqlParameter();
      newNameParameter.ParameterName = "@NewName";
      newNameParameter.Value = newStatus;
      cmd.Parameters.Add(newNameParameter);


      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = this.GetId();
      cmd.Parameters.Add(categoryIdParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._status = rdr.GetString(0);
      }

      if (rdr != null)
      {
        rdr.Close();
      }

      if (conn != null)
      {
        conn.Close();
      }
    }

  }
}
