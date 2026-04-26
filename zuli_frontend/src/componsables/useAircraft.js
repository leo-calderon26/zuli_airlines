import { useAircraftStore } from "../stores/aircraftStrore";
import { getAircrafts } from "../helpers/getAircraft";

// funcion para que la api y la store se comuniquen
export const useAircraft = async () => {
    const aircrafts = await getAircrafts(); 
    const aircraftStrore = useAircraftStore();
    aircraftStrore.aircrafts = aircrafts;
}