<script setup>
import { onMounted } from "vue";
import { useRouter } from "vue-router";
import { useFlightRoute } from "../composable/useFlightRoute";
import { useFlightRouteStore } from "../store/flightRouteStore";
import AppTable from '../../../shared/AppTable.vue';

const router = useRouter();
const flightRouteStore = useFlightRouteStore();
const { fetchRoutesPaginated, changePage } = useFlightRoute();

const dayLabels = [
  { bit: 1, label: "Lun" },
  { bit: 2, label: "Mar" },
  { bit: 4, label: "Mie" },
  { bit: 8, label: "Jue" },
  { bit: 16, label: "Vie" },
  { bit: 32, label: "Sab" },
  { bit: 64, label: "Dom" },
];

const formatFrequency = (value) => {
  const mask = Number(value);
  if (!Number.isFinite(mask) || mask <= 0) return "Sin frecuencia";
  const selected = dayLabels
    .filter((day) => (mask & day.bit) !== 0)
    .map((day) => day.label);
  return selected.length ? selected.join(", ") : "Sin frecuencia";
};

const formatDateTime = (value) => {
  if (!value) return "-";
  if (/^\d{2}:\d{2}(:\d{2})?$/.test(value)) {
    return value.slice(0, 5);
  }
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return String(value);
  return date.toLocaleString("es-ES", {
    dateStyle: "short",
    timeStyle: "short",
  });
};

const formatDuration = (value) => {
  const totalSeconds = Number(value);
  if (!Number.isFinite(totalSeconds)) return "-";
  const totalMinutes = Math.round(totalSeconds / 60);
  if (!Number.isFinite(totalMinutes)) return "-";
  if (totalMinutes < 60) return `${totalMinutes} min`;
  const hours = Math.floor(totalMinutes / 60);
  const minutes = totalMinutes % 60;
  return `${hours} h ${minutes} min`;
};

const selectRoute = (route) => {
  if (!route?.flightRouteId) return;
  router.push({
    name: "flights",
    query: {
      routeId: route.flightRouteId,
      departure: route.departureAirport,
      arrival: route.arrivalAirport,
      duration: route.estimatedDuration,
      frequency: route.frequency,
    },
  });
};

onMounted(async () => {
  await fetchRoutesPaginated(1, 10);
});
</script>

<template>
<AppTable
    :total-records="flightRouteStore.totalRecords"
    :page-number="flightRouteStore.pageNumber"
    :total-pages="flightRouteStore.totalPages"
    :page-size="flightRouteStore.pageSize"
    :empty="flightRouteStore.routes.length === 0"
    empty-text="No se encontraron rutas."
    @change-page="changePage"
>
    <template #header>
        <label for="route-search" class="sr-only">Buscar</label>
        <div class="relative">
            <div class="absolute inset-y-0 start-0 flex items-center ps-3 pointer-events-none">
                <svg class="w-4 h-4 text-gray-500" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24"><path stroke="currentColor" stroke-linecap="round" stroke-width="2" d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"/></svg>
            </div>
            <input type="text" id="route-search" class="block w-full max-w-96 ps-9 pe-3 py-2 border border-gray-300 bg-white text-sm text-gray-900 rounded-lg shadow-sm placeholder:text-gray-500 focus:border-gold focus:ring-1 focus:ring-gold" placeholder="Search">
        </div>
        <router-link :to="{ name: 'createRoute' }" class="bg-primary hover:bg-select text-white font-semibold hover:text-white py-2 px-4 border hover:border-transparent rounded hover:cursor-pointer text-center">
            Agregar
        </router-link>
    </template>

    <template #thead>
        <th scope="col" class="px-8 py-4 font-medium">Origen</th>
        <th scope="col" class="px-8 py-4 font-medium">Destino</th>
        <th scope="col" class="px-8 py-4 font-medium">Salida programada</th>
        <th scope="col" class="px-8 py-4 font-medium">Llegada programada</th>
        <th scope="col" class="px-8 py-4 font-medium">Duracion estimada</th>
        <th scope="col" class="px-8 py-4 font-medium">Frecuencia</th>
        <th scope="col" class="px-8 py-4 font-medium">Accion</th>
    </template>

    <tr
        v-for="(route, index) in flightRouteStore.routes"
        :key="route.flightRouteId ?? `${route.departureAirport}-${route.arrivalAirport}-${index}`"
        class="border-b border-gray-200 bg-white hover:bg-gray-50"
    >
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
</AppTable>
</template>
