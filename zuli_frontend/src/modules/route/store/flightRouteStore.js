import { defineStore } from "pinia";
import { ref } from "vue";

const createFlightRoutePayload = (route = {}) => ({
    flightRouteId: route.flightRouteId ?? 0,
    departureAirport: route.departureAirport ?? "",
    arrivalAirport: route.arrivalAirport ?? "",
    scheduledDepartureTime: route.scheduledDepartureTime ?? route.ScheduledDepartureTime ?? "",
    scheduledArrivalTime: route.scheduledArrivalTime ?? route.ScheduledArrivalTime ?? "",
    estimatedDuration: route.estimatedDuration ?? 0,
    frequency: route.frequency ?? 0,
    airlineId: route.airlineId ?? 0,
    aircraftId: route.aircraftId ?? null,
    touristPrice: route.touristPrice ?? 0,
    firstClassPrice: route.firstClassPrice ?? 0,
    carryOnPrice: route.carryOnPrice ?? 0,
    checkedPrice: route.checkedPrice ?? 0,
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