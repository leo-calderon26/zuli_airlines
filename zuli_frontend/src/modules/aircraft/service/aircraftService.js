import axios from "axios"
const API_URL = "/api/Aircraft"

export const getAircrafts = async ()=>
{
    try {
        const response = await axios.get(`${API_URL}/GetAll`);
        return response.data;
    } catch (error) {
        console.error("Error al obtener aeronaves:", error);
        throw error;
    }
};

export const getAircraftsPaginated = async (pageNumber = 1, pageSize = 10) => {
    try {
        const response = await axios.get(`${API_URL}/GetPaginated`, {
            params: {
                pageNumber,
                pageSize
            }
        });
        return response.data;
    } catch (error) {
        console.error("Error al obtener aeronaves paginadas:", error);
        throw error;
    }
};

export const createAircraft = async (aircraftData) => {
    try {
        const response = await axios.post(`${API_URL}/CreateAircraft`, aircraftData);
        return response.data;
    } catch (error) {
        console.error("Error al crear aeronave:", error);
        throw error;
    }
};
