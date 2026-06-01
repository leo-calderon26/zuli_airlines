<template>
  <InfoCard title="Pago">
    <div class="rounded-lg bg-[#F4F3F3] p-4">
      <p class="text-sm text-[#554241]">
        Método de pago
      </p>

      <p class="mt-1 font-semibold uppercase text-content">
        {{ paymentMethod || '-' }}
      </p>
    </div>

    <div class="border-t border-[#dcc0be]/30 pt-4">
      <div class="flex justify-between gap-4">
        <span class="text-[#554241]">Clase seleccionada</span>
        <span class="font-medium text-content">{{ flightClass || '-' }}</span>
      </div>

      <div class="mt-3 flex items-center justify-between gap-4">
        <span class="text-content">Total pagado</span>

        <span class="text-2xl font-semibold text-primary">
          {{ formattedTotal }}
        </span>
      </div>
    </div>
  </InfoCard>
</template>

<script setup>
import { computed } from 'vue'
import InfoCard from './InfoCard.vue'

const props = defineProps({
  paymentMethod: {
    type: String,
    default: '-',
  },
  flightClass: {
    type: String,
    default: '-',
  },
  totalAmount: {
    type: Number,
    default: 0,
  },
})

const formattedTotal = computed(() => {
  return new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'USD',
  }).format(props.totalAmount ?? 0)
})
</script>