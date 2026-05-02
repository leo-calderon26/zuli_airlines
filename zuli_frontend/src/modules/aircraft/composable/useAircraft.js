import { getAircrafts } from "../service/aircraftService";
import { useAircraftStore } from "../store/aircraftStore";

export function useAircraft() {
    const store = useAircraftStore();
    const fetchAircrafts = async () => {
        const data = await getAircrafts();
        store.aircrafts = data;
    };

    return {
        fetchAircrafts,
        aircrafts: store.aircrafts
    };
}