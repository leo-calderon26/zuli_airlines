import axios from "axios";

const FLIGHT_API_URL = "/api/Flight";
const AIRPORT_API_URL = "/api/admin/Airport";

export const searchAirportSuggestions = async (query) => {
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

export const searchFlights = async (searchParams) => {
    try {
        const response = await axios.get(`${FLIGHT_API_URL}/search`, {
            params: searchParams
        });
        return response.data;
    } catch (error) {
        console.error("Error al buscar vuelos:", error);
        throw error;
    }
};

export const checkFlightAvailability = async (availabilityData) => {
    try {
        const response = await axios.post(`${FLIGHT_API_URL}/check-availability`, availabilityData);
        return response.data;
    } catch (error) {
        console.error("Error al verificar disponibilidad:", error);
        throw error;
    }
};
