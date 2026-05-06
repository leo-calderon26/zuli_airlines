<template>
    <div class="flex flex-col min-h-screen bg-body">
        <PublicNavBar />
        
        <main class="flex-1 w-full max-w-7xl mx-auto px-4 py-8">
            
            <!-- Header Busqueda -->
            <div class="bg-primary rounded-xl p-6 text-white mb-8 shadow-md flex flex-col md:flex-row justify-between items-center gap-4">
                <div>
                    <h2 class="text-2xl font-bold flex items-center gap-2">
                        {{ route.query.origin }} 
                        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 5l7 7m0 0l-7 7m7-7H3"></path></svg>
                        {{ route.query.destination }}
                    </h2>
                    <p class="text-white/80 mt-1 font-medium">
                        Fecha: {{ route.query.date }} | Pasajeros: {{ route.query.seats }} | Clase: {{ route.query.flightClass }}
                    </p>
                </div>
                <router-link to="/" class="bg-white/20 hover:bg-white/30 px-6 py-2 rounded-lg font-semibold transition border border-white/40">
                    Modificar Búsqueda
                </router-link>
            </div>

            <!-- Loader -->
            <div v-if="searchStore.isLoading" class="flex justify-center items-center py-20">
                <div class="animate-spin rounded-full h-16 w-16 border-t-4 border-b-4 border-primary"></div>
            </div>

            <!-- Error -->
            <div v-else-if="searchStore.error" class="bg-red-50 text-error p-6 rounded-xl border border-red-200 text-center font-semibold text-lg">
                {{ searchStore.error }}
            </div>

            <!-- Resultados -->
            <div v-else-if="searchStore.flightResults">
                
                <h3 class="text-xl font-bold text-gray-800 mb-4 border-b pb-2">Vuelos de Salida</h3>
                
                <div v-if="searchStore.flightResults.outboundFlights.length === 0" class="text-center py-12 text-gray-500 font-medium">
                    No se encontraron vuelos para esta fecha y ruta.
                </div>

                <FlightCard 
                    v-for="flight in searchStore.flightResults.outboundFlights" 
                    :key="flight.pathIds" 
                    :flight="flight" 
                />

                <!-- Paginación (Ejemplo simple) -->
                <div v-if="searchStore.flightResults.totalPagesOutbound > 1" class="flex justify-center gap-2 mt-8">
                    <button 
                        @click="changePage(searchStore.flightResults.currentPage - 1)"
                        :disabled="searchStore.flightResults.currentPage === 1"
                        class="px-4 py-2 bg-white border border-gray-300 rounded-md text-sm font-medium hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
                        Anterior
                    </button>
                    <span class="px-4 py-2 text-sm font-semibold text-gray-700">
                        Página {{ searchStore.flightResults.currentPage }} de {{ searchStore.flightResults.totalPagesOutbound }}
                    </span>
                    <button 
                        @click="changePage(searchStore.flightResults.currentPage + 1)"
                        :disabled="searchStore.flightResults.currentPage === searchStore.flightResults.totalPagesOutbound"
                        class="px-4 py-2 bg-white border border-gray-300 rounded-md text-sm font-medium hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
                        Siguiente
                    </button>
                </div>
            </div>

        </main>
        
        <PublicBottomBar />
    </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
import PublicNavBar from '../components/PublicNavBar.vue';
import PublicBottomBar from '../components/PublicBottomBar.vue';
import FlightCard from '../components/FlightCard.vue';
import { useFlightSearchStore } from '../store/flightSearchStore';

const route = useRoute();
const searchStore = useFlightSearchStore();

onMounted(() => {
    const params = {
        Origin: route.query.origin,
        Destination: route.query.destination,
        Date: route.query.date,
        Seats: parseInt(route.query.seats) || 1,
        IsRoundTrip: route.query.roundTrip === 'true',
        ReturnDate: route.query.returnDate || null,
        DirectFlightsOnly: route.query.directOnly === 'true',
        FlightClass: route.query.flightClass || 'Turista',
        Page: 1,
        //PageSize: 10 Revisar paginacion
    };
    
    searchStore.performSearch(params);
});

const changePage = (page) => {
    searchStore.changePage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
};
</script>