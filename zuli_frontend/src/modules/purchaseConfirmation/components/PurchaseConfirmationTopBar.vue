<template>
  <header class="flex h-[84px] w-full items-center bg-[var(--color-primary)] px-8 text-white">
    <div class="flex w-52 shrink-0 items-center">
      <button
        type="button"
        class="flex items-center"
        @click="$emit('goHome')"
      >
        <img
          src="../../../assets/logoZuli.svg"
          alt="Zuli Airlines"
          class="h-20 w-auto object-contain"
        >
      </button>
    </div>

    <nav class="flex items-center gap-10">
      <button
        type="button"
        class="text-[16px] font-medium text-white transition hover:opacity-80"
        @click="$emit('goDestination')"
      >
        Destino
      </button>

      <button
        type="button"
        class="text-[16px] font-medium text-white transition hover:opacity-80"
        @click="$emit('goReservation')"
      >
        Reserva
      </button>
    </nav>
  </header>
</template>

<script setup>
defineEmits([
  'goHome',
  'goDestination',
  'goReservation',
])
</script>