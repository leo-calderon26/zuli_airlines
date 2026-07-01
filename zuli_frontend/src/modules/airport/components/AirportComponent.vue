<script setup>
import { onMounted, ref } from 'vue';
import { useAirport } from '../composable/useAirport';
import { useAirportStore } from '../store/airportStore';
import AppTable from '../../../shared/AppTable.vue';
import SuccessModal from '../../../shared/SuccessModal.vue'; 
import ErrorModal from '../../../shared/ErrorModal.vue';
import AlertModal from '../../../shared/AlertModal.vue';
import { useForm } from '../../../shared/useForm.js';

const airportStore = useAirportStore();
const { fetchAirportsPaginated, changePage,deleteAirport: apiDeleteAirport } = useAirport();
const { showSuccessModal, successMessage, showErrorModal, errorMessage, handleSubmit, onSuccess } = useForm();
const showDeletionModal = ref(false);
const airportToDelete = ref(null);

const cacheAirportForEdit = (airport) => {
    sessionStorage.setItem('airportEditData', JSON.stringify(airport));
};

const chooseAirportToDelete = (airportCode) => {
    airportToDelete.value = airportCode;
    showDeletionModal.value = true;
};

const processAirportDeletion = async (code) => {
    showDeletionModal.value = false;


    await handleSubmit(async () => {
        await apiDeleteAirport(code);
        
        onSuccess('El proceso de eliminación o deshabilitación del aeropuerto se ejecutó correctamente.');
        
        await fetchAirportsPaginated(airportStore.pageNumber, airportStore.pageSize);
    }, 'Ocurrió un error al intentar eliminar el aeropuerto.');
};

onMounted(async () => {
    await fetchAirportsPaginated(1, 10);
})

</script>

<template>
<AppTable
    :total-records="airportStore.totalRecords"
    :page-number="airportStore.pageNumber"
    :total-pages="airportStore.totalPages"
    :page-size="airportStore.pageSize"
    :empty="airportStore.airports.length === 0"
    empty-text="No se encontraron aeropuertos."
    @change-page="changePage"
>
    <template #header>
        <label for="search-airports" class="sr-only">Buscar</label>
        <div class="relative">
            <div class="absolute inset-y-0 inset-x-0 flex items-center ps-3 pointer-events-none">
                <svg class="w-4 h-4 text-gray-500" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24"><path stroke="currentColor" stroke-linecap="round" stroke-width="2" d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"/></svg>
            </div>
            <input type="text" id="search-airports" class="block w-full max-w-96 ps-9 pe-3 py-2 border border-gray-300 bg-white text-sm text-gray-900 rounded-lg shadow-sm placeholder:text-gray-500 focus:border-gold focus:ring-1 focus:ring-gold" placeholder="Search">
        </div>
        <router-link :to="{ name: 'createAirport' }" class="bg-primary hover:bg-select text-white font-semibold hover:text-white py-2 px-4 border hover:border-transparent rounded hover:cursor-pointer text-center">
            Agregar
        </router-link>
    </template>

    <template #thead>
        <th scope="col" class="px-8 py-4 font-medium">Código</th>
        <th scope="col" class="px-8 py-4 font-medium">Nombre</th>
        <th scope="col" class="px-8 py-4 font-medium">Ciudad</th>
        <th scope="col" class="px-8 py-4 font-medium">País</th>
        <th scope="col" class="px-8 py-4 font-medium">Detalles</th>
    </template>

    <tr v-for="airport in airportStore.airports" :key="airport.airportCode" class="border-b border-gray-200 bg-white hover:bg-gray-50">
        <th scope="row" class="whitespace-nowrap px-8 py-5 font-medium text-gray-900">
            <p>{{ airport.airportCode }}</p>
        </th>
        <td class="px-8 py-5">
            <p>{{ airport.name }}</p>
        </td>
        <td class="px-8 py-5">
            <p>{{ airport.city }}</p>
        </td>
        <td class="px-8 py-5">
            <p>{{ airport.country }}</p>
        </td>
        <td class="px-8 py-5">
            <router-link
                :to="{
                    name: 'editAirport',
                    params: { airportCode: airport.airportCode },
                    state: { airport: JSON.parse(JSON.stringify(airport)) }
                }"
                @click="cacheAirportForEdit(airport)"
                class="font-medium text-gold hover:underline"
            >
                Editar
            </router-link>
        </td>

        <td class="px-4 py-5">
            <button
                type="button"
                class="rounded-md bg-primary px-2 py-2 text-sm font-semibold text-white hover:bg-select"
                @click="chooseAirportToDelete(airport.airportCode)"
            >
                <img src="../../../assets/TrashCan.png" alt="Trash Can Icon" class="h-6 w-6 shrink-0" />
            </button>
        </td>
    </tr>
</AppTable>
<SuccessModal v-model="showSuccessModal" :message="successMessage" />
<ErrorModal v-model="showErrorModal" :message="errorMessage" />

<AlertModal
    v-model="showDeletionModal"
    title="Eliminar Aeropuerto"
    :message="'¿Está seguro de querer eliminar el aeropuerto seleccionado?\nEsta acción deshabilitará sus rutas comerciales activas o lo eliminará permanentemente del sistema.'"
    buttonText="Eliminar"
    @confirm="processAirportDeletion(airportToDelete)"
/>
</template>
