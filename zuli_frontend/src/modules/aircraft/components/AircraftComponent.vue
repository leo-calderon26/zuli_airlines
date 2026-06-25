<script setup>
import { onMounted, ref } from 'vue';
import { useAircraft } from '../composable/useAircraft';
import { useAircraftStore } from '../store/aircraftStore';
import AppTable from '../../../shared/AppTable.vue';
import ErrorModal from '../../../shared/ErrorModal.vue';
import SuccessModal from '../../../shared/SuccessModal.vue';
import AlertModal from '../../../shared/AlertModal.vue';
import { useForm } from '../../../shared/useForm.js';

const aircraftStore = useAircraftStore();
const { fetchAircraftsPaginated, changePage, deleteAircraft } = useAircraft();
const { showSuccessModal, successMessage, showErrorModal, errorMessage, isLoading, errors, clearErrors, onSuccess, handleSubmit } = useForm();
const showDeletionModal = ref(false);
const aircraftToDelete = ref(null);

const cacheAircraftForEdit = (aircraft) => {
    sessionStorage.setItem('aircraftEditData', JSON.stringify(aircraft));
};

function chooseAircraftToDelete(aircraftId) {
    aircraftToDelete.value = aircraftId;
    showDeletionModal.value = true;
};

async function processAircraftDeletion(aircraftId){
    await handleSubmit(async () => {
        await deleteAircraft(aircraftId);
        onSuccess('La aeronave se ha eliminado correctamente');
    }, 'Error al eliminar la aeronave')
    if (showErrorModal.value) {
        showErrorModal.value = false;
    }
}

onMounted(async () => {
    await fetchAircraftsPaginated(1, 10);
})
</script>

<template>
<AppTable
    :total-records="aircraftStore.totalRecords"
    :page-number="aircraftStore.pageNumber"
    :total-pages="aircraftStore.totalPages"
    :page-size="aircraftStore.pageSize"
    :empty="aircraftStore.aircrafts.length === 0"
    empty-text="No se encontraron aeronaves."
    @change-page="changePage"
>
    <template #header>
        <label for="search-aircrafts" class="sr-only">Buscar</label>
        <div class="relative">
            <div class="absolute inset-y-0 inset-x-0 flex items-center ps-3 pointer-events-none">
                <svg class="w-4 h-4 text-gray-500" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24"><path stroke="currentColor" stroke-linecap="round" stroke-width="2" d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"/></svg>
            </div>
            <input type="text" id="search-aircrafts" class="block w-full max-w-96 ps-9 pe-3 py-2 border border-gray-300 bg-white text-sm text-gray-900 rounded-lg shadow-sm placeholder:text-gray-500 focus:border-gold focus:ring-1 focus:ring-gold" placeholder="Search">
        </div>
        <router-link :to="{ name: 'createAircraft' }" class="bg-primary hover:bg-select text-white font-semibold hover:text-white py-2 px-4 border hover:border-transparent rounded hover:cursor-pointer text-center">
            Agregar
        </router-link>
    </template>

    <template #thead>
        <th scope="col" class="px-8 py-4 font-medium">Modelo</th>
        <th scope="col" class="px-8 py-4 font-medium">Peso</th>
        <th scope="col" class="px-8 py-4 font-medium">Filas Económica</th>
        <th scope="col" class="px-8 py-4 font-medium">Asientos Económica</th>
        <th scope="col" class="px-8 py-4 font-medium">Filas Primera</th>
        <th scope="col" class="px-8 py-4 font-medium">Asientos Primera</th>
        <th scope="col" class="px-8 py-4 font-medium">Detalles</th>
        <th scope="col" class="px-4 py-4 font-medium"></th>
    </template>

    <tr v-for="(aircraft, index) in aircraftStore.aircrafts" :key="aircraft.id ?? `${aircraft.model}-${index}`" class="border-b border-gray-200 bg-white hover:bg-gray-50">
        <th scope="row" class="whitespace-nowrap px-8 py-5 font-medium text-gray-900">
            <p>{{ aircraft.model }}</p>
        </th>
        <td class="px-8 py-5">
            <p>{{ aircraft.weight }}</p>
        </td>
        <td class="px-8 py-5">
            <p>{{ aircraft.numberEconomyClassRows }}</p>
        </td>
        <td class="px-8 py-5">
            <p>{{ aircraft.numberSeatingRowsEconomy }}</p>
        </td>
        <td class="px-8 py-5">
            <p>{{ aircraft.numberFirstClassRows }}</p>
        </td>
        <td class="px-8 py-5">
            <p>{{ aircraft.numberSeatingRowsFirst }}</p>
        </td>
        <td class="px-8 py-5">
            <router-link
                :to="{
                    name: 'editAircraft',
                    params: { aircraftId: aircraft.aircraftId },
                    state: { aircraft: JSON.parse(JSON.stringify(aircraft)) }
                }"
                @click="cacheAircraftForEdit(aircraft)"
                class="font-medium text-gold hover:underline"
            >
                Editar
            </router-link>
        </td>
        <td class="px-4 py-5">
            <button
                type="button"
                class="rounded-md bg-primary px-2 py-2 text-sm font-semibold text-white hover:bg-select"
                @click="chooseAircraftToDelete(aircraft.aircraftId)"
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
    title="Eliminar Aeronave" 
    :message="'¿Está seguro de querer eliminar la aeronave seleccionada?\nEsta acción no se puede deshacer.'"
    buttonText="Eliminar"
    @confirm="processAircraftDeletion(aircraftToDelete)"
/>
</template>
