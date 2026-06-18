<template>
  <section class="rounded-xl bg-white border border-[#dcc0be]/20 p-6 shadow-sm">
    <h2 class="text-lg font-semibold text-heading mb-4">{{ title }}</h2>
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div v-for="filter in filters" :key="filter.key">
        <label class="mb-2 block text-sm font-medium text-content">{{ filter.label }}</label>
        <select
          v-if="filter.type === 'select'"
          v-model="model[filter.key]"
          class="block w-full appearance-none rounded-base border bg-body px-3 py-2.5 text-sm text-content shadow-xs transition-all duration-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
        >
          <option v-for="opt in filter.options" :key="opt.value" :value="opt.value">
            {{ opt.label }}
          </option>
        </select>
        <input 
          v-else-if="filter.type === 'date'"
          type="date"
          v-model="model[filter.key]"
          class="block w-full appearance-none rounded-base border bg-body px-3 py-2.5 text-sm text-content shadow-xs transition-all duration-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
        />

        <input 
          v-else-if="filter.type === 'text'"
          type="text"
          v-model="model[filter.key]"
          :placeholder="filter.placeholder || ''"
          class="block w-full appearance-none rounded-base border bg-body px-3 py-2.5 text-sm text-content shadow-xs transition-all duration-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
        />
      </div>
    </div>
    <div class="mt-4 flex justify-end">
      <button
        type="button"
        class="inline-flex items-center gap-2 rounded-base bg-primary px-6 py-2.5 text-sm font-semibold text-white shadow-md transition hover:brightness-110"
        @click="emitApply"
      >
        <svg class="h-4 w-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" d="m21 21-4.34-4.34M11 19a8 8 0 1 0 0-16 8 8 0 0 0 0 16Z"/>
        </svg>
        Aplicar filtros
      </button>
    </div>
  </section>
</template>

<script setup>
const props = defineProps({
  title: {
    type: String,
    default: 'Filtros'
  },
  filters: {
    type: Array,
    required: true
    // [{ key, label, type: 'select', options: [{ label, value }] }]
  }
})

const model = defineModel({ type: Object, default: () => ({}) })

const emit = defineEmits(['apply'])

const emitApply = () => {
  emit('apply', { ...model.value })
}
</script>
