import { ref } from "vue"
import { getAirports, createAirport } from "../service/airportService"
import { useAirportStore } from "../store/airportStore";

export function useAirport() {
    const store = useAirportStore();
    const airports = ref([])

    const fetchAirports = async () => {
        const data = await getAirports()
        airports.value = data
    }

    const addAirport = async (airportData) => {
        const newAirport = await createAirport(airportData);
        store.addAirport(newAirport);
        return newAirport;
    };

    return {
        airports,
        fetchAirports,
        addAirport,
    }
}    