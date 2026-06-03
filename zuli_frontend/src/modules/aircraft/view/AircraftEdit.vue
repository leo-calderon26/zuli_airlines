<script setup>
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';
import AircraftForm from '../components/AircraftForm.vue';
import { useAircraftStore } from '../store/aircraftStore';

const route = useRoute();
const aircraftStore = useAircraftStore();

const aircraftId = computed(() => String(route.params.aircraftId ?? ''));
const readCachedAircraft = () => {
    const historyAircraft = window.history.state?.aircraft;

    if (historyAircraft?.aircraftId === aircraftId.value) {
        return historyAircraft;
    }

    const cachedAircraft = sessionStorage.getItem('aircraftEditData');

    if (cachedAircraft) {
        try {
            const parsedAircraft = JSON.parse(cachedAircraft);

            if (parsedAircraft?.aircraftId === aircraftId.value) {
                return parsedAircraft;
            }
        } catch {
            sessionStorage.removeItem('aircraftEditData');
        }
    }

    return aircraftStore.aircrafts.find((aircraft) => aircraft.aircraftId === aircraftId.value) ?? null;
};

const selectedAircraft = ref(readCachedAircraft());
const isLoading = ref(false);
const notFoundMessage = ref('');

if (!selectedAircraft.value) {
    notFoundMessage.value = 'No se encontró la aeronave que deseas editar. Regresa a la lista y abre Edit desde allí.';
}
</script>

<template>
    <div class="flex flex-col">
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <div v-if="isLoading" class="rounded-lg border border-gray-200 bg-white p-8 text-center text-gray-600 shadow-sm">
                    Cargando aeronave...
                </div>

                <AircraftForm
                    v-else-if="selectedAircraft"
                    :aircraft="selectedAircraft"
                    :is-edit="true"
                />

                <div v-else class="rounded-lg border border-gray-200 bg-white p-8 text-center text-gray-600 shadow-sm">
                    {{ notFoundMessage }}
                </div>
            </div>
        </main>
    </div>
</template>
