<template>
  <div class="bg-white border-t-4 border-t-primary rounded-b-xl shadow-2xl p-6 md:p-8 relative transition-all duration-300">
    <div class="flex justify-between items-start mb-6">
      <div>
        <h4 class="font-bold text-xl text-content">Salida {{ formatDateHeader(journey.departureDateTime) }}</h4>
        <p class="text-sm text-content-muted mt-1">
          {{ journey.originCity }} ({{ journey.originCode }}) &rarr; {{ journey.destinationCity }} ({{ journey.destinationCode }})
        </p>
        <p class="text-xs text-content-light mt-2 font-medium">
          {{ journey.stops === 0 ? 'Directo' : journey.stops + ' escala(s)' }} &bull; {{ formatDuration(journey.totalDurationMinutes) }}
        </p>
      </div>
        <div class="text-right flex flex-col items-end">
        <div class="flex items-center gap-1.5">
            <p class="text-xs font-bold text-primary tracking-widest uppercase">
            {{ journey.flightClass }}
            </p>
            <span class="w-1.5 h-4 bg-primary rounded-full shrink-0"></span>
        </div>
        </div>
    </div>

    <div class="py-6 my-6 border-y border-border-xlight overflow-x-auto">
      <div class="flex justify-between items-end relative min-w-[400px]">
        <div class="absolute top-1/2 left-4 right-4 h-0.5 bg-border-light -translate-y-1/2 z-0"></div>

        <div class="relative z-10 flex flex-col items-start bg-white pr-4">
          <span class="text-2xl font-bold text-content leading-none">{{ formatTime(journey.departureDateTime) }}</span>
          <div class="w-3 h-3 rounded-full bg-primary my-2 ring-4 ring-white"></div>
          <span class="text-sm font-bold text-content">{{ journey.originCode }}</span>
        </div>

        <div v-for="(layover, idx) in journey.layovers" :key="idx" class="relative z-10 flex flex-col items-center bg-white px-2">
          <div class="w-2.5 h-2.5 rounded-full bg-gold my-2 ring-4 ring-white"></div>
          <span class="text-xs font-bold text-content">{{ layover.airportCode }}</span>
          <span class="text-[10px] text-content-light font-medium">{{ formatDuration(layover.durationMinutes) }}</span>
        </div>

        <div class="relative z-10 flex flex-col items-end bg-white pl-4">
          <div class="flex items-center gap-1 mb-1">
            <span v-if="arrivesNextDay(journey.departureDateTime, journey.arrivalDateTime)" class="bg-border-xlight text-[10px] font-bold px-2 py-0.5 rounded text-content-subtle border border-border-light">
              {{ getMonthShort(journey.arrivalDateTime) }}<br>{{ getDay(journey.arrivalDateTime) }}
            </span>
            <span class="text-2xl font-bold text-content leading-none">{{ formatTime(journey.arrivalDateTime) }}</span>
          </div>
          <div class="w-3 h-3 rounded-full bg-primary my-2 ring-4 ring-white"></div>
          <span class="text-sm font-bold text-content">{{ journey.destinationCode }}</span>
        </div>
      </div>
    </div>

    <div class="flex flex-wrap justify-between items-center gap-4 mt-2">
      <div class="text-xs text-content-muted font-medium flex flex-wrap gap-4">
        <span v-for="(segment, idx) in journey.segments" :key="idx" class="flex items-center gap-1">
          <svg class="w-3.5 h-3.5 text-primary" fill="currentColor" viewBox="0 0 24 24"><path d="M21 16v-2l-8-5V3.5c0-.83-.67-1.5-1.5-1.5S10 2.67 10 3.5V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5l8 2.5z"/></svg>
          {{ segment.airline }} {{ segment.flightNumber }} &bull; {{ segment.aircraftModel }}
        </span>
      </div>
      <button @click="showDetails = !showDetails" class="flex items-center gap-1 text-primary text-sm font-bold border-b border-primary pb-0.5 hover:text-gold hover:border-gold transition-colors">
        Detalles de vuelo
        <svg :class="{'rotate-180': showDetails}" class="w-4 h-4 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path></svg>
      </button>
    </div>

    <div v-if="showDetails" class="mt-8 pt-6 border-t border-border-xlight">
      
      <div v-if="passengers && passengers.length > 0" class="mb-8">
        <h4 class="font-bold text-content mb-4 text-lg">Pasajeros</h4>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div v-for="(passenger, idx) in passengers" :key="idx" class="flex items-center gap-3 bg-surface-muted p-4 rounded-lg border border-border-xlight shadow-sm">
            <div class="bg-primary/10 p-2 rounded-full text-primary shrink-0">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
            </div>
            <div class="min-w-0">
              <p class="font-bold text-content text-sm capitalize truncate">{{ [passenger.firstName, passenger.firstLastName, passenger.secondLastName].filter(Boolean).join(' ').toLowerCase() }}</p>
            </div>
          </div>
        </div>
      </div>

      <h4 class="font-bold text-content mb-4 text-lg">Itinerario del vuelo</h4>
      <div class="relative border-l-2 border-primary ml-3 pl-6 space-y-6">
        <div v-for="(segment, index) in journey.segments" :key="index" class="relative">
          <div class="absolute w-3 h-3 bg-primary rounded-full -left-[1.65rem] top-1.5 border-2 border-white"></div>
          
          <div class="bg-surface-muted p-4 md:p-5 rounded-lg border border-border-xlight shadow-sm">
            <p class="text-sm font-bold text-content mb-3">{{ segment.airline }} {{ segment.flightNumber }} | {{ formatDateHeader(segment.departureDateTime) }}</p>
            <div class="flex justify-between items-center text-sm">
              <div class="flex flex-col">
                <span class="font-bold text-lg text-content">{{ formatTime(segment.departureDateTime) }}</span>
                <span class="text-content-muted font-medium">{{ segment.originCode }}</span>
              </div>
              <div class="flex flex-col items-center px-4">
                <span class="text-xs font-semibold text-content-muted">{{ formatDuration(segment.durationMinutes) }}</span>
                <span class="w-16 md:w-24 h-px bg-border-medium my-1.5"></span>
                <span class="text-[10px] text-content-light font-medium">{{ segment.aircraftModel }}</span>
              </div>
              <div class="flex flex-col text-right">
                <span class="font-bold text-lg text-content">{{ formatTime(segment.arrivalDateTime) }}</span>
                <span class="text-content-muted font-medium">{{ segment.destinationCode }}</span>
              </div>
            </div>
          </div>

          <div v-if="index < journey.segments.length - 1" class="mt-4 mb-2 text-sm font-semibold text-gold flex items-center gap-2">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
            Escala de {{ formatDuration(journey.layovers[index]?.durationMinutes || 0) }} en {{ segment.destinationCode }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { arrivesNextDay, formatDuration, formatTime, getMonthShort, getDay, formatDateHeader } from '../utils/dateFormatter';

const props = defineProps({
  journey: {
    type: Object,
    required: true
  },
  passengers: {
    type: Array,
    required: false,
    default: () => []
  }
});

const showDetails = ref(false);
</script>