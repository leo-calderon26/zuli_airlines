import { defineStore } from "pinia";
import { ref } from "vue";

const createFlightRoutePayload = (route = {}) => ({
    flightRouteId: route.flightRouteId ?? 0,
    departureAirport: route.departureAirport ?? "",
    arrivalAirport: route.arrivalAirport ?? "",
    estimatedDuration: route.estimatedDuration ?? 0,
    frequency: route.frequency ?? 0,
    airlineId: route.airlineId ?? 0,
});

export const useFlightRouteStore = defineStore("flightRoute", () => {
    const routes = ref([]);
    const selectedRouteId = ref(null);
    const pageNumber = ref(1);
    const pageSize = ref(10);
    const totalRecords = ref(0);
    const totalPages = ref(0);

    const setPaginatedData = (data, page, size, total, pages) => {
        routes.value = (data ?? []).map(createFlightRoutePayload);
        pageNumber.value = page;
        pageSize.value = size;
        totalRecords.value = total;
        totalPages.value = pages;
    };

    const setSelectedRouteId = (routeId) => {
        selectedRouteId.value = routeId;
    };

    return {
        routes,
        selectedRouteId,
        pageNumber,
        pageSize,
        totalRecords,
        totalPages,
        setPaginatedData,
        setSelectedRouteId,
    };
});