import axios from "axios";

const API_URL = "/api/Location";

export const getCountries = async () => {
    try {
        const response = await axios.get(`${API_URL}/countries`);
        return response.data;
    } catch (error) {
        console.error("Error al obtener países:", error);
        throw error;
    }
};

export const getCitiesByCountry = async (countryId) => {
    try {
        const response = await axios.get(`${API_URL}/cities/${countryId}`);
        return response.data;
    } catch (error) {
        console.error("Error al obtener ciudades:", error);
        throw error;
    }
};