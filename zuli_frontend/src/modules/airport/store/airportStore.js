import { defineStore } from "pinia";
import { ref } from "vue";


const createAirportPayload = (airport = {}) => ({
    airportCode: airport.airportCode ?? "",
    name: airport.name ?? "",
    country: airport.country ?? "",
    city: airport.city ?? "",

    adminId: airport.adminId ?? "", 
});

export const useAirportStore = defineStore("airport", () => {

    const airports = ref([]);
    
    const pageNumber = ref(1);
    const pageSize = ref(10);
    const totalRecords = ref(0);
    const totalPages = ref(0);

    const setPaginatedData = (data, page, size, total, pages) => {
        airports.value = data.map(createAirportPayload);
        pageNumber.value = page;
        pageSize.value = size;
        totalRecords.value = total;
        totalPages.value = pages;
    };


    const addAirport = (airport) => {
        airports.value.push(createAirportPayload(airport));
    };

    return {
        airports,
        pageNumber,
        pageSize,
        totalRecords,
        totalPages,
        setPaginatedData,
        addAirport,
    };
});