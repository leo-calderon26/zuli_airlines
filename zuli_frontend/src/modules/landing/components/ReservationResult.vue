<template>
  <div class="w-full flex flex-col z-20">
    <div class="w-full mb-6 flex justify-start">
      <button
        @click="$emit('clear')"
        class="flex items-center gap-2 text-white hover:text-gold transition-colors font-semibold text-lg drop-shadow-md"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M10 19l-7-7m0 0l7-7m-7 7h18"
          />
        </svg>
        Realizar otra búsqueda
      </button>
    </div>

    <div class="bg-white rounded-xl p-8 shadow-2xl mb-8 w-full border border-border-xlight">
      <div class="flex flex-wrap items-center gap-3 mb-4">
        <span
          class="inline-block text-xs font-bold px-3 py-1 rounded-full uppercase tracking-wider border"
          :class="timeStatusClasses"
        >
          {{ remainingTimeText }}
        </span>

        <span
          class="inline-block text-xs font-bold px-3 py-1 rounded-full uppercase tracking-wider border"
          :class="statusClasses"
        >
          {{ statusLabel }}
        </span>
      </div>

      <h2 class="text-3xl font-bold text-primary mb-3">
        Viaje a {{ reservationData.destinationCity }}
        <span class="font-semibold text-content-light">
          {{ reservationData.destinationCode }}
        </span>
      </h2>

      <div class="flex flex-wrap items-center text-sm font-medium text-content-subtle gap-2">
        <span>
          Reserva:
          <strong class="text-primary tracking-wider uppercase">
            {{ reservationData.reservationCode }}
          </strong>
        </span>

        <span class="text-border-medium">|</span>

        <span>
          {{ formatDateRange(reservationData.journey.departureDateTime) }}
        </span>

        <span class="text-border-medium">|</span>

        <span>
          {{ reservationData.passengerCount }} Pasajero(s)
        </span>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8 w-full">
      <div class="lg:col-span-2 space-y-6">
        <h3 class="flex items-center gap-2 font-bold text-white drop-shadow-md text-xl mb-4">
          <svg class="w-6 h-6 text-gold" fill="currentColor" viewBox="0 0 24 24">
            <path
              d="M21 16v-2l-8-5V3.5c0-.83-.67-1.5-1.5-1.5S10 2.67 10 3.5V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5l8 2.5z"
            />
          </svg>
          Detalles del viaje
        </h3>

        <FlightItinerary
          :journey="reservationData.journey"
          :passengers="reservationData.passengers"
        />
      </div>

      <div class="space-y-6 lg:mt-11">
        <div class="bg-white rounded-xl p-6 shadow-2xl">
          <h3 class="font-bold text-content text-lg mb-4 border-b border-border-xlight pb-3">Opciones de viaje</h3>
          <router-link
            :to="{ name: 'additionalBaggage', query: { reservationCode: reservationData.reservationCode } }"
            class="group w-full flex justify-between items-center bg-surface-muted hover:bg-primary hover:text-white transition-all p-4 rounded-lg text-sm font-semibold text-content border border-border-light"
          >
            <span class="flex items-center gap-3">
              <svg
                class="w-5 h-5 text-primary group-hover:text-white transition-colors"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                />
              </svg>
              Gestionar equipaje
            </span>
            <svg class="w-4 h-4 text-content-light group-hover:text-white transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
          </router-link>
        </div>

        <div class="bg-white rounded-xl p-6 shadow-2xl">
          <h3 class="font-bold text-content text-lg mb-4 border-b border-border-xlight pb-3">
            Gestión de la reserva
          </h3>

          <div class="space-y-3">
            <button
              @click="$emit('request-itinerary')"
              :disabled="isDownloading"
              class="group w-full flex justify-between items-center bg-surface-muted hover:bg-primary hover:text-white transition-all p-4 rounded-lg text-sm font-semibold text-content border border-border-light disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span class="flex items-center gap-3">
                <svg v-if="isDownloading" class="w-5 h-5 animate-spin text-primary group-hover:text-white transition-colors" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <svg
                  v-else
                  class="w-5 h-5 text-primary group-hover:text-white transition-colors"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z"
                  />
                </svg>
                {{ isDownloading ? 'Generando PDF...' : 'Imprimir itinerario' }}
              </span>

              <svg
                class="w-4 h-4 text-content-light group-hover:text-white transition-colors"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M9 5l7 7-7 7"
                />
              </svg>
            </button>

            <button
              v-if="!isCancelled"
              @click="$emit('request-cancel')"
              class="group w-full flex justify-between items-center bg-error-soft hover:bg-primary hover:text-white hover:border-transparent transition-all p-4 rounded-lg text-sm font-semibold text-error border border-error-border"
            >
              <span class="flex items-center gap-3">
                <svg
                  class="w-5 h-5 text-error opacity-80 group-hover:text-white group-hover:opacity-100 transition-all"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                  />
                </svg>
                Cancelar reservación
              </span>

              <svg
                class="w-4 h-4 text-error opacity-60 group-hover:text-white group-hover:opacity-100 transition-all"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M9 5l7 7-7 7"
                />
              </svg>
            </button>

            <div
              v-else
              class="w-full flex items-center gap-3 p-4 rounded-lg text-sm font-semibold text-content-muted bg-border-xlight border border-border-light cursor-not-allowed"
            >
              <svg
                class="w-5 h-5 opacity-60"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                />
              </svg>
              Reserva cancelada
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { formatDateRange, getRemainingTimeText } from '../utils/dateFormatter';
import FlightItinerary from './FlightItinerary.vue';

const RESERVATION_STATUS = Object.freeze({
  ACTIVE: 1,
  CANCELLED: 2
});

const props = defineProps({
  reservationData: {
    type: Object,
    required: true
  },
  isDownloading: {
    type: Boolean,
    default: false
  }
});

defineEmits(['clear', 'request-cancel', 'request-itinerary']);

const reservationStatusId = computed(() => {
  return props.reservationData.reservationStatusId;
});

const isCancelled = computed(() => {
  return reservationStatusId.value === RESERVATION_STATUS.CANCELLED;
});

const remainingTimeText = computed(() => {
  return getRemainingTimeText(
    props.reservationData.journey?.departureDateTime,
    props.reservationData.journey?.arrivalDateTime
  );
});

const timeStatusClasses = computed(() => {
  const text = remainingTimeText.value;

  if (text === 'Vuelo en curso') {
    return 'bg-success-soft text-success border-success-border';
  }

  return 'bg-border-xlight text-content-muted border-border-light';
});

const statusLabel = computed(() => {
  if (isCancelled.value) {
    return 'Cancelada';
  }

  const arrivalDate = new Date(
    props.reservationData.journey?.arrivalDateTime
  );

  return new Date() > arrivalDate
    ? 'Concluida'
    : 'Activa';
});

const statusClasses = computed(() => {
  if (isCancelled.value) {
    return 'bg-error-soft text-error border-error-border';
  }

  if (statusLabel.value === 'Concluida') {
    return 'bg-border-xlight text-content-subtle border-border-medium';
  }

  return 'bg-success-soft text-success border-success-border';
});
</script>