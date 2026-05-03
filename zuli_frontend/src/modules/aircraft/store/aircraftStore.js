import { defineStore } from "pinia";
import { ref } from "vue";

const createAircraftPayload = (aircraft = {}) => ({
    model: aircraft.model ?? "",
    //capacity: aircraft.capacity ?? 0,
    weight: aircraft.weight ?? 0,
    numberEconomyClassRows: aircraft.numberEconomyClassRows ?? 0,
    numberSeatingRowsEconomy: aircraft.numberSeatingRowsEconomy ?? 0,
    numberFirstClassRows: aircraft.numberFirstClassRows ?? 0,
    numberSeatingRowsFirst: aircraft.numberSeatingRowsFirst ?? 0,
    baggageCapacity: aircraft.baggageCapacity ?? 0,
});

export const useAircraftStore = defineStore("aircraft", () => {
    const aircrafts = ref([]);
    const pageNumber = ref(1);
    const pageSize = ref(10);
    const totalRecords = ref(0);
    const totalPages = ref(0);

    const setAircraft = (aircraft) => {
        aircrafts.value = [createAircraftPayload(aircraft)];
    };

    const addAircraft = (aircraft) => {
        aircrafts.value.push(createAircraftPayload(aircraft));
    };

    const setPaginatedData = (data, page, size, total, pages) => {
        aircrafts.value = data.map(createAircraftPayload);
        pageNumber.value = page;
        pageSize.value = size;
        totalRecords.value = total;
        totalPages.value = pages;
    };

    const setPageNumber = (page) => {
        pageNumber.value = page;
    };

    return {
        aircrafts,
        pageNumber,
        pageSize,
        totalRecords,
        totalPages,
        setAircraft,
        addAircraft,
        setPaginatedData,
        setPageNumber,
    };
});
