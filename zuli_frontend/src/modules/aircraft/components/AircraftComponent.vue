<script setup>
import { onMounted } from 'vue';
import { useAircraft } from '../composable/useAircraft';
import { useAircraftStore } from '../store/aircraftStrore';

const aircraftStore = useAircraftStore();
const { fetchAircrafts } = useAircraft();

onMounted(async () => {
    await fetchAircrafts();
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
        <button class="bg-primary hover:bg-select text-white font-semibold
         hover:text-white py-2 px-4 border hover:border-transparent rounded hover:cursor-pointer">
            Agregar
        </button>
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
</div>
</template>