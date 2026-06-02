<script setup>
import { onMounted } from 'vue';
import { useAirport } from '../composable/useAirport';
import { useAirportStore } from '../store/airportStore';
import AppTable from '../../../shared/AppTable.vue';

const airportStore = useAirportStore();
const { fetchAirportsPaginated, changePage } = useAirport();

const cacheAirportForEdit = (airport) => {
    sessionStorage.setItem('airportEditData', JSON.stringify(airport));
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
                    state: { airport }
                }"
                @click="cacheAirportForEdit(airport)"
                class="font-medium text-gold hover:underline"
            >
                Edit
            </router-link>
        </td>
    </tr>
</AppTable>
</template>
