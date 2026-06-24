import axios from "axios";

const API_URL = "/api/admin/FlightsReport";

export const getFlightsReport = async (filtros = {}) => {
    try {
        const response = await axios.get(`${API_URL}/GetFlightsReport`, {
            params: filtros,
            withCredentials: true
        });
        return response.data; 
    } catch (error) {
        console.error("Error al obtener el reporte de vuelos:", error);
        throw error;
    }
};

export const getFilterOptions = async () => {
    try {
        const response = await axios.get('/api/FilterOptions/filteroptions', {
            withCredentials: true
        });
        return response.data; 
    } catch (error) {
        console.error("Error al obtener las opciones de filtros:", error);
        throw error;
    }
};