<template>
  <section class="rounded-xl bg-white border border-[#dcc0be]/20 px-7 py-7 shadow-sm">
    <div class="mb-6 flex items-center justify-between">
      <h2 class="text-[18px] font-medium text-content">
        {{ title }}
      </h2>
      <span v-if="!loading && rows.length > 0" class="text-xs text-content-muted">
        {{ rows.length }} registros
      </span>
    </div>

    <div class="overflow-x-auto">
      <table class="w-full border-collapse min-w-[900px]">
        <thead>
          <tr class="bg-[#f3efef] text-left">
            <th
              v-for="col in columns"
              :key="col.key"
              class="px-5 py-4 text-[16px] font-semibold text-[#5a4847] whitespace-nowrap"
              :class="col.align === 'right' ? 'text-right' : 'text-left'"
            >
              {{ col.label }}
            </th>
          </tr>
        </thead>

        <tbody>
          <template v-if="loading">
            <tr v-for="n in 5" :key="n">
              <td
                v-for="col in columns"
                :key="col.key"
                class="px-5 py-4"
              >
                <div class="h-4 animate-pulse rounded bg-[#f3efef] w-full"></div>
              </td>
            </tr>
          </template>
          <tr v-else-if="rows.length === 0">
            <td
              :colspan="columns.length"
              class="px-5 py-12 text-center"
            >
              <div class="flex flex-col items-center justify-center text-content-muted">
                <svg
                  class="h-10 w-10 mb-3 opacity-60"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="1.5"
                  viewBox="0 0 24 24"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M20.25 6.375c0 2.278-3.694 4.125-8.25 4.125S3.75 8.653 3.75 6.375m16.5 0c0-2.278-3.694-4.125-8.25-4.125S3.75 4.097 3.75 6.375m16.5 0v11.25c0 2.278-3.694 4.125-8.25 4.125s-8.25-1.847-8.25-4.125V6.375m16.5 0v3.75m-16.5-3.75v3.75m16.5 0v3.75C20.25 16.153 16.556 18 12 18s-8.25-1.847-8.25-4.125v-3.75m16.5 0c0 2.278-3.694 4.125-8.25 4.125s-8.25-1.847-8.25-4.125"
                  />
                </svg>
                <p class="font-medium">{{ emptyMessage }}</p>
              </div>
            </td>
          </tr>
          <tr
            v-for="(row, idx) in rows"
            :key="idx"
            class="border-b border-[#ebe3e3] transition hover:bg-gray-50"
          >
            <td
              v-for="col in columns"
              :key="col.key"
              class="px-5 py-4 text-[16px] text-content whitespace-nowrap"
              :class="col.align === 'right' ? 'text-right' : 'text-left'"
            >
              <span :class="col.font === 'bold' ? 'font-semibold text-heading' : ''">
                {{ formatValue(row[col.key], col.format) }}
              </span>
            </td>
          </tr>
          <tr
            v-if="showTotals && !loading && rows.length > 0"
            class="bg-[#faf8f8] border-t-2 border-[#ebe3e3]"
          >
            <td
              v-for="col in columns"
              :key="col.key"
              class="px-5 py-4 text-[16px] font-bold text-heading whitespace-nowrap"
              :class="col.align === 'right' ? 'text-right' : 'text-left'"
            >
              {{ col.key === 'period' || col.key === 'month' || col.key === 'route' || col.key === 'date' ? 'Total' : formatValue(computedTotals[col.key], col.format) }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  title: {
    type: String,
    default: 'Transacciones Recientes'
  },
  columns: {
    type: Array,
    required: true
  },
  rows: {
    type: Array,
    default: () => []
  },
  loading: {
    type: Boolean,
    default: false
  },
  emptyMessage: {
    type: String,
    default: 'No existen transacciones para el período seleccionado'
  },
  showTotals: {
    type: Boolean,
    default: true
  },
  totals: {
    type: Object,
    default: null
  }
})

const computedTotals = computed(() => {
  if (props.totals) return props.totals

  const result = {}
  props.columns.forEach(col => {
    if (col.format === 'number' || col.format === 'money') {
      result[col.key] = props.rows.reduce((sum, row) => {
        const val = parseFloat(String(row[col.key]).replace(/[$,]/g, '')) || 0
        return sum + val
      }, 0)
    }
  })
  return result
})

function formatValue(value, format) {
  if (value == null || value === '') return '-'

  if (format === 'money') {
    const num = typeof value === 'number' ? value : parseFloat(String(value).replace(/[$,]/g, ''))
    if (isNaN(num)) return value
    return '$' + num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
  }

  if (format === 'number') {
    const num = typeof value === 'number' ? value : parseFloat(String(value).replace(/[$,]/g, ''))
    if (isNaN(num)) return value
    return num.toLocaleString('en-US')
  }

  return value
}
</script>
