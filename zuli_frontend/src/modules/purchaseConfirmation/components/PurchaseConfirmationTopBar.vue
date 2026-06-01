<template>
  <header class="h-[66px] w-full bg-primary">
    <div class="mx-auto flex h-full max-w-7xl items-center px-6">
      <button
        type="button"
        class="flex items-center"
        @click="$emit('goHome')"
      >
        <img
          src="../../../assets/logoZuli.svg"
          alt="Zuli Airlines"
          class="h-[48px] w-auto"
        >
      </button>

      <nav class="ml-8 flex items-center gap-10">
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
    </div>
  </header>
</template>

<script setup>
defineEmits([
  'goHome',
  'goDestination',
  'goReservation',
])
</script>