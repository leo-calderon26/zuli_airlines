<script setup>
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import { useFlightRoute } from "../composable/useFlightRoute";
import { useFlightRouteStore } from "../store/flightRouteStore";
import AppTable from '../../../shared/AppTable.vue';
import ErrorModal from '../../../shared/ErrorModal.vue';
import SuccessModal from '../../../shared/SuccessModal.vue';
import AlertModal from '../../../shared/AlertModal.vue';
import { useForm } from '../../../shared/useForm.js';

const router = useRouter();
const flightRouteStore = useFlightRouteStore();
const { fetchRoutesPaginated, changePage, deleteRoute } = useFlightRoute();
const showMaintenanceModal = ref(false);
const {
    showSuccessModal,
    successMessage,
    showErrorModal,
    errorMessage,
    onSuccess,
    handleSubmit
} = useForm();

const showDeletionModal = ref(false);
const routeToDelete = ref(null);
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
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return String(value);
  if (/^\d{2}:\d{2}(:\d{2})?$/.test(value)) {
    return value.slice(0, 5);
  }
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
  showMaintenanceModal.value = true;
};
function chooseRouteToDelete(flightRouteId) {
    routeToDelete.value = flightRouteId;
    showDeletionModal.value = true;
}

async function processRouteDeletion(flightRouteId) {
    await handleSubmit(async () => {
        await deleteRoute(flightRouteId);

        onSuccess('La ruta se ha eliminado correctamente');
    }, 'Error al eliminar la ruta');

    if (showErrorModal.value) {
        showErrorModal.value = false;
    }
}
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
            <div class="flex items-center gap-2 whitespace-nowrap">
                <button
                    type="button"
                    class="rounded-md bg-primary px-3 py-2 text-sm font-semibold text-white hover:bg-select"
                    @click="selectRoute(route)"
                >
                    Seleccionar
                </button>

                <button
                    type="button"
                    class="flex h-11 w-11 shrink-0 items-center justify-center rounded-md bg-primary text-white hover:bg-select"
                    title="Eliminar ruta"
                    :aria-label="`Eliminar ruta ${route.departureAirport} - ${route.arrivalAirport}`"
                    @click="chooseRouteToDelete(route.flightRouteId)"
                >
                    <img
                        src="../../../assets/TrashCan.png"
                        alt="Trash Can Icon"
                        class="h-6 w-6 shrink-0"
                    />
                </button>
            </div>
        </td>
    </tr>
</AppTable>

<ErrorModal 
    v-model="showMaintenanceModal" 
    title="En Mantenimiento" 
    message="La funcionalidad para gestionar vuelos a partir de esta ruta se encuentra actualmente en mantenimiento. Por favor, intente más tarde." 
    buttonText="Entendido"
/>
<SuccessModal
    v-model="showSuccessModal"
    :message="successMessage"
/>

<ErrorModal
    v-model="showErrorModal"
    :message="errorMessage"
/>

<AlertModal
    v-model="showDeletionModal"
    title="Eliminar Ruta"
    :message="'¿Está seguro de querer eliminar la ruta seleccionada?\nSi la ruta tiene compras o reservas asociadas, se deshabilitará para conservar el historial; de lo contrario, se eliminará permanentemente.'"
    buttonText="Eliminar"
    @confirm="processRouteDeletion(routeToDelete)"
/>
</template>
