<script setup>
import { computed } from 'vue'

const props = defineProps({
  totalRecords: { type: Number, default: 0 },
  pageNumber: { type: Number, default: 1 },
  totalPages: { type: Number, default: 0 },
  pageSize: { type: Number, default: 10 },
  loading: { type: Boolean, default: false },
  empty: { type: Boolean, default: false },
  emptyText: { type: String, default: 'No se encontraron elementos.' },
  loadingText: { type: String, default: 'Cargando...' },
})

const emit = defineEmits(['changePage'])

const startIndex = computed(() => {
  if (props.totalRecords === 0) return 0
  return (props.pageNumber - 1) * props.pageSize + 1
})

const endIndex = computed(() => {
  return Math.min(props.pageNumber * props.pageSize, props.totalRecords)
})
</script>

<template>
  <div class="relative overflow-hidden overflow-x-auto rounded-xl border border-gray-200 bg-white shadow-sm">
    <div v-if="$slots.header" class="flex items-center justify-between gap-4 border-b border-gray-200 p-4">
      <slot name="header" />
    </div>

    <table class="min-w-[1200px] w-full text-left text-base text-gray-600">
      <thead v-if="$slots.thead" class="border-y border-gray-200 bg-gray-50 text-base text-gray-700">
        <tr>
          <slot name="thead" />
        </tr>
      </thead>
      <tbody>
        <slot />
      </tbody>
      <tbody v-if="loading">
        <tr>
          <td :colspan="99" class="px-8 py-10 text-center text-gray-400">
            {{ loadingText }}
          </td>
        </tr>
      </tbody>
      <tbody v-else-if="empty">
        <tr>
          <td :colspan="99" class="px-8 py-10 text-center text-gray-400">
            {{ emptyText }}
          </td>
        </tr>
      </tbody>
    </table>

    <div v-if="totalPages > 0" class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6">
      <div class="flex flex-1 justify-between sm:hidden">
        <button
          @click="emit('changePage', pageNumber - 1)"
          :disabled="pageNumber === 1"
          class="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Anterior
        </button>
        <button
          @click="emit('changePage', pageNumber + 1)"
          :disabled="pageNumber === totalPages"
          class="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Siguiente
        </button>
      </div>
      <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
        <div>
          <p class="text-sm text-gray-700">
            Mostrando <span class="font-medium">{{ startIndex }}</span> a
            <span class="font-medium">{{ endIndex }}</span> de
            <span class="font-medium">{{ totalRecords }}</span> resultados
          </p>
        </div>
        <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
          <button
            @click="emit('changePage', pageNumber - 1)"
            :disabled="pageNumber === 1"
            class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span class="sr-only">Anterior</span>
            <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z" clip-rule="evenodd" />
            </svg>
          </button>
          <button
            v-for="page in totalPages"
            :key="page"
            @click="emit('changePage', page)"
            :aria-current="page === pageNumber ? 'page' : undefined"
            :class="page === pageNumber
              ? 'z-10 bg-primary text-white focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary'
              : 'text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0'"
            class="relative inline-flex items-center px-4 py-2 text-sm font-semibold"
          >
            {{ page }}
          </button>
          <button
            @click="emit('changePage', pageNumber + 1)"
            :disabled="pageNumber === totalPages"
            class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span class="sr-only">Siguiente</span>
            <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z" clip-rule="evenodd" />
            </svg>
          </button>
        </nav>
      </div>
    </div>
  </div>
</template>
