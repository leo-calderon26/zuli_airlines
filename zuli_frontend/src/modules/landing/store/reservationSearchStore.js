import { defineStore } from "pinia";
import { ref } from "vue";
import { searchReservation, downloadItineraryPdf } from "../service/reservationSearchService";

export const useReservationSearchStore = defineStore("reservationSearch", () => {
    const reservationData = ref(null);
    const isLoading = ref(false);
    const error = ref(null);
    const isDownloading = ref(false);
    const lastSearchedName = ref("");

    const performSearch = async (reservationCode, lastName) => {
        isLoading.value = true;
        error.value = null;
        reservationData.value = null;
        lastSearchedName.value = lastName;
        
        try {
            reservationData.value = await searchReservation(reservationCode, lastName);
        } catch (err) {
            error.value = err.response?.data?.detail || err.response?.data?.message || "Error al buscar la reservación. Verifica tus datos.";
        } finally {
            isLoading.value = false;
        }
    };

    const downloadItinerary = async (reservationCode, lastName) => {
        isDownloading.value = true;
        error.value = null;
        try {
            const blob = await downloadItineraryPdf(reservationCode, lastName);
            const url = window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `Itinerario_${reservationCode}.pdf`);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        } catch (err) {
            error.value = "Error al descargar el itinerario.";
        } finally {
            isDownloading.value = false;
        }
    };

    const clearSearch = () => {
        reservationData.value = null;
        error.value = null;
        lastSearchedName.value = "";
    };

    return {
        reservationData,
        isLoading,
        isDownloading,
        error,
        lastSearchedName,
        performSearch,
        clearSearch,
        downloadItinerary
    };
});