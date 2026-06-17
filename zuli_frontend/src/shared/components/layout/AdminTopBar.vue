<template>
  <header class="sticky top-0 z-20 bg-white border-b border-gray-200">
    <div class="flex items-center justify-between px-4 py-3">
      <button
        type="button"
        @click="$emit('toggle-sidebar')"
        class="p-2 rounded-lg hover:bg-gray-100 transition"
        aria-label="Toggle sidebar"
      >
        <Bars3Icon class="w-6 h-6 text-gray-700" />
      </button>

      <div class="flex-1 lg:pl-4">
        <h2 class="text-lg font-semibold text-gray-900">Panel de Reportes</h2>
      </div>
    </div>
  </header>
</template>

<script setup>
import { Bars3Icon } from '@heroicons/vue/24/outline'

defineEmits(['toggle-sidebar'])
</script>
