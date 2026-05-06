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

export const getFlightRoutesPaginated = async (pageNumber = 1, pageSize = 10) => {
  try {
    const response = await axios.get(`${ROUTE_API_URL}/GetPaginated`, {
      params: {
        pageNumber,
        pageSize,
      },
      // TODO(Leo): esto esta bien
      withCredentials: true,
    });
    return response.data;
  } catch (error) {
    console.error("Error al obtener rutas paginadas:", error);
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
