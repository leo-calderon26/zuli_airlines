import axios from "axios";

const PURCHASE_CONFIRMATION_API_URL = "/api/PurchaseConfirmation";

export const getPurchaseConfirmation = async (reservationCode) => {
    try {
        const response = await axios.get(`${PURCHASE_CONFIRMATION_API_URL}/${reservationCode}`);
        return response.data;
    } catch (error) {
        console.error("Error al obtener la confirmación de compra:", error);
        throw error;
    }
};

export const completePurchaseConfirmation = async (reservationCode) => {
    try {
        const response = await axios.post(`${PURCHASE_CONFIRMATION_API_URL}/Complete/${reservationCode}`);
        return response.data;
    } catch (error) {
        console.error("Error al completar la confirmación de compra:", error);
        throw error;
    }
};