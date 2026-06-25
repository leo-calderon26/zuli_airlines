<template>
  <div class="space-y-6">
    <nav class="text-sm text-content-subtle">
      <ol class="flex items-center gap-2">
        <li class="hover:text-primary transition cursor-pointer">Gestión</li>
        <li>/</li>
        <li class="text-primary font-medium">Ingresos</li>
      </ol>
    </nav>
    <div>
      <h1 class="text-2xl font-bold text-heading">
        Panel Administrativo - Gestión de ingresos
      </h1>
      <p class="mt-1 text-content-subtle">
        Consulta y filtra los ingresos generados por venta de boletos
      </p>
    </div>
    <FilterCard
      v-model="filters"
      title="Filtros"
      :filters="filterConfig"
      @apply="applyFilters"
    />

    <ErrorModal v-model="showErrorModal" :message="errorMessage" @close="resetModals" />

    <section class="grid grid-cols-1 sm:grid-cols-2 gap-6">
      <div class="rounded-xl bg-white border border-[#dcc0be]/20 p-6 shadow-sm">
        <div class="flex items-center gap-4">
          <div class="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10">
            <svg class="h-6 w-6 text-primary" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 6v12m-3-2.818.879.659c1.171.879 3.07.879 4.242 0 1.172-.879 1.172-2.303 0-3.182C13.536 12.219 12.768 12 12 12c-.725 0-1.45-.22-2.003-.659-1.106-.879-1.106-2.303 0-3.182s2.9-.879 4.006 0l.415.33M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z"/></svg>
          </div>
          <div>
            <p class="text-sm font-medium text-content-subtle">Ingreso total</p>
            <p class="text-2xl font-bold text-heading">{{ totalIncome }}</p>
          </div>
        </div>
      </div>

      <div class="rounded-xl bg-white border border-[#dcc0be]/20 p-6 shadow-sm">
        <div class="flex items-center gap-4">
          <div class="flex h-12 w-12 items-center justify-center rounded-full bg-gold/10">
            <svg class="h-6 w-6 text-gold" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M15.75 6a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0ZM4.501 20.118a7.5 7.5 0 0 1 14.998 0A17.933 17.933 0 0 1 12 21.75c-2.676 0-5.216-.584-7.499-1.632Z"/></svg>
          </div>
          <div>
            <p class="text-sm font-medium text-content-subtle">Ingreso promedio por asiento</p>
            <p class="text-2xl font-bold text-heading">{{ averageSeatIncome }}</p>
          </div>
        </div>
      </div>
    </section>
    <TransactionsTable
      title="Transacciones Recientes"
      :columns="tableColumns"
      :rows="transactions"
      :loading="isLoading"
      empty-message="No existen transacciones para el período seleccionado"
    />
    <section class="flex justify-end">
      <button
        type="button"
        class="inline-flex items-center gap-2 rounded-full bg-primary px-8 py-3 text-sm font-semibold text-white shadow-lg transition hover:opacity-90"
        @click="downloadReport"
      >
        <svg class="h-4 w-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M3 16.5v2.25A2.25 2.25 0 0 0 5.25 21h13.5A2.25 2.25 0 0 0 21 18.75V16.5M16.5 12 12 16.5m0 0L7.5 12m4.5 4.5V3"/></svg>
        Descargar reporte
      </button>
    </section>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import TransactionsTable from '../../../shared/components/TransactionsTable.vue'
import FilterCard from '../../../shared/components/FilterCard.vue'
import ErrorModal from '../../../shared/ErrorModal.vue'
import { useForm } from '../../../shared/useForm.js'
import { useFilterOptionsStore } from '../store/filterOptionStore'
import { getIncomeReport, exportIncomeReport } from '../service/incomeReportService'
import { formatMonthLabel } from '../helpers/monthNames'

const { showErrorModal, errorMessage, onError, resetModals } = useForm()

const filterOptionsStore = useFilterOptionsStore()
const { years, origins, destinations, airlines } = storeToRefs(filterOptionsStore)

onMounted(() => {
  filterOptionsStore.ensureLoaded()
})

const isLoading = ref(false)

const filters = ref({
  year: '',
  origin: '',
  destination: '',
  airline: ''
})

const filterConfig = computed(() => [
  {
    key: 'year',
    label: 'Año',
    type: 'select',
    options: [
      { label: 'Todos', value: '' },
      ...years.value.map(y => ({ label: String(y), value: String(y) }))
    ]
  },
  {
    key: 'origin',
    label: 'Origen',
    type: 'select',
    options: [
      { label: 'Todos', value: '' },
      ...origins.value.map(o => ({ label: o.airportCode, value: o.airportCode }))
    ]
  },
  {
    key: 'destination',
    label: 'Destino',
    type: 'select',
    options: [
      { label: 'Todos', value: '' },
      ...destinations.value.map(d => ({ label: d.airportCode, value: d.airportCode }))
    ]
  },
  {
    key: 'airline',
    label: 'Aerolínea',
    type: 'select',
    options: [
      { label: 'Todas', value: '' },
      ...airlines.value.map(a => ({ label: a.airlineName, value: a.airlineCode }))
    ]
  }
])

const tableColumns = [
  { key: 'month', label: 'Mes' },
  { key: 'flights', label: 'Cantidad de vuelos', align: 'right', format: 'number' },
  { key: 'firstClass', label: 'Pasajeros Primera Clase', align: 'right', format: 'number' },
  { key: 'touristClass', label: 'Pasajeros Clase Turista', align: 'right', format: 'number' },
  { key: 'totalPassengers', label: 'Total Pasajeros', align: 'right', format: 'number' },
  { key: 'ticketIncome', label: 'Ingreso Tiquetes', align: 'right', format: 'money' },
  { key: 'baggageIncome', label: 'Ingreso Maletas', align: 'right', format: 'money' },
  { key: 'totalIncome', label: 'Total Ingreso', align: 'right', format: 'money', font: 'bold' }
]

const transactions = ref([])
const summary = ref(null)

const totalIncome = computed(() => {
  const total = summary.value?.totalIncome ?? 0
  return '$' + total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
})

const averageSeatIncome = computed(() => {
  const totalPassengers = summary.value?.totalPassengers ?? 0
  const total = summary.value?.totalIncome ?? 0
  if (totalPassengers === 0) return '$0.00'
  return '$' + (total / totalPassengers).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
})

const applyFilters = async () => {
  if (!filters.value.year) {
    onError(new Error('Debe seleccionar un año para generar el reporte'))
    return
  }

  isLoading.value = true
  try {
    const data = await getIncomeReport(filters.value)
    const sorted = [...data.rows].sort((a, b) => a.year - b.year || a.month - b.month)
    transactions.value = sorted.map(r => ({
      month: formatMonthLabel(r.year, r.month),
      flights: r.flights,
      firstClass: r.firstClass,
      touristClass: r.touristClass,
      totalPassengers: r.totalPassengers,
      ticketIncome: r.ticketIncome,
      baggageIncome: r.baggageIncome,
      totalIncome: r.totalIncome
    }))
    summary.value = data.summary
  } catch (e) {
    onError(e, 'Error al cargar el reporte de ingresos')
  } finally {
    isLoading.value = false
  }
}

const downloadReport = async () => {
  if (!filters.value.year) {
    onError(new Error('Debe seleccionar un año para generar el reporte'))
    return
  }

  isLoading.value = true
  try {
    const { blob, filename } = await exportIncomeReport(filters.value)
    const url = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = filename || `reporte_ingresos_${filters.value.year}.xlsx`
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
    window.URL.revokeObjectURL(url)
  } catch (e) {
    onError(e, 'Error al descargar el reporte de ingresos')
  } finally {
    isLoading.value = false
  }
}
</script>
