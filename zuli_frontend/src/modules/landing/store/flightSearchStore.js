import { defineStore } from "pinia";
import { ref } from "vue";
import { searchFlights, checkFlightAvailability } from "../service/flightSearchService";

export const useFlightSearchStore = defineStore("flightSearch", () => {
    const searchParams = ref({
        Origin: '',
        Destination: '',
        Date: '',
        Seats: 1,
        IsRoundTrip: false,
        ReturnDate: null,
        DirectFlightsOnly: false,
        FlightClass: 'Turista',
        Page: 1,
        PageSize: 2
    });

    const flightResults = ref(null);
    const isLoading = ref(false);
    const error = ref(null);

    const performSearch = async (params) => {
        isLoading.value = true;
        error.value = null;
        searchParams.value = { ...searchParams.value, ...params };
        
        try {
            flightResults.value = await searchFlights(searchParams.value);
        } catch (err) {
            error.value = err.response?.data?.detail || "Error al buscar vuelos";
        } finally {
            isLoading.value = false;
        }
    };

    const changePage = async (newPage) => {
        await performSearch({ Page: newPage });
    };

    const verifyFlightAvailability = async (flight, seats) => {
        isLoading.value = true;
        try {
            const segments = flight.segments.map(s => ({
                flightRouteId: s.flightId, 
                departureDate: s.departureDateText 
            }));

            await checkFlightAvailability({ seats, segments });
            return { isAvailable: true };

        } catch (err) {
            if (err.response?.status === 422 || err.response?.status === 404) {
                return { isAvailable: false };
            }
            error.value = "Ocurrió un error al verificar la disponibilidad.";
            return { isAvailable: false, isError: true };
        } finally {
            isLoading.value = false;
        }
    };

    const getFlightRouteData = async (flight) => {
        const segments = flight.segments.map(s => ({
            flightRouteId: s.flightId, 
            departureDate: s.departureDateText
        }));
        return segments;
    };


    return {
        searchParams,
        flightResults,
        isLoading,
        error,
        performSearch,
        changePage,
        verifyFlightAvailability,
        getFlightRouteData
    };
});