<script setup>
import { computed, onMounted, ref } from "vue";
import PublicNavBar from "../../../shared/PublicNavBar.vue";
import { useFlightRoute } from "../../route/composable/useFlightRoute";

const {
    fetchRoutesPaginated,
    changePage,
    selectRoute,
    routes,
    selectedRouteId,
    pageNumber,
    totalPages,
    totalRecords,
} = useFlightRoute();

const loading = ref(false);
const error = ref("");

const selectedRoute = computed(() =>
    (routes.value ?? []).find((route) => route.flightRouteId === selectedRouteId.value) ?? null
);

const selectedDepartureAirport = computed(() => selectedRoute.value?.departureAirport ?? "");
const selectedArrivalAirport = computed(() => selectedRoute.value?.arrivalAirport ?? "");

const canGoPrev = computed(() => pageNumber.value > 1 && !loading.value);
const canGoNext = computed(() => pageNumber.value < totalPages.value && !loading.value);

onMounted(async () => {
    await loadPage(1);
});

async function loadPage(page) {
    try {
        loading.value = true;
        error.value = "";
        await fetchRoutesPaginated(page, 10);
    } catch (err) {
        const status = err?.response?.status;
        const message = err?.response?.data?.message;
        error.value = message
            ? `No se pudieron cargar las rutas de vuelo (${status ?? "sin status"}): ${message}`
            : `No se pudieron cargar las rutas de vuelo (${status ?? "sin status"})`;
    } finally {
        loading.value = false;
    }
}

async function previousPage() {
    if (!canGoPrev.value) return;
    await changePage(pageNumber.value - 1);
}

async function nextPage() {
    if (!canGoNext.value) return;
    await changePage(pageNumber.value + 1);
}

function chooseRoute(routeId) {
    selectRoute(routeId);
}
</script>

<template>
    <div class="flex flex-col">
        <PublicNavBar />

        <main class="flex-1 pb-8">
            <div class="page-shell">
                <section class="form-card">
                    <header class="mb-6">
                        <h1 class="text-2xl font-bold text-[var(--color-primary)]">Seleccionar Ruta de Vuelo</h1>
                        <p class="text-sm text-gray-600">
                            Elige una ruta para autocompletar el aeropuerto de salida y llegada en el siguiente paso.
                        </p>
                    </header>

                    <div v-if="error" class="mb-4 rounded-md border border-red-300 bg-red-50 p-3 text-sm text-red-700">
                        {{ error }}
                    </div>

                    <div class="overflow-x-auto rounded-lg border border-gray-200">
                        <table class="min-w-full bg-white text-sm">
                            <thead class="bg-gray-100 text-left text-xs uppercase tracking-wide text-gray-600">
                                <tr>
                                    <th class="px-4 py-3">ID</th>
                                    <th class="px-4 py-3">Salida</th>
                                    <th class="px-4 py-3">Llegada</th>
                                    <th class="px-4 py-3">Duracion (s)</th>
                                    <th class="px-4 py-3">Accion</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-if="loading">
                                    <td class="px-4 py-4 text-gray-500" colspan="5">Cargando rutas...</td>
                                </tr>
                                <tr
                                    v-for="route in routes"
                                    :key="route.flightRouteId"
                                    class="border-t border-gray-100"
                                >
                                    <td class="px-4 py-3">{{ route.flightRouteId }}</td>
                                    <td class="px-4 py-3 font-medium">{{ route.departureAirport }}</td>
                                    <td class="px-4 py-3 font-medium">{{ route.arrivalAirport }}</td>
                                    <td class="px-4 py-3">{{ route.estimatedDuration }}</td>
                                    <td class="px-4 py-3">
                                        <button
                                            type="button"
                                            class="rounded-md border border-[var(--color-primary)] px-3 py-1 text-xs font-semibold text-[var(--color-primary)] transition hover:bg-[var(--color-primary)] hover:text-white"
                                            @click="chooseRoute(route.flightRouteId)"
                                        >
                                            Seleccionar
                                        </button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="mt-4 flex items-center justify-between">
                        <p class="text-xs text-gray-600">
                            Pagina {{ pageNumber }} de {{ totalPages || 1 }} · {{ totalRecords }} rutas
                        </p>

                        <div class="flex gap-2">
                            <button
                                type="button"
                                class="rounded-md border px-3 py-1 text-sm"
                                :disabled="!canGoPrev"
                                @click="previousPage"
                            >
                                Anterior
                            </button>
                            <button
                                type="button"
                                class="rounded-md border px-3 py-1 text-sm"
                                :disabled="!canGoNext"
                                @click="nextPage"
                            >
                                Siguiente
                            </button>
                        </div>
                    </div>

                    <div class="mt-6 grid gap-3 sm:grid-cols-2">
                        <label class="text-sm font-medium text-gray-700">
                            Aeropuerto de salida
                            <input
                                :value="selectedDepartureAirport"
                                type="text"
                                readonly
                                class="form-input mt-1 bg-gray-100"
                            />
                        </label>

                        <label class="text-sm font-medium text-gray-700">
                            Aeropuerto de llegada
                            <input
                                :value="selectedArrivalAirport"
                                type="text"
                                readonly
                                class="form-input mt-1 bg-gray-100"
                            />
                        </label>
                    </div>
                </section>
            </div>
        </main>
    </div>
</template>