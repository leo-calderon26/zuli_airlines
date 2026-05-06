import { createAirport } from "../service/airportService";
import { useAirportStore } from "../store/airportStore";

export function useAirport() {
    const store = useAirportStore();

    const addAirport = async (airportData) => {
        const newAirport = await createAirport(airportData);
        store.addAirport(newAirport);
        return newAirport;
    };

    return {
        addAirport,
        airports: store.airports
    };
}