<script setup>
import { onMounted } from 'vue';
import { useAircraft } from '../composable/useAircraft';
import { useAircraftStore } from '../store/aircraftStore';
import AppTable from '../../../shared/AppTable.vue';

const aircraftStore = useAircraftStore();
const { fetchAircraftsPaginated, changePage } = useAircraft();

const cacheAircraftForEdit = (aircraft) => {
    sessionStorage.setItem('aircraftEditData', JSON.stringify(aircraft));
};

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
                Edit
            </router-link>
        </td>
    </tr>
</AppTable>
</template>
