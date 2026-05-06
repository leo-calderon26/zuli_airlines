<script setup>
import { onMounted } from 'vue';
import { useAirport } from '../composable/useAirport';
import { useAirportStore } from '../store/airportStore';

const airportStore = useAirportStore();
const { fetchAirportsPaginated, changePage } = useAirport();

onMounted(async () => {
    // Ajusta los parámetros según tu backend, asumiendo página 1, tamaño 10
    await fetchAirportsPaginated(1, 10);
})
</script>

<template>
<div class="relative overflow-hidden overflow-x-auto rounded-xl border border-gray-200 bg-white shadow-sm">
    <div class="p-4 flex items-center justify-between space-x-4">
        <label for="search-airports" class="sr-only">Buscar</label>
        <div class="relative">
            <div class="absolute inset-y-0 start-0 flex items-center ps-3 pointer-events-none">
                <svg class="w-4 h-4 text-gray-500" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24">
                    <path stroke="currentColor" stroke-linecap="round" stroke-width="2" d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"/>
                </svg>
            </div>
            <input type="text" id="search-airports" class="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2 ps-10 text-sm text-gray-900 focus:border-primary focus:ring-primary" placeholder="Buscar aeropuertos">
        </div>
        <div>
            <router-link to="/admin/create-airport" class="inline-flex items-center rounded-lg bg-red-900 px-4 py-2 text-sm font-medium text-white hover:bg-red-800">
                Agregar Aeropuerto
            </router-link>
        </div>
    </div>

    <table class="w-full text-left text-sm text-gray-500">
        <thead class="bg-gray-50 text-xs uppercase text-gray-700">
            <tr>
                <th scope="col" class="px-6 py-3">Código</th>
                <th scope="col" class="px-6 py-3">Nombre</th>
                <th scope="col" class="px-6 py-3">Ciudad</th>
                <th scope="col" class="px-6 py-3">País</th>
            </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
            <tr v-for="airport in airportStore.airports" :key="airport.airportCode" class="bg-white hover:bg-gray-50">
                <td class="px-6 py-4 font-medium text-gray-900">{{ airport.airportCode }}</td>
                <td class="px-6 py-4">{{ airport.name }}</td>
                <td class="px-6 py-4">{{ airport.city }}</td>
                <td class="px-6 py-4">{{ airport.country }}</td>
            </tr>
            <tr v-if="airportStore.airports.length === 0">
                <td colspan="4" class="px-6 py-10 text-center text-gray-400">No se encontraron aeropuertos.</td>
            </tr>
        </tbody>
    </table>

    <div class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6">
        <div class="flex flex-1 justify-between sm:hidden">
            <button @click="changePage(airportStore.pageNumber - 1)" :disabled="airportStore.pageNumber === 1" class="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50">Anterior</button>
            <button @click="changePage(airportStore.pageNumber + 1)" :disabled="airportStore.pageNumber === airportStore.totalPages" class="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50">Siguiente</button>
        </div>
        <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
            <div>
                <p class="text-sm text-gray-700">
                    Mostrando página <span class="font-medium">{{ airportStore.pageNumber }}</span> de <span class="font-medium">{{ airportStore.totalPages }}</span>
                </p>
            </div>
            <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                <button v-for="page in airportStore.totalPages" :key="page" @click="changePage(page)"
                    :class="[page === airportStore.pageNumber ? 'z-10 bg-red-900 text-white' : 'text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50']"
                    class="relative inline-flex items-center px-4 py-2 text-sm font-semibold focus:z-20">
                    {{ page }}
                </button>
            </nav>
        </div>
    </div>
</div>
</template>