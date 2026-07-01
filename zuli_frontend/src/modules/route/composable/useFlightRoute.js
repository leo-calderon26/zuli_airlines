import { getFlightRoutesPaginated,deleteFlightRoute as deleteFlightRouteRequest } from "../service/routeService";
import { useFlightRouteStore } from "../store/flightRouteStore";

export function useFlightRoute() {
    const store = useFlightRouteStore();

    const fetchRoutesPaginated = async (pageNumber = 1, pageSize = 10) => {
        const response = await getFlightRoutesPaginated(pageNumber, pageSize);
        store.setPaginatedData(
            response.data,
            response.pageNumber,
            response.pageSize,
            response.totalRecords,
            response.totalPages
        );
    };

    const changePage = async (pageNumber) => {
        await fetchRoutesPaginated(pageNumber, store.pageSize);
    };

    const selectRoute = (routeId) => {
        store.setSelectedRouteId(routeId);
    };
    const deleteRoute = async flightRouteId => {
        const response = await deleteFlightRouteRequest(flightRouteId);

        await fetchRoutesPaginated(
            store.pageNumber,
            store.pageSize
        );

        return response;
    };
    return {
        fetchRoutesPaginated,
        changePage,
        selectRoute,
        deleteRoute,
        routes: store.routes,
        selectedRouteId: store.selectedRouteId,
        pageNumber: store.pageNumber,
        totalPages: store.totalPages,
        totalRecords: store.totalRecords,
    };
}