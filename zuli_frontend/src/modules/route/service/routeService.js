import axios from "axios";

const ROUTE_API_URL = "/api/admin/FlightRoute";
const AIRPORT_API_URL = "/api/admin/Airport";

export const createFlightRoute = async (routeData) => {
  try {
    const response = await axios.post(`${ROUTE_API_URL}/CreateFlightRoute`, routeData);
    return response.data;
  } catch (error) {
    console.error("Error al crear ruta:", error);
    throw error;
  }
};

export const searchAirportSuggestionsByName = async (query) => {
  try {
    const response = await axios.get(`${AIRPORT_API_URL}/suggestions`, {
      params: { query },
    });
    return response.data;
  } catch (error) {
    console.error("Error al buscar aeropuertos:", error);
    throw error;
  }
};
