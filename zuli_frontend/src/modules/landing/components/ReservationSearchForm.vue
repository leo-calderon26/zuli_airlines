<template>
  <div class="w-full flex flex-col items-center">
    <div class="w-full mb-8">
      <h1 class="text-4xl md:text-5xl font-bold text-white drop-shadow-md">
        Encuentra tu reservación
      </h1>
    </div>

    <div class="w-full bg-white rounded-xl shadow-2xl p-8 md:p-10 relative z-20">
      <h2 class="text-lg font-semibold text-primary mb-8">
        Ingresa los detalles de tu reserva
      </h2>

      <form
        @submit.prevent="submitForm"
        class="grid grid-cols-1 md:grid-cols-[1fr_1fr_auto] gap-8 items-end"
      >
        <div class="relative">
          <label class="flex items-center gap-2 text-sm font-bold text-primary mb-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 110 4v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 110-4V7a2 2 0 00-2-2H5z" />
            </svg>
            Código de reserva
          </label>
          <input
            v-model="reservationCode"
            type="text"
            placeholder="Ej: KJN8P289"
            class="w-full bg-transparent border-0 border-b-2 border-border-light px-0 py-2 text-content focus:ring-0 focus:border-primary outline-none transition-colors uppercase placeholder:normal-case font-medium"
            required
            maxlength="8"
          />
        </div>

        <div class="relative">
          <label class="flex items-center gap-2 text-sm font-bold text-primary mb-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
            Apellidos del pasajero
          </label>
          <input
            v-model="lastName"
            type="text"
            placeholder="Ej: Arias Paniagua"
            class="w-full bg-transparent border-0 border-b-2 border-border-light px-0 py-2 text-content focus:ring-0 focus:border-primary outline-none transition-colors font-medium"
            required
          />
        </div>

        <div class="pb-1">
          <AppButton
            type="submit"
            variant="primary"
            class="w-full md:w-32 !rounded-md"
            :loading="isLoading"
          >
            Buscar
          </AppButton>
        </div>
      </form>

      <div
        v-if="error"
        class="mt-6 p-4 bg-error-soft text-error text-sm rounded-md border border-error-border flex items-center gap-2"
      >
        <svg class="w-5 h-5 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        {{ error }}
      </div>
    </div>

    <div class="absolute bottom-0 left-0 w-full flex justify-center pointer-events-none z-10 opacity-90 mask-image-bottom">
      <img
        src="https://www.transparenttextures.com/patterns/stardust.png"
        alt="Fondo Decorativo"
        class="max-w-4xl w-full h-[400px] object-cover mix-blend-screen opacity-50"
      />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import AppButton from '../../../shared/AppButton.vue';

defineProps({
  isLoading: Boolean,
  error: String
});

const emit = defineEmits(['search']);

const reservationCode = ref('');
const lastName = ref('');

const submitForm = () => {
  emit('search', {
    reservationCode: reservationCode.value,
    lastName: lastName.value
  });
};
</script>

<style scoped>
.mask-image-bottom {
  mask-image: linear-gradient(to bottom, transparent 0%, black 50%, transparent 100%);
  -webkit-mask-image: linear-gradient(to bottom, transparent 0%, black 50%, transparent 100%);
}
</style>