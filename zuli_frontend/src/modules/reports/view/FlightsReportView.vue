<script setup>
import { ref, onMounted } from 'vue';
import AppButton from '../../../shared/AppButton.vue';
import AppTable from '../../../shared/AppTable.vue';
import FilterCard from '../../../shared/components/FilterCard.vue'; 
import TransactionsTable from '../../../shared/components/TransactionsTable.vue';
import { getFlightsReport, getFilterOptions, exportFlightsReport } from '../service/flightsReportService';

const filterConfig = ref([
  { key: 'fromDate', label: 'Fecha Desde', type: 'date' },
  { key: 'toDate', label: 'Fecha Hasta', type: 'date' },
  { key: 'origin', label: 'Origen', type: 'select', options: [] },
  { key: 'destination', label: 'Destino', type: 'select', options: [] },
  {
    key: 'flightClass',
    label: 'Clase',
    type: 'select',
    options: [
      { label: 'Todas', value: '' },
      { label: 'Turista', value: 'Turista' },
      { label: 'Primera Clase', value: 'Primera Clase' }
    ]
  }
]);

const reportColumns = [
  { key: 'date', label: 'Fecha' },
  { key: 'origin', label: 'Origen' },
  { key: 'destination', label: 'Destino' },
  { key: 'flightNumber', label: '# Vuelo' },
  { key: 'firstClassPassengers', label: 'Pasajeros Primera Clase', format: 'number', align: 'center' },
  { key: 'economyClassPassengers', label: 'Pasajeros Clase Turista', format: 'number', align: 'center' },
  { key: 'airline', label: 'Aerolínea' },
  { key: 'passengerSales', label: 'Venta Pasajeros', format: 'money', align: 'right' },
  { key: 'baggageSales', label: 'Venta Equipaje', format: 'money', align: 'right' },
  { key: 'totalSales', label: 'Venta Total', format: 'money', align: 'right', font: 'bold' }
];

const selectedFilters = ref({
    fromDate: '',
    toDate: '',
    origin: '',
    destination: '',
    flightClass: ''
});

const lastAppliedFilters = ref({});
const isLoading = ref(false);
const hasSearched = ref(false);
const isDownloading = ref(false);
const reportData = ref([]);

const mapAirportToOption = (airport) => ({
    label: airport.airportCode, 
    value: airport.airportCode
});

onMounted(async () => {
    try {
        const dataOptions = await getFilterOptions();

        filterConfig.value = filterConfig.value.map(filter => {
            
            if (filter.key === 'origin') {
                return {
                    ...filter,
                    options: [
                        { label: 'Todos', value: '' },
                        ...dataOptions.origins.map(mapAirportToOption)
                    ]
                };
            }

            if (filter.key === 'destination') {
                return {
                    ...filter,
                    options: [
                        { label: 'Todos', value: '' },
                        ...dataOptions.destinations.map(mapAirportToOption)
                    ]
                };
            }
            return filter;
        });

    } catch (error) {
        console.error("Error al cargar las opciones de filtros:", error);
    }
});

async function handleSearch(finalFilters) {
    isLoading.value = true; 
    hasSearched.value = true;
    lastAppliedFilters.value = { ...finalFilters };
    
    try {
        
      const data = await getFlightsReport(finalFilters);
      reportData.value = data;
    } catch (error) {
        console.error("Error al cargar los datos en la vista:", error);
        reportData.value = [];
    } finally {
        isLoading.value = false; 
    }
}

async function handleDownload() {
    isLoading.value = true; 

    try {
      const { blob, filename } = await exportFlightsReport(lastAppliedFilters.value);

        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a'); 
        a.href = url;
        
        a.download = filename || 'reporte_vuelos.xlsx';
        
        document.body.appendChild(a);
        a.click();
        
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    } catch (e) {
        console.error("Error en la descarga del reporte:", e);
    } finally {
        isLoading.value = false;
    }
}
</script>

<template>
<div class="space-y-6">
    <nav class="text-sm text-content-subtle">
      <ol class="flex items-center gap-2">
        <li class="hover:text-primary transition cursor-pointer">Gestión</li>
        <li>/</li>
        <li class="text-primary font-medium">Vuelos</li>
      </ol>
    </nav>
    <div>
      <h1 class="text-2xl font-bold text-heading">
        Panel Administrativo - Gestión de vuelos
      </h1>
      <p class="mt-1 text-content-subtle">
        Consulta y filtra la información de los vuelos
      </p>
    </div>
    <FilterCard 
      title="Filtros del Reporte de Vuelos"
      :filters="filterConfig"
      v-model="selectedFilters"
      @apply="handleSearch"
    />

    <div class="space-y-4">
      
      <TransactionsTable
        title="Vista Previa de la Información"
        :columns="reportColumns"
        :rows="reportData"
        :loading="isLoading"
        emptyMessage="No se encontraron registros de vuelos para este reporte."
        :showTotals="true"
      />

      <div v-if="reportData.length > 0" class="flex justify-end">
        <button
          type="button"
          class="inline-flex items-center gap-2 rounded-full bg-primary px-8 py-3 text-sm font-semibold text-white shadow-lg transition hover:opacity-90"
          @click="handleDownload" 
        >
          <svg class="h-4 w-4 text-white" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
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