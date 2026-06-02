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
        <path d="M20 21a8 8 0 0 0-16 0" />
        <circle cx="12" cy="7" r="4" />
      </svg>

      <h2 class="text-[18px] font-medium text-content">
        Información del Comprador
      </h2>
    </div>

    <div class="space-y-7">
      <div>
        <p class="text-[16px] text-[#6b5a59]">
          Nombre Completo
        </p>
        <p class="mt-1 text-[18px] font-semibold text-content">
          {{ buyerName || '-' }}
        </p>
      </div>

      <div>
        <p class="text-[16px] text-[#6b5a59]">
          Correo Electrónico
        </p>
        <p class="mt-1 text-[18px] text-content">
          {{ buyerEmail || '-' }}
        </p>
      </div>

      <div>
        <p class="text-[16px] text-[#6b5a59]">
          Teléfono
        </p>
        <p class="mt-1 text-[18px] text-content">
          {{ buyerPhone || '-' }}
        </p>
      </div>
    </div>
  </section>
</template>

<script setup>
defineProps({
  buyerName: {
    type: String,
    default: '-',
  },
  buyerEmail: {
    type: String,
    default: '-',
  },
  buyerPhone: {
    type: String,
    default: '-',
  },
})
</script>