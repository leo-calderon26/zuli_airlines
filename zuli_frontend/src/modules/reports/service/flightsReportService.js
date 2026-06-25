import axios from "axios";

const API_URL = "/api/admin/FlightsReport";
const EXPORT_URL = "/api/admin/FlightsReport/export";

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

const getFilenameFromHeader = (header) => {
    if (!header) return null;
    const starMatch = header.match(/filename\*=UTF-8''([^;]+)/i);
    if (starMatch) return decodeURIComponent(starMatch[1]);
    const match = header.match(/filename="([^"]+)"/);
    if (match) return match[1];
    return null;
};

export const exportFlightsReport = async (filtros = {}) => {
    try {
        const response = await axios.get(EXPORT_URL, {
            params: filtros,
            withCredentials: true,
            responseType: 'blob'
        });

        const contentDisposition = response.headers['content-disposition'];
        const filename = getFilenameFromHeader(contentDisposition);

        return { 
            blob: response.data,
            filename 
        };
    } catch (error) {
        console.error("Error al exportar el reporte de vuelos:", error);
        throw error;
    }
};