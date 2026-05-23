import { ref } from "vue"
import { getAirports, getAirportsPaginated, createAirport } from "../service/airportService"
import { useAirportStore } from "../store/airportStore";
import { getAirport, updateAirport as updateAirportRequest } from "../service/airportService";

export function useAirport() {
    const store = useAirportStore();
    const airports = ref([])

    const fetchAirports = async () => {
        const data = await getAirports()
        airports.value = data
    }

    const fetchAirport = async (code) => {
            return await getAirport(code);
    };

    const updateAirport = async (code, airportData) => {
        await updateAirportRequest(code, airportData);
        store.updateAirport(code, airportData);
        return airportData;
    };

    const fetchAirportsPaginated = async (pageNumber = 1, pageSize = 10) => {
        const response = await getAirportsPaginated(pageNumber, pageSize);
        store.setPaginatedData(
            response.data,
            response.pageNumber,
            response.pageSize,
            response.totalRecords,
            response.totalPages
        );
    };

    const addAirport = async (airportData) => {
        await createAirport(airportData);
        store.addAirport(airportData);
        return airportData;
    };

    const changePage = async (pageNumber) => {
        await fetchAirportsPaginated(pageNumber, store.pageSize);
    };

    return {
        airports,
        fetchAirports,
        addAirport,
        fetchAirportsPaginated,
        changePage,
        fetchAirport,
        updateAirport
    };
}
