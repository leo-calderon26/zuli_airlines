<script setup>
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';
import AirportForm from '../components/AirportForm.vue';
import AirportNavBar from '../components/AirportNavBar.vue';
import { useAirport } from '../composable/useAirport';
import { useAirportStore } from '../store/airportStore';

const route = useRoute();
const { fetchAirport } = useAirport();
const airportStore = useAirportStore();

const airportCode = computed(() => String(route.params.airportCode ?? ''));
const readCachedAirport = () => {
    const cachedAirport = sessionStorage.getItem('airportEditData');

    if (!cachedAirport) {
        return null;
    }

    try {
        const parsedAirport = JSON.parse(cachedAirport);

        return parsedAirport?.airportCode === airportCode.value ? parsedAirport : null;
    } catch {
        sessionStorage.removeItem('airportEditData');
        return null;
    }
};

const selectedAirport = ref(readCachedAirport());
const isLoading = ref(!selectedAirport.value);
const notFoundMessage = ref('');

onMounted(async () => {
    notFoundMessage.value = '';

    if (selectedAirport.value) {
        return;
    }

    isLoading.value = true;

    try {
        const airport = await fetchAirport(airportCode.value);

        selectedAirport.value = airport;
        sessionStorage.setItem('airportEditData', JSON.stringify(airport));
    } catch (error) {
        if (!selectedAirport.value && error?.response?.status === 404) {
            notFoundMessage.value = 'No se encontró el aeropuerto que deseas editar. Regresa a la lista y abre Edit desde allí.';
            return;
        }

        if (!selectedAirport.value) {
            notFoundMessage.value = 'No se pudo cargar el aeropuerto para editar. Intenta de nuevo desde la lista.';
        }
    } finally {
        isLoading.value = false;
    }
});
</script>

<template>
    <div class="flex flex-col">
        <PublicNavBar/>
        <AirportNavBar/>
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