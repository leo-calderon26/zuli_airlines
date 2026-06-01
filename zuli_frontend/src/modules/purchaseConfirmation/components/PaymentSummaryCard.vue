<template>
  <section class="rounded-[18px] bg-white px-7 py-7 shadow-md">
    <div class="mb-7 flex items-center gap-3">
      <svg
        class="h-6 w-6 text-primary"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        stroke-linecap="round"
        stroke-linejoin="round"
      >
        <rect x="3" y="5" width="16" height="16" rx="1.5" />
        <path d="M7 9h8" />
        <path d="M7 13h8" />
        <path d="M7 17h5" />
        <path d="M18 3h3v3" />
      </svg>

      <h2 class="text-[18px] font-medium text-content">
        Pago y Resumen
      </h2>
    </div>

    <div class="rounded-[12px] bg-[#f5f1f1] px-5 py-5">
      <div class="flex items-center justify-between gap-4">
        <div class="flex items-center gap-4">
          <svg
            class="h-9 w-9 text-[#92701B]"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="1.8"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <rect x="2" y="5" width="20" height="14" rx="2" />
            <path d="M2 10h20" />
          </svg>

          <div>
            <p class="text-[16px] text-[#6b5a59]">
              Método Utilizado
            </p>

            <p class="mt-1 text-[18px] font-semibold uppercase text-content">
              {{ paymentMethod || '-' }}
            </p>
          </div>
        </div>

        <svg
          class="h-7 w-7 shrink-0"
          viewBox="0 0 24 24"
          fill="none"
        >
          <path
            d="M12 2l7 3v6c0 5-3.5 9.3-7 11-3.5-1.7-7-6-7-11V5l7-3Z"
            fill="#92701B"
          />
          <path
            d="M8.3 12.1l2.2 2.2 5.2-5.2"
            stroke="white"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </div>
    </div>

    <div class="mt-7 border-t border-[#ebe3e3] pt-6">
      <div class="flex items-center justify-between gap-4">
        <span class="text-[18px] text-[#554241]">
          Clase Seleccionada
        </span>

        <span class="text-[18px] font-semibold text-content">
          {{ flightClass || '-' }}
        </span>
      </div>

      <div class="mt-7 flex items-end justify-between gap-4">
        <span class="text-[18px] text-content">
          Total pagado
        </span>

        <span class="text-[24px] font-medium text-primary">
          {{ formattedTotal }}
        </span>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed } from 'vue'

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
  return new Intl.NumberFormat('es-ES', {
    style: 'currency',
    currency: 'EUR',
  }).format(props.totalAmount ?? 0)
})
</script>