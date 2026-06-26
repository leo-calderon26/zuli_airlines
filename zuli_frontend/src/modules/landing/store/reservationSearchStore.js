import { defineStore } from "pinia";
import { ref } from "vue";
import { searchReservation } from "../service/reservationSearchService";

export const useReservationSearchStore = defineStore("reservationSearch", () => {
    const reservationData = ref(null);
    const isLoading = ref(false);
    const error = ref(null);

    const performSearch = async (reservationCode, lastName) => {
        isLoading.value = true;
        error.value = null;
        reservationData.value = null;
        
        try {
            reservationData.value = await searchReservation(reservationCode, lastName);
        } catch (err) {
            error.value = err.response?.data?.detail || err.response?.data?.message || "Error al buscar la reservación. Verifica tus datos.";
        } finally {
            isLoading.value = false;
        }
    };

    const clearSearch = () => {
        reservationData.value = null;
        error.value = null;
    };

    return {
        reservationData,
        isLoading,
        error,
        performSearch,
        clearSearch
    };
});