import { ref } from "vue"
import { getAirports } from "../service/airportService"

export function useAirport() {
    const airports = ref([])

    const fetchAirports = async () => {
        const data = await getAirports()
        airports.value = data
    }

    return {
        airports,
        fetchAirports,
    }
}