<script setup>
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import { useFlightRoute } from '../../route/composable/useFlightRoute';
import { useFlightRouteStore } from '../../route/store/flightRouteStore';

const router = useRouter();
const flightRouteStore = useFlightRouteStore();
const { fetchFlightRoutesPaginated, changePage } = useFlightRoute();

const daysOfWeek = [
    { value: 'mon', label: 'Lunes' },
    { value: 'tue', label: 'Martes' },
    { value: 'wed', label: 'Miercoles' },
    { value: 'thu', label: 'Jueves' },
    { value: 'fri', label: 'Viernes' },
    { value: 'sat', label: 'Sabado' },
    { value: 'sun', label: 'Domingo' },
];

const dayBits = {
    mon: 1,
    tue: 2,
    wed: 4,
    thu: 8,
    fri: 16,
    sat: 32,
    sun: 64,
};

function decodeDays(mask) {
    return Object.keys(dayBits).filter((day) => (mask & dayBits[day]) !== 0);
}

function formatFrequency(mask) {
    const selected = decodeDays(Number(mask) || 0);
    if (selected.length === 0) return 'Sin frecuencia';
    return daysOfWeek
        .filter((day) => selected.includes(day.value))
        .map((day) => day.label)
        .join(', ');
}

function formatDateTime(value) {
    if (!value) return '-';
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return String(value);
    return date.toLocaleString('es-CR', { dateStyle: 'short', timeStyle: 'short' });
}

function formatDuration(seconds) {
    const totalSeconds = Number(seconds);
    if (!Number.isFinite(totalSeconds) || totalSeconds <= 0) return '-';
    const minutes = Math.round(totalSeconds / 60);
    return `${minutes} min`;
}

function selectRoute(route) {
    if (!route?.flightRouteId) return;
    router.push({
        name: 'flightCreate',
        query: {
            routeId: route.flightRouteId,
            departure: route.departureAirport,
            arrival: route.arrivalAirport,
            duration: route.estimatedDuration,
            frequency: route.frequency,
        },
    });
}

onMounted(async () => {
    await fetchFlightRoutesPaginated(1, 10);
});
</script>

<template>
    <div class="flex flex-col">
        <PublicNavBar />
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <div class="mb-6">
                    <p class="text-sm font-semibold uppercase tracking-[0.2em] text-primary">Modulo de vuelos</p>
                    <h2 class="mt-2 text-3xl font-semibold text-slate-900">Seleccionar ruta</h2>
                    <p class="mt-2 text-sm text-slate-600">Elige la ruta para continuar con la creacion del vuelo.</p>
                </div>

                <div class="relative overflow-hidden overflow-x-auto rounded-xl border border-gray-200 bg-white shadow-sm">
                    <div class="p-4 flex items-center justify-between space-x-4">
                        <label for="input-group-1" class="sr-only">Buscar</label>
                        <div class="relative">
                            <div class="absolute inset-y-0 start-0 flex items-center ps-3 pointer-events-none">
                                <svg class="w-4 h-4 text-gray-500" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24"><path stroke="currentColor" stroke-linecap="round" stroke-width="2" d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"/></svg>
                            </div>
                            <input type="text" id="input-group-1" class="block w-full max-w-96 ps-9 pe-3
                            py-2 border border-gray-300 bg-white text-sm text-gray-900
                            rounded-lg shadow-sm placeholder:text-gray-500 focus:border-gold focus:ring-1 focus:ring-gold" placeholder="Buscar">
                        </div>
                    </div>

                    <table class="min-w-[1200px] w-full text-left text-base text-gray-600 rtl:text-right">
                        <thead class="border-y border-gray-200 bg-gray-50 text-base text-gray-700">
                            <tr>
                                <th scope="col" class="px-8 py-4 font-medium">Origen</th>
                                <th scope="col" class="px-8 py-4 font-medium">Destino</th>
                                <th scope="col" class="px-8 py-4 font-medium">Salida programada</th>
                                <th scope="col" class="px-8 py-4 font-medium">Llegada programada</th>
                                <th scope="col" class="px-8 py-4 font-medium">Duracion</th>
                                <th scope="col" class="px-8 py-4 font-medium">Frecuencia</th>
                                <th scope="col" class="px-8 py-4 font-medium">Accion</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="route in flightRouteStore.routes" :key="route.flightRouteId"
                            class="border-b border-gray-200 bg-white hover:bg-gray-50">
                                <th scope="row" class="whitespace-nowrap px-8 py-5 font-medium text-gray-900">
                                    <p>{{ route.departureAirport }}</p>
                                </th>
                                <td class="px-8 py-5">
                                    <p>{{ route.arrivalAirport }}</p>
                                </td>
                                <td class="px-8 py-5">
                                    <p>{{ formatDateTime(route.scheduledDepartureTime) }}</p>
                                </td>
                                <td class="px-8 py-5">
                                    <p>{{ formatDateTime(route.scheduledArrivalTime) }}</p>
                                </td>
                                <td class="px-8 py-5">
                                    <p>{{ formatDuration(route.estimatedDuration) }}</p>
                                </td>
                                <td class="px-8 py-5">
                                    <p>{{ formatFrequency(route.frequency) }}</p>
                                </td>
                                <td class="px-8 py-5">
                                    <button
                                        type="button"
                                        class="rounded-md bg-primary px-3 py-2 text-sm font-semibold text-white hover:bg-select"
                                        @click="selectRoute(route)"
                                    >
                                        Seleccionar
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <div class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6">
                        <div class="flex flex-1 justify-between sm:hidden">
                            <button
                                @click="changePage(flightRouteStore.pageNumber - 1)"
                                :disabled="flightRouteStore.pageNumber === 1"
                                class="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
                                Anterior
                            </button>
                            <button
                                @click="changePage(flightRouteStore.pageNumber + 1)"
                                :disabled="flightRouteStore.pageNumber === flightRouteStore.totalPages"
                                class="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
                                Siguiente
                            </button>
                        </div>
                        <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
                            <div>
                                <p class="text-sm text-gray-700">
                                    Mostrando <span class="font-medium">{{ (flightRouteStore.pageNumber - 1) * flightRouteStore.pageSize + 1 }}</span> a
                                    <span class="font-medium">{{ Math.min(flightRouteStore.pageNumber * flightRouteStore.pageSize, flightRouteStore.totalRecords) }}</span> de
                                    <span class="font-medium">{{ flightRouteStore.totalRecords }}</span> resultados
                                </p>
                            </div>
                            <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                                <button
                                    @click="changePage(flightRouteStore.pageNumber - 1)"
                                    :disabled="flightRouteStore.pageNumber === 1"
                                    class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50 disabled:cursor-not-allowed">
                                    <span class="sr-only">Anterior</span>
                                    <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                                        <path fill-rule="evenodd" d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z" clip-rule="evenodd" />
                                    </svg>
                                </button>
                                <button
                                    v-for="page in flightRouteStore.totalPages"
                                    :key="page"
                                    @click="changePage(page)"
                                    :aria-current="page === flightRouteStore.pageNumber ? 'page' : undefined"
                                    :class="{
                                        'z-10 bg-primary text-white focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary': page === flightRouteStore.pageNumber,
                                        'text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0': page !== flightRouteStore.pageNumber
                                    }"
                                    class="relative inline-flex items-center px-4 py-2 text-sm font-semibold">
                                    {{ page }}
                                </button>
                                <button
                                    @click="changePage(flightRouteStore.pageNumber + 1)"
                                    :disabled="flightRouteStore.pageNumber === flightRouteStore.totalPages"
                                    class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50 disabled:cursor-not-allowed">
                                    <span class="sr-only">Siguiente</span>
                                    <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                                        <path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z" clip-rule="evenodd" />
                                    </svg>
                                </button>
                            </nav>
                        </div>
                    </div>
                </div>
            </div>
        </main>
    </div>
</template>
