import { defineStore } from "pinia";
import { ref } from "vue";

const createAirportPayload = (airport = {}) => ({
    airportCode: airport.airportCode ?? "",
    name: airport.name ?? "",
    country: airport.country ?? "",
    city: airport.city ?? "",
    adminId: airport.adminId ?? "",
});

export const useAirportStore = defineStore("airport", () => {
    const airports = ref([]);

    const addAirport = (airport) => {
        airports.value.push(createAirportPayload(airport));
    };

    return {
        airports,
        addAirport,
    };
});