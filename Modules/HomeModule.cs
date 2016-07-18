using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ViewEngines.Razor;

namespace Airplane
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get ["/"]= _ =>{
        return View ["index.cshtml"];
      };
      Get ["/flights"]= _ => {
        List<Flight> AllFlights = Flight.GetAll();
        return View["flights.cshtml", AllFlights];
      };
      Get ["/cities"]= _ =>{
        List<City> allCities = City.GetAll();
        return View ["cities.cshtml", allCities];
      };
      Get ["flights/new"]= _ =>{
        return View ["flight_form.cshtml"];
      };
      Post ["flights/new"]= _ =>{
        Flight newFlight = new Flight(Request.Form["flight-description"],Request.Form["day"]);
        newFlight.Save();
        List<Flight> AllFlights = Flight.GetAll();
        return View ["flights.cshtml", AllFlights];
      };
      Get ["cities/new"]= _ =>{
        return View ["cities_form.cshtml"];
      };
      Post ["cities/new"]= _ =>{
        City newCity = new City(Request.Form["city-name"]);
        newCity.Save();
        List<City> allCities = City.GetAll();
        return View ["cities.cshtml", allCities];
      };
      Get["flights/{id}"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Flight SelectedFlight = Flight.Find(parameters.id);
        List<City> FlightCities = SelectedFlight.GetDepartureCities();
        List<City> AllCities = City.GetAll();
        model.Add("flight", SelectedFlight);
        model.Add("flightCities", FlightCities);
        model.Add("allCities", AllCities);
        return View["flight.cshtml", model];
      };

      Get["cities/{id}"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>();
        City SelectedCity = City.Find(parameters.id);
        List<Flight> CityFlights = SelectedCity.GetFlightsByDepartureCity();
        List<Flight> AllFlights = Flight.GetAll();
        model.Add("city", SelectedCity);
        model.Add("cityFlights", CityFlights);
        model.Add("allFlights", AllFlights);
        return View["city.cshtml", model];
      };
      Post["flight/add_city"] = _ => {
        City city = City.Find(Request.Form["city-id"]);
        Flight flight = Flight.Find(Request.Form["flight-id"]);
        flight.AddDepartureCity(city);
        return View["success.cshtml"];
      };
      Post["city/add_flight"] = _ => {
        City city = City.Find(Request.Form["city-id"]);
        Flight flight = Flight.Find(Request.Form["flight-id"]);
        city.AddFlight(flight);
        return View["success.cshtml"];
      };
      Post ["/flightComplete"]= _ =>{
        // flightName = (Request.Form["flight-id"]);
        // flightBool =(Request.Form["flight-complete"]);
        // Flight flight = new Flight (flightName, flightBool);
        // flight.Save();
        Flight flight = Flight.Find(Request.Form["GetId"]);
        flight.Update(Request.Form["flight-complete"]);
        return View["success.cshtml"];
      };
    }
  }
}
