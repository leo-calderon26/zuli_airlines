import { getAircrafts, createAircraft, getAircraftsPaginated, updateAircraft as updateAircraftRequest } from "../service/aircraftService";
import { useAircraftStore } from "../store/aircraftStore";

export function useAircraft() {
    const store = useAircraftStore();
    
    const fetchAircrafts = async () => {
        const data = await getAircrafts();
        // store.aircrafts is a ref created in the Pinia store; assign to .value to preserve reactivity
        store.aircrafts.value = data;
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
        const response = await createAircraft(aircraftData);
        await fetchAircraftsPaginated(store.pageNumber, store.pageSize);
        return response;
    };

    const updateAircraft = async (aircraftId, aircraftData) => {
        const response = await updateAircraftRequest(aircraftId, aircraftData);
        await fetchAircraftsPaginated(store.pageNumber, store.pageSize);
        return response;
    };

    const changePage = async (pageNumber) => {
        await fetchAircraftsPaginated(pageNumber, store.pageSize);
    };

    return {
        fetchAircrafts,
        fetchAircraftsPaginated,
        addAircraft,
        updateAircraft,
        changePage,
        aircrafts: store.aircrafts
    };
}