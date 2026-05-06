import axios from "axios";
const API_URL = "/api/admin/Airport";

export const getAirports = async ()=>
{
    try {
        const response = await axios.get(`${API_URL}/GetAll`);
        return response.data;
    } catch (error) {
        console.error("Error al obtener Aeropuertos:", error);
        throw error;
    }
};

export const getAirportsPaginated = async (pageNumber = 1, pageSize = 10) => {
    try {
        const response = await axios.get(`${API_URL}/GetPaginated`, {
            params: { pageNumber, pageSize }
        });
        return response.data;
    } catch (error) {
        console.error("Error al obtener aeropuertos paginados:", error);
        throw error;
    }
};

export const createAirport = async (airportData) => {
    try {
        const response = await axios.post(API_URL, airportData);
        return response.data;
    } catch (error) {
        console.error("Error al crear aeropuerto:", error);
        throw error;
    }
};
