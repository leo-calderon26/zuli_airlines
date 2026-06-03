<template>
  <div class="w-full bg-white rounded-xl shadow-md border border-border-light overflow-hidden mb-6 transition-all hover:shadow-lg">
    <div class="grid grid-cols-1 md:grid-cols-12 gap-0">
      
      <div class="col-span-1 md:col-span-6 p-6 flex flex-col justify-between border-b md:border-b-0 md:border-r border-border-light">
        <div class="flex justify-between items-start mb-4">
            <span class="text-xs font-bold tracking-wider text-content-muted uppercase">
              {{ flight.stops === 0 ? 'Directo' : flight.stops + ' Escala(s)' }}
            </span>
            <span class="text-xs font-semibold text-primary bg-surface px-2 py-1 rounded-md">
               {{ flight.totalDurationText }}
            </span>
        </div>

        <div class="flex items-center justify-between mt-2">
            <div class="text-left">
                <p class="text-2xl font-bold text-heading">{{ flight.departureTimeText }}</p>
                <p class="text-sm font-semibold text-content-muted">{{ flight.origin }}</p>
            </div>
            
            <div class="flex-1 px-4 flex items-center justify-center relative">
                <div class="w-full h-px bg-border-medium"></div>
                <div class="absolute bg-white px-2 text-gold">
                   <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 transform rotate-90" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                   </svg>
                </div>
            </div>

            <!-- Destino -->
            <div class="text-right">
                <p class="text-2xl font-bold text-heading">{{ flight.arrivalTimeText }}</p>
                <p class="text-sm font-semibold text-content-muted">{{ flight.destination }}</p>
                <p v-if="flight.arrivalDateText" class="text-xs text-error font-medium mt-1">{{ flight.arrivalDateText }}</p>
            </div>
        </div>

        <div class="mt-6">
            <AppButton @click="toggleDetails" variant="link" class="text-sm font-semibold !gap-1">
                Detalles 
                <svg :class="{'rotate-180': showDetails}" class="w-4 h-4 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path></svg>
            </AppButton>
        </div>
      </div>

      <div class="col-span-1 md:col-span-3 border-b md:border-b-0 md:border-r border-border-light bg-surface-muted hover:bg-surface transition flex flex-col justify-between p-6">
        <div>
            <h3 class="font-bold text-content mb-1 text-sm">Clase Turista</h3>
            <p class="text-xs text-content-muted">Cabina principal</p>
        </div>
        <div class="mt-6 text-center">
            <p class="text-sm text-content-muted mb-1">Desde</p>
            <p class="text-3xl font-bold text-primary mb-4">${{ flight.totalTouristPrice.toFixed(2) }}</p>
            <AppButton variant="primary" class="w-full" @click="selectFlight('Turista', flight.totalTouristPrice.toFixed(2))">
                Seleccionar
            </AppButton>
        </div>
      </div>

      <div class="col-span-1 md:col-span-3 bg-surface-muted hover:bg-surface transition flex flex-col justify-between p-6">
        <div>
            <h3 class="font-bold text-content mb-1 text-sm">Primera Clase</h3>
            <p class="text-xs text-content-muted">Asiento más amplio</p>
        </div>
        <div class="mt-6 text-center">
            <p class="text-sm text-content-muted mb-1">Desde</p>
            <p class="text-3xl font-bold text-primary mb-4">${{ flight.totalFirstClassPrice.toFixed(2) }}</p>
            <AppButton variant="outline" class="w-full" @click="selectFlight('Primera Clase', flight.totalFirstClassPrice.toFixed(2))">
                Seleccionar
            </AppButton>
        </div>
      </div>

    </div>

    <div v-if="showDetails" class="bg-white border-t border-border-light p-6">
        <h4 class="font-bold text-content mb-4">Itinerario del vuelo</h4>
        <div class="relative border-l-2 border-primary ml-3 pl-6 space-y-6">
            <div v-for="(segment, index) in flight.segments" :key="segment.flightId" class="relative">
                <div class="absolute w-3 h-3 bg-primary rounded-full -left-[1.65rem] top-1.5 border-2 border-white"></div>
                
                <div class="bg-surface-muted p-4 rounded-lg border border-border-light">
                    <p class="text-sm font-bold text-content mb-2">ZU-{{ segment.flightId }} | {{ segment.departureDateText }} </p>
                    <div class="flex justify-between items-center text-sm">
                        <div>
                            <p class="font-semibold">{{ segment.departureTimeText }} - {{ segment.origin }}</p>
                        </div>
                        <div class="text-content-muted flex flex-col items-center px-4">
                            <span>{{ segment.durationText }}</span>
                            <span class="w-12 h-px bg-border-medium my-1"></span>
                        </div>
                        <div class="text-right">
                            <p class="font-semibold">{{ segment.arrivalTimeText }} - {{ segment.destination }}</p>
                        </div>
                    </div>
                </div>

                <!-- Escala info -->
                <div v-if="index < flight.segments.length - 1" class="mt-4 mb-2 text-sm font-semibold text-gold flex items-center gap-2">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                    Escala de {{ flight.segments[index + 1].layoverTimeText }} en {{ segment.destination }}
                </div>
            </div>
        </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import AppButton from '../../../shared/AppButton.vue';

const props = defineProps({
  flight: {
    type: Object,
    required: true
  }
});

const emit = defineEmits(['selectFlight']);

const showDetails = ref(false);

const toggleDetails = () => {
    showDetails.value = !showDetails.value;
};

const selectFlight = (travelClass, price) => {
    emit('selectFlight', {
        flight: props.flight,
        travelClass,
        price
    });
};
</script>