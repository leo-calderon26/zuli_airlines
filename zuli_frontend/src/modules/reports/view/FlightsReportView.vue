<script setup>
import { ref, computed } from 'vue';
import AppButton from '../../../shared/AppButton.vue';
import AppTable from '../../../shared/AppTable.vue';
import FilterCard from '../../../shared/components/FilterCard.vue'; 
import TransactionsTable from '../../../shared/components/TransactionsTable.vue';
import { getFlightsReport } from '../service/flightsReportService';

const configuracionFiltros = [
  { key: 'fechaDesde', label: 'Fecha Desde', type: 'date'},
    { key: 'fechaHasta', label: 'Fecha Hasta', type: 'date' },
    { key: 'origen', label: 'Origen', type: 'text', placeholder: 'Ej: SJO' },
    { key: 'destino', label: 'Destino', type: 'text', placeholder: 'Ej: MAD' },
    {
        key: 'clase',
        label: 'Clase',
        type: 'select',
        options: [
            { label: 'Todas', value: '' },
            { label: 'Turista', value: 'Turista' },
            { label: 'Primera Clase', value: 'Primera' }
        ]
    }
];

const columnasReporte = [
  { key: 'fecha', label: 'Fecha' },
  { key: 'origen', label: 'Origen' },
  { key: 'destino', label: 'Destino' },
  { key: 'numeroVuelo', label: '# Vuelo' },
  { key: 'pasajerosPrimera', label: 'Pasajeros Primera', format: 'number', align: 'center' },
  { key: 'pasajerosEconomica', label: 'Pasajeros Económica', format: 'number', align: 'center' },
  { key: 'aerolinea', label: 'Aerolínea' },
  { key: 'ventaPasajeros', label: 'Venta Pasajeros', format: 'money', align: 'right' },
  { key: 'ventaEquipaje', label: 'Venta Equipaje', format: 'money', align: 'right' },
  { key: 'totalVenta', label: 'Total Venta', format: 'money', align: 'right', font: 'bold' }
];

const filtrosSeleccionados = ref({
    fechaDesde: '',
    fechaHasta: '',
    origen: '',
    destino: '',
    clase: ''
});

const isLoading = ref(false);
const hasSearched = ref(false);
const reportData = ref([]);


async function handleSearch(filtrosFinales) {
    isLoading.value = true; 
    hasSearched.value = true;
    
    try {
        
        const data = await getFlightsReport();
        
        
        reportData.value = data;
    } catch (error) {
        console.error("Error al cargar los datos en la vista:", error);
        
        reportData.value = [];
    } finally {
        isLoading.value = false; 
    }
}
</script>

<template>
<div class="space-y-6">
    <FilterCard 
      title="Filtros del Reporte de Vuelos"
      :filters="configuracionFiltros"
      v-model="filtrosSeleccionados"
      @apply="handleSearch"
    />

    <div v-if="hasSearched" class="space-y-4">
      
      <TransactionsTable
        title="Vista Previa de la Información"
        :columns="columnasReporte"
        :rows="reportData"
        :loading="isLoading"
        emptyMessage="No se encontraron registros de vuelos para este reporte."
        :showTotals="true"
      />

      <div v-if="reportData.length > 0" class="flex justify-end">
        <button
          type="button"
          class="inline-flex items-center gap-2 rounded-base border border-gray-300 bg-white px-6 py-2.5 text-sm font-semibold text-gray-700 shadow-sm transition hover:bg-gray-50"
          @click="handleDownload" 
        >
          <svg class="h-4 w-4 text-gray-500" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3 16.5v2.25A2.25 2.25 0 0 0 5.25 21h13.5A2.25 2.25 0 0 0 21 18.75V16.5M16.5 12 12 16.5m0 0L7.5 12m4.5 4.5V3"/>
          </svg>
          Descargar reporte
        </button>
      </div>

    </div>
  </div>
</template>

<style scoped>
@reference "../../../style.css";
</style>