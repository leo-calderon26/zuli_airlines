<script setup>
import { onMounted } from 'vue';
import { useAircraft } from '../composable/useAircraft';
import { useAircraftStore } from '../store/aircraftStore';

const aircraftStore = useAircraftStore();
const { fetchAircraftsPaginated, changePage } = useAircraft();

onMounted(async () => {
    await fetchAircraftsPaginated(1, 10);
})

</script>
<template>

<div class="relative overflow-hidden overflow-x-auto rounded-xl border border-gray-200 bg-white shadow-sm">
    <div class="p-4 flex items-center justify-between space-x-4">
        <label for="input-group-1" class="sr-only">Buscar</label>
        <div class="relative">
            <div class="absolute inset-y-0 start-0 flex items-center ps-3 pointer-events-none">
                <svg class="w-4 h-4 text-gray-500" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" 
                width="24" height="24" fill="none" viewBox="0 0 24 24"><path stroke="currentColor" stroke-linecap="round" 
                stroke-width="2" d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"/></svg>
            </div>
            <input type="text" id="input-group-1" class="block w-full max-w-96 ps-9 pe-3
            py-2 border border-gray-300 bg-white text-sm text-gray-900
            rounded-lg shadow-sm placeholder:text-gray-500 focus:border-gold focus:ring-1 focus:ring-gold" placeholder="Search">
        </div>
        <router-link :to="{ name: 'createAircraft' }" class="bg-primary hover:bg-select text-white font-semibold
         hover:text-white py-2 px-4 border hover:border-transparent rounded hover:cursor-pointer text-center">
            Agregar
        </router-link>
    </div>
    <table class="min-w-[1200px] w-full text-left text-base text-gray-600 rtl:text-right">
        <thead class="border-y border-gray-200 bg-gray-50 text-base text-gray-700">
            <tr>
                <th scope="col" class="px-8 py-4 font-medium">
                    Modelo
                </th>
                <th scope="col" class="px-8 py-4 font-medium">
                    Peso
                </th>
                <th scope="col" class="px-8 py-4 font-medium">
                    Número de filas en clase Económica
                </th>
                <th scope="col" class="px-8 py-4 font-medium">
                    Número de asientos en clase económica
                </th>
                <th scope="col" class="px-8 py-4 font-medium">
                    Número de filas en primera clase
                </th>
                <th scope="col" class="px-8 py-4 font-medium">
                    Número de asientos en primera Clase
                </th>
                <th scope="col" class="px-8 py-4 font-medium">
                    Detalles
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(aircraft, index) in aircraftStore.aircrafts" :key="aircraft.id ?? `${aircraft.model}-${index}`"
            class="border-b border-gray-200 bg-white hover:bg-gray-50">
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
                    <a href="#" class="font-medium text-gold hover:underline">Edit</a>
                </td>
            </tr>
        </tbody>
    </table>
    
    <!-- Paginación -->
    <div class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6">
        <div class="flex flex-1 justify-between sm:hidden">
            <button 
                @click="changePage(aircraftStore.pageNumber - 1)"
                :disabled="aircraftStore.pageNumber === 1"
                class="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
                Anterior
            </button>
            <button 
                @click="changePage(aircraftStore.pageNumber + 1)"
                :disabled="aircraftStore.pageNumber === aircraftStore.totalPages"
                class="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
                Siguiente
            </button>
        </div>
        <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
            <div>
                <p class="text-sm text-gray-700">
                    Mostrando <span class="font-medium">{{ (aircraftStore.pageNumber - 1) * aircraftStore.pageSize + 1 }}</span> a 
                    <span class="font-medium">{{ Math.min(aircraftStore.pageNumber * aircraftStore.pageSize, aircraftStore.totalRecords) }}</span> de 
                    <span class="font-medium">{{ aircraftStore.totalRecords }}</span> resultados
                </p>
            </div>
            <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                <button 
                    @click="changePage(aircraftStore.pageNumber - 1)"
                    :disabled="aircraftStore.pageNumber === 1"
                    class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50 disabled:cursor-not-allowed">
                    <span class="sr-only">Anterior</span>
                    <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                        <path fill-rule="evenodd" d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z" clip-rule="evenodd" />
                    </svg>
                </button>
                <button 
                    v-for="page in aircraftStore.totalPages"
                    :key="page"
                    @click="changePage(page)"
                    :aria-current="page === aircraftStore.pageNumber ? 'page' : undefined"
                    :class="{
                        'z-10 bg-primary text-white focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary': page === aircraftStore.pageNumber,
                        'text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0': page !== aircraftStore.pageNumber
                    }"
                    class="relative inline-flex items-center px-4 py-2 text-sm font-semibold">
                    {{ page }}
                </button>
                <button 
                    @click="changePage(aircraftStore.pageNumber + 1)"
                    :disabled="aircraftStore.pageNumber === aircraftStore.totalPages"
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
</template>