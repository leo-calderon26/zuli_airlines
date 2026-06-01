import axios from "axios";

const TICKET_API_URL = "/api/Ticket";
const FLIGHT_API_URL = "/api/Flight";

export const purchaseTickets = async (payload) => {
    try {
        const response = await axios.post(`${TICKET_API_URL}/purchase`, payload);
        return response.data;
    } catch (error) {
        console.error("Error al procesar la compra:", error);
        throw error;
    }
};

export const getFlightDetails = async (flightId) => {
    try {
        const response = await axios.get(`${FLIGHT_API_URL}/${flightId}`);
        return response.data;
    } catch (error) {
        console.error("Error al obtener detalles del vuelo:", error);
        throw error;
    }
};

export const getConfirmation = async (code) => {
    try {
        const response = await axios.get(`${TICKET_API_URL}/confirmation/${code}`);
        return response.data;
    } catch (error) {
        console.error("Error al obtener confirmación:", error);
        throw error;
    }
};
