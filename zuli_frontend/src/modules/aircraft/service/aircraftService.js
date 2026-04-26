import axios from "axios"
const API_URL = "/api/Aircraft/GetAll"

export const getAircrafts = async ()=>
{
    try {
        const response = await axios.get(API_URL);
        return response.data;
    } catch (error) {
        console.error("Error al obtener aeronaves:", error);
        throw error;
    }
};