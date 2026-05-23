import axios from "axios";

const TICKET_API_URL = "/api/Ticket";
const FLIGHT_API_URL = "/api/Flight";

export const getFlightDetails = async (flightId) => {
    const response = await axios.get(`${FLIGHT_API_URL}/${flightId}`);
    return response.data;
};

export const purchaseTickets = async (purchaseData) => {
    const response = await axios.post(`${TICKET_API_URL}/purchase`, purchaseData, {
        withCredentials: true
    });
    return response.data;
};

export const getTicketByConfirmation = async (confirmationCode) => {
    const response = await axios.get(`${TICKET_API_URL}/confirmation/${confirmationCode}`);
    return response.data;
};
