<template>
  <div class="w-full bg-white rounded-xl shadow-md border border-gray-200 overflow-hidden mb-6 transition-all hover:shadow-lg">
    <div class="grid grid-cols-1 md:grid-cols-12 gap-0">
      
      <div class="col-span-1 md:col-span-6 p-6 flex flex-col justify-between border-b md:border-b-0 md:border-r border-gray-200">
        <div class="flex justify-between items-start mb-4">
            <span class="text-xs font-bold tracking-wider text-gray-500 uppercase">
              {{ flight.stops === 0 ? 'Directo' : flight.stops + ' Escala(s)' }}
            </span>
            <span class="text-xs font-semibold text-primary bg-surface px-2 py-1 rounded-md">
               {{ flight.totalDurationText }}
            </span>
        </div>

        <div class="flex items-center justify-between mt-2">
            <div class="text-left">
                <p class="text-2xl font-bold text-gray-900">{{ flight.departureTimeText }}</p>
                <p class="text-sm font-semibold text-gray-500">{{ flight.origin }}</p>
            </div>
            
            <div class="flex-1 px-4 flex items-center justify-center relative">
                <div class="w-full h-px bg-gray-300"></div>
                <div class="absolute bg-white px-2 text-gold">
                   <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 transform rotate-90" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                   </svg>
                </div>
            </div>

            <!-- Destino -->
            <div class="text-right">
                <p class="text-2xl font-bold text-gray-900">{{ flight.arrivalTimeText }}</p>
                <p class="text-sm font-semibold text-gray-500">{{ flight.destination }}</p>
                <p v-if="flight.arrivalDateText" class="text-xs text-error font-medium mt-1">{{ flight.arrivalDateText }}</p>
            </div>
        </div>

        <div class="mt-6">
            <button @click="toggleDetails" class="text-sm font-semibold text-primary hover:text-gold transition flex items-center gap-1">
                Detalles 
                <svg :class="{'rotate-180': showDetails}" class="w-4 h-4 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path></svg>
            </button>
        </div>
      </div>

      <div class="col-span-1 md:col-span-3 border-b md:border-b-0 md:border-r border-gray-200 bg-gray-50 hover:bg-surface transition flex flex-col justify-between p-6">
        <div>
            <h3 class="font-bold text-gray-800 mb-1 text-sm">Clase Turista</h3>
            <p class="text-xs text-gray-500">Cabina principal</p>
        </div>
        <div class="mt-6 text-center">
            <p class="text-sm text-gray-500 mb-1">Desde</p>
            <p class="text-3xl font-bold text-primary mb-4">${{ flight.totalPrice }}</p>
            <button class="w-full py-2 bg-primary text-white font-semibold rounded-lg hover:bg-select transition shadow-sm active:scale-95">
                Seleccionar
            </button>
        </div>
      </div>

      <div class="col-span-1 md:col-span-3 bg-gray-50 hover:bg-surface transition flex flex-col justify-between p-6">
        <div>
            <h3 class="font-bold text-gray-800 mb-1 text-sm">Primera Clase</h3>
            <p class="text-xs text-gray-500">Asiento más amplio</p>
        </div>
        <div class="mt-6 text-center">
            <p class="text-sm text-gray-500 mb-1">Desde</p>
            <p class="text-3xl font-bold text-primary mb-4">${{ (flight.totalPrice * 1.8).toFixed(2) }}</p>
            <button class="w-full py-2 border-2 border-primary text-primary font-semibold rounded-lg hover:bg-primary hover:text-white transition shadow-sm active:scale-95">
                Seleccionar
            </button>
        </div>
      </div>

    </div>

    <div v-if="showDetails" class="bg-white border-t border-gray-200 p-6">
        <h4 class="font-bold text-gray-800 mb-4">Itinerario del vuelo</h4>
        <div class="relative border-l-2 border-primary ml-3 pl-6 space-y-6">
            <div v-for="(segment, index) in flight.segments" :key="segment.flightId" class="relative">
                <div class="absolute w-3 h-3 bg-primary rounded-full -left-[1.65rem] top-1.5 border-2 border-white"></div>
                
                <div class="bg-gray-50 p-4 rounded-lg border border-gray-200">
                    <p class="text-sm font-bold text-gray-800 mb-2">Vuelo {{ segment.flightId }}</p>
                    <div class="flex justify-between items-center text-sm">
                        <div>
                            <p class="font-semibold">{{ segment.departureTimeText }} - {{ segment.origin }}</p>
                        </div>
                        <div class="text-gray-500 flex flex-col items-center px-4">
                            <span>{{ segment.durationText }}</span>
                            <span class="w-12 h-px bg-gray-300 my-1"></span>
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

const props = defineProps({
  flight: {
    type: Object,
    required: true
  }
});

const showDetails = ref(false);

const toggleDetails = () => {
    showDetails.value = !showDetails.value;
};
</script>