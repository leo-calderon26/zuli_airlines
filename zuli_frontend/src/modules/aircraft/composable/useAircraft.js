import { getAircrafts, createAircraft, getAircraftsPaginated } from "../service/aircraftService";
import { useAircraftStore } from "../store/aircraftStore";

export function useAircraft() {
    const store = useAircraftStore();
    
    const fetchAircrafts = async () => {
        const data = await getAircrafts();
        store.aircrafts = data;
    };

    const fetchAircraftsPaginated = async (pageNumber = 1, pageSize = 10) => {
        const response = await getAircraftsPaginated(pageNumber, pageSize);
        store.setPaginatedData(
            response.data,
            response.pageNumber,
            response.pageSize,
            response.totalRecords,
            response.totalPages
        );
    };

    const addAircraft = async (aircraftData) => {
        const newAircraft = await createAircraft(aircraftData);
        store.addAircraft(newAircraft);
        return newAircraft;
    };

    const changePage = async (pageNumber) => {
        await fetchAircraftsPaginated(pageNumber, store.pageSize);
    };

    return {
        fetchAircrafts,
        fetchAircraftsPaginated,
        addAircraft,
        changePage,
        aircrafts: store.aircrafts
    };
}