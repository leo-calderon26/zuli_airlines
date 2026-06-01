<template>
  <section class="rounded-[18px] bg-white px-7 py-8 shadow-md">
    <div class="flex flex-col gap-6 lg:flex-row lg:items-center">
      <div class="flex h-24 w-24 shrink-0 items-center justify-center rounded-full bg-[#f3ebeb]">
        <svg
          class="h-16 w-16"
          viewBox="0 0 64 64"
          fill="none"
        >
          <path
            d="M32 4L37.1 8.4L43.8 7.6L47.2 13.4L53.6 15.5L54.4 22.2L59.6 26.5L57.2 32.8L59.6 39.1L54.4 43.4L53.6 50.1L47.2 52.2L43.8 58L37.1 57.2L32 61.6L26.9 57.2L20.2 58L16.8 52.2L10.4 50.1L9.6 43.4L4.4 39.1L6.8 32.8L4.4 26.5L9.6 22.2L10.4 15.5L16.8 13.4L20.2 7.6L26.9 8.4L32 4Z"
            fill="#8A1D1D"
          />
          <path
            d="M22.5 32.2L28.5 38.2L42.5 24.2"
            stroke="white"
            stroke-width="4.8"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </div>

      <div class="flex-1">
        <h1 class="text-[34px] font-semibold leading-none text-primary">
          Reserva completada.
        </h1>

        <p class="mt-4 text-[18px] text-[#554241]">
          {{ message }}
        </p>

        <div class="mt-5 inline-flex items-center rounded-[10px] border border-[#e6dbdb] bg-[#f8f4f4] px-5 py-3">
          <span class="text-[16px] uppercase tracking-[0.04em] text-[#6f5a5a]">
            CÓDIGO DE RESERVA:
          </span>

          <span class="ml-3 text-[18px] font-medium tracking-[0.12em] text-primary">
            {{ reservationCode }}
          </span>
        </div>
      </div>

      <div class="hidden h-28 w-px bg-[#ece3e3] lg:block"></div>

      <div class="min-w-[360px] space-y-5">
        <div class="flex items-center gap-4">
          <svg class="h-6 w-6 shrink-0" viewBox="0 0 24 24" fill="none">
            <circle cx="12" cy="12" r="10" fill="#92701B" />
            <path
              d="M8 12.2l2.5 2.5L16.5 8.8"
              stroke="white"
              stroke-width="2.2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>

          <span class="text-[18px] text-content">
            Factura enviada
          </span>
        </div>

        <div class="flex items-center gap-4">
          <svg class="h-6 w-6 shrink-0" viewBox="0 0 24 24" fill="none">
            <circle cx="12" cy="12" r="10" fill="#92701B" />
            <path
              d="M8 12.2l2.5 2.5L16.5 8.8"
              stroke="white"
              stroke-width="2.2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>

          <span class="text-[18px] text-content">
            Confirmación, pasajeros e itinerario enviados.
          </span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
defineProps({
  message: {
    type: String,
    default: 'Los detalles han sido enviados a su correo electrónico.',
  },
  reservationCode: {
    type: String,
    required: true,
  },
})
</script>