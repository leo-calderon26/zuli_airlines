<script setup>
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';
import AirportForm from '../components/AirportForm.vue';
import AirportNavBar from '../components/AirportNavBar.vue';
import { useAirportStore } from '../store/airportStore';

const route = useRoute();
const airportStore = useAirportStore();

const airportCode = computed(() => String(route.params.airportCode ?? ''));
const readCachedAirport = () => {
    const historyAirport = window.history.state?.airport;

    if (historyAirport?.airportCode === airportCode.value) {
        return historyAirport;
    }

    const cachedAirport = sessionStorage.getItem('airportEditData');

    if (!cachedAirport) {
        return airportStore.airports.find((airport) => airport.airportCode === airportCode.value) ?? null;
    }

    try {
        const parsedAirport = JSON.parse(cachedAirport);

        if (parsedAirport?.airportCode === airportCode.value) {
            return parsedAirport;
        }
    } catch {
        sessionStorage.removeItem('airportEditData');
    }

    return airportStore.airports.find((airport) => airport.airportCode === airportCode.value) ?? null;
};

const selectedAirport = ref(readCachedAirport());
const isLoading = ref(false);
const notFoundMessage = ref('');

if (!selectedAirport.value) {
    notFoundMessage.value = 'No se encontró el aeropuerto que deseas editar. Regresa a la lista y abre Edit desde allí.';
}
</script>

<template>
    <div class="flex flex-col">
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <div v-if="isLoading" class="rounded-lg border border-gray-200 bg-white p-8 text-center text-gray-600 shadow-sm">
                    Cargando aeropuerto...
                </div>

                <AirportForm
                    v-else-if="selectedAirport"
                    :airport="selectedAirport"
                    :is-edit="true"
                />

                <div v-else class="rounded-lg border border-gray-200 bg-white p-8 text-center text-gray-600 shadow-sm">
                    {{ notFoundMessage }}
                </div>
            </div>
        </main>
    </div>
</template>