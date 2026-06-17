<script setup>
import { ref, computed } from 'vue';
import AppInput from '../../../shared/AppInput.vue';
import AppButton from '../../../shared/AppButton.vue';
import AppTable from '../../../shared/AppTable.vue';

const filters = ref({
    fechaDesde: '',
    fechaHasta: '',
    origen: '',
    destino: '',
    clase: ''
});

const isLoading = ref(false);
const hasSearched = ref(false);


const reportData = ref([]);


async function handleSearch() {
    isLoading.value = true;
    hasSearched.value = true;

    try {

        // reportData.value = await getAirlineReport(filters.value);
    } catch (error) {
        console.error("Error al obtener el reporte:", error);
    } finally {
        isLoading.value = false;
    }
}
</script>

<template>
    <div class="space-y-6">
        <div class="form-card">
            <h2 class="text-xl font-semibold text-gray-800 mb-6 pb-2 border-b border-gray-100">
                Filtros del Reporte
            </h2>
            
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4 items-end">
                <AppInput v-model="filters.fechaDesde" label="Fecha Desde" type="date" />
                <AppInput v-model="filters.fechaHasta" label="Fecha Hasta" type="date" />
                <AppInput v-model="filters.origen" label="Origen" placeholder="Ej: SJO" />
                <AppInput v-model="filters.destino" label="Destino" placeholder="Ej: MAD" />
                
                <div class="flex flex-col">
                    <label class="text-sm font-medium text-gray-700 mb-1.5">Clase</label>
                    <select v-model="filters.clase" class="w-full h-[42px] rounded-md border border-gray-300 bg-white px-3 py-2 text-sm focus:border-primary focus:outline-none">
                        <option value="">Todas</option>
                        <option value="Turista">Turista</option>
                        <option value="Primera">Primera Clase</option>
                    </select>
                </div>
            </div>

            <div class="mt-6 flex flex-wrap gap-3 justify-start">
                <AppButton type="button" variant="primary" :loading="isLoading" @click="handleSearch">
                    Generar Reporte
                </AppButton>
            </div>
        </div>

        <AppTable
            v-if="hasSearched"
            :empty="reportData.length === 0"
            empty-text="No se encontraron registros de vuelos para este reporte."
        >
            <template #header>
                <h3 class="font-semibold text-gray-700 text-sm">Vista Previa de la Información</h3>
            </template>

            <template #thead>
                <th scope="col" class="px-6 py-4 font-medium">Fecha</th>
                <th scope="col" class="px-6 py-4 font-medium">Origen</th>
                <th scope="col" class="px-6 py-4 font-medium">Destino</th>
                <th scope="col" class="px-6 py-4 font-medium"># Vuelo</th>
                <th scope="col" class="px-6 py-4 font-medium text-center">Pasajeros Primera clase</th>
                <th scope="col" class="px-6 py-4 font-medium text-center">Pasajeros clase Económica</th>
                <th scope="col" class="px-6 py-4 font-medium">Aerolínea</th>
                <th scope="col" class="px-6 py-4 font-medium text-right">Venta Pasajeros</th>
                <th scope="col" class="px-6 py-4 font-medium text-right">Venta Equipaje</th>
                <th scope="col" class="px-6 py-4 font-medium text-right">Total Venta</th>
            </template>

            <tr v-for="(row, index) in reportData" :key="index" class="border-b border-gray-200 bg-white hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap text-gray-500 text-xs">{{ row.fecha }}</td>
                <th scope="row" class="whitespace-nowrap px-6 py-4 font-medium text-gray-900">{{ row.origen }}</th>
                <td class="px-6 py-4 font-medium text-gray-900">{{ row.destino }}</td>
                <td class="px-6 py-4 font-mono text-xs text-gray-400">{{ row.numeroVuelo }}</td>
                <td class="px-6 py-4 text-center">{{ row.pasajerosPrimera }}</td>
                <td class="px-6 py-4 text-center">{{ row.pasajerosEconomica }}</td>
                <td class="px-6 py-4 text-gray-600">{{ row.aerolinea }}</td>
                <td class="px-6 py-4 text-right font-medium">${{ row.ventaPasajeros?.toFixed(2) }}</td>
                <td class="px-6 py-4 text-right font-medium">${{ row.ventaEquipajes?.toFixed(2) }}</td>
                <td class="px-6 py-4 text-right font-semibold text-gray-900 bg-gray-50/50">${{ row.totalVenta?.toFixed(2) }}</td>
            </tr>

            <tr v-if="reportData.length > 0" class="bg-gray-100/80 font-bold text-gray-900 border-t-2 border-gray-300">
                <td colspan="4" class="px-6 py-3 text-right uppercase tracking-wider text-xs text-gray-400">Totales del Reporte:</td>
                <td class="px-6 py-3 text-center bg-gray-50">{{ totalPasajerosPrimera }}</td>
                <td class="px-6 py-3 text-center bg-gray-50">{{ totalPasajerosEconomica }}</td>
                <td></td>
                <td class="px-6 py-3 text-right text-emerald-700 bg-gray-50">${{ totalVentaPasajeros?.toFixed(2) }}</td>
                <td class="px-6 py-3 text-right text-emerald-700 bg-gray-50">${{ totalVentaEquipajes?.toFixed(2) }}</td>
                <td class="px-6 py-3 text-right text-blue-950 bg-blue-50">${{ totalGeneralVenta?.toFixed(2) }}</td>
            </tr>
        </AppTable>

    </div>
</template>

<style scoped>
@reference "../../../style.css";

.form-card {
    @apply w-full max-w-none rounded-lg border border-gray-200 bg-white p-6 shadow-sm;
}
</style>