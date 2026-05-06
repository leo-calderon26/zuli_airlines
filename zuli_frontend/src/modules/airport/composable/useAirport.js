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
        const newAirport = await createAirport(airportData);
        store.addAirport(newAirport);
        return newAirport;
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
        airports: store.airports
    };
}
