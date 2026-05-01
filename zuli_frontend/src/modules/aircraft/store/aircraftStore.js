import { defineStore } from "pinia";
import { ref } from "vue";

const createAircraftPayload = (aircraft = {}) => ({
    model: aircraft.model ?? "",
    weight: aircraft.weight ?? 0,
    numberEconomyClassRows: aircraft.numberEconomyClassRows ?? 0,
    numberSeatingRowsEconomy: aircraft.numberSeatingRowsEconomy ?? 0,
    numberFirstClassRows: aircraft.numberFirstClassRows ?? 0,
    numberSeatingRowsFirst: aircraft.numberSeatingRowsFirst ?? 0,
});

export const useAircraftStore = defineStore("aircraft", () => {
    const aircrafts = ref([]);

    const setAircraft = (aircraft) => {
        aircrafts.value = [createAircraftPayload(aircraft)];
    };

    const addAircraft = (aircraft) => {
        aircrafts.value.push(createAircraftPayload(aircraft));
    };

    return {
        aircrafts,
        setAircraft,
        addAircraft,
    };
});
