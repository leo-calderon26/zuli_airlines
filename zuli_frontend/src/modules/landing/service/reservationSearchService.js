import axios from "axios";

const RESERVATION_SEARCH_API_URL = "/api/ReservationSearch";

export const searchReservation = async (reservationCode, lastName) => {
    try {
        const response = await axios.post(`${RESERVATION_SEARCH_API_URL}/search`, {
            ReservationCode: reservationCode,
            LastName: lastName
        });
        return response.data;
    } catch (error) {
        console.error("Error al buscar la reservación:", error);
        throw error;
    }
};