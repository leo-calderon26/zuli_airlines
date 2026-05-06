import axios from "axios";
const API_URL = "/api/Airport";

export const createAirport = async (airportData) => {
    try {
        const response = await axios.post(API_URL, airportData);
        return response.data;
    } catch (error) {
        console.error("Error al crear aeropuerto:", error);
        throw error;
    }
};
