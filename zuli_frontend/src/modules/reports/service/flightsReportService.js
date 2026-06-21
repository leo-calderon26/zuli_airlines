import axios from "axios";

const API_URL = "/api/admin/FlightsReport";

export const getFlightsReport = async () => {
    try {
        const response = await axios.get(`${API_URL}/GetFlightsReport`, {
            withCredentials: true
        });
        return response.data; 
    } catch (error) {
        console.error("Error al obtener el reporte de vuelos:", error);
        throw error;
    }
};