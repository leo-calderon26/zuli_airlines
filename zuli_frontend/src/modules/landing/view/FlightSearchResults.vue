<template>
    <div class="flex flex-col min-h-screen bg-body">
        <PublicNavBar />
        
        <main class="flex-1 w-full max-w-7xl mx-auto px-4 py-8">
            
            <div class="bg-primary rounded-xl p-6 text-white mb-8 shadow-md flex flex-col md:flex-row justify-between items-center gap-4">
                <div>
                    <h2 class="text-2xl font-bold flex items-center gap-2">
                        {{ searchStore.searchParams.Origin || route.query.origin }} 
                        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 5l7 7m0 0l-7 7m7-7H3"></path></svg>
                        {{ searchStore.searchParams.Destination || route.query.destination }}
                    </h2>
                    <p class="text-white/80 mt-1 font-medium">
                        Fecha: {{ searchStore.searchParams.Date || route.query.date }} 
                        <span v-if="searchStore.searchParams.IsRoundTrip"> | Regreso: {{ searchStore.searchParams.ReturnDate || route.query.returnDate }}</span>
                        | Pasajeros: {{ searchStore.searchParams.Seats || route.query.seats }} 
                    </p>
                </div>
            <router-link to="/" class="inline-block rounded-md bg-white/5 px-4 py-2 font-semibold text-white transition hover:bg-black/15">
                    Modificar Búsqueda
                </router-link>
            </div>

            <div v-if="searchStore.isLoading" class="flex justify-center items-center py-20">
                <div class="animate-spin rounded-full h-16 w-16 border-t-4 border-b-4 border-primary"></div>
            </div>

            <div v-else-if="searchStore.error" class="bg-error-soft text-error p-6 rounded-xl border border-error-border text-center font-semibold text-lg">
                {{ searchStore.error }}
            </div>

            <div v-else-if="searchStore.flightResults">
                
                <div v-if="!selectedDepartureFlight">
                    <h3 class="text-xl font-bold text-content mb-4 border-b border-border-light pb-2">
                        Selecciona tu vuelo de salida ({{ searchStore.searchParams.Origin }} a {{ searchStore.searchParams.Destination }})
                    </h3>
                    
                    <div v-if="searchStore.flightResults.departureFlights?.length === 0" class="text-center py-12 text-content-muted font-medium">
                        No se encontraron vuelos de salida para esta fecha y ruta.
                    </div>

                    <FlightCard 
                        v-for="flight in searchStore.flightResults.departureFlights" 
                        :key="'out-'+flight.pathIds" 
                        :flight="flight"
                        @selectFlight="handleSelectDeparture" 
                    />

                    <div v-if="searchStore.flightResults.totalPagesDeparture > 1" class="flex justify-center items-center gap-4 mt-8">
                        <AppButton 
                            @click="changePage(searchStore.flightResults.currentPage - 1)"
                            :disabled="searchStore.flightResults.currentPage === 1"
                            variant="outline"
                            size="sm">
                            Anterior
                        </AppButton>
                        <span class="text-sm font-semibold text-content-subtle">
                            Página {{ searchStore.flightResults.currentPage }} de {{ searchStore.flightResults.totalPagesDeparture }}
                        </span>
                        <AppButton 
                            @click="changePage(searchStore.flightResults.currentPage + 1)"
                            :disabled="searchStore.flightResults.currentPage === searchStore.flightResults.totalPagesDeparture"
                            variant="outline"
                            size="sm">
                            Siguiente
                        </AppButton>
                    </div>
                </div>

                <div v-else-if="searchStore.searchParams.IsRoundTrip">

                    <div class="bg-surface-muted border border-border-light rounded-xl p-6 shadow-sm flex flex-col md:flex-row items-center justify-between gap-6 mb-8">
                        
                        <div class="flex-1 w-full md:w-auto text-center md:text-left">
                            <p class="text-sm text-content-subtle font-medium">Su viaje a:</p>
                            <h3 class="text-2xl font-bold text-heading mt-1">{{ selectedDepartureFlight.flight.destination }}</h3>
                        </div>
                        
                        <div class="flex-2 flex flex-col items-center w-full md:w-auto">
                            <span class="text-sm font-medium text-content-subtle mb-1">
                                {{ selectedDepartureFlight.flight.stops === 0 ? 'Directo' : selectedDepartureFlight.flight.stops + ' escala(s)' }}
                            </span>
                            <div class="flex items-center gap-4 text-xl font-bold text-heading w-full justify-center">
                                <span>{{ selectedDepartureFlight.flight.departureTimeText }}</span>
                                <div class="flex flex-col items-center w-24 md:w-40 relative">
                                    <div class="absolute top-1/2 w-full border-t-[1.5px] border-dashed border-content-muted -translate-y-1/2"></div>
                                    <span class="text-xs text-content-muted font-medium bg-surface-muted px-2 relative z-10">
                                        {{ selectedDepartureFlight.flight.totalDurationText }}
                                    </span>
                                </div>
                                <span>{{ selectedDepartureFlight.flight.arrivalTimeText }}</span>
                            </div>
                            <div class="flex justify-between w-full md:w-64 text-sm text-content-subtle mt-1 font-bold">
                                <span>{{ selectedDepartureFlight.flight.origin }}</span>
                                <span>{{ selectedDepartureFlight.flight.destination }}</span>
                            </div>
                        </div>

                        <div class="flex-1 flex flex-col items-center md:items-end text-center md:text-right w-full md:w-auto border-t md:border-t-0 md:border-l border-border-light pt-4 md:pt-0 md:pl-6">
                            <p class="text-base font-semibold text-content">{{ searchStore.searchParams.Date }}</p>
                            <p class="text-sm text-content-subtle mt-1">Clase: <span class="font-bold text-content">{{ selectedDepartureFlight.travelClass }}</span></p>
                            <AppButton @click="clearDepartureSelection" variant="link" class="mt-3 text-sm font-bold">
                                Cambiar vuelo
                            </AppButton>
                        </div>
                    </div>

                    <h3 class="text-xl font-bold text-content mb-4 border-b border-border-light pb-2">
                        Selecciona tu vuelo de regreso ({{ searchStore.searchParams.Destination }} a {{ searchStore.searchParams.Origin }})
                    </h3>
                    
                    <div v-if="searchStore.flightResults.returnFlights?.length === 0" class="text-center py-12 text-content-muted font-medium">
                        No se encontraron vuelos de regreso para esta fecha y ruta.
                    </div>

                    <FlightCard 
                        v-for="flight in searchStore.flightResults.returnFlights" 
                        :key="'ret-'+flight.pathIds" 
                        :flight="flight"
                        @selectFlight="handleSelectReturn" 
                    />

                    <div v-if="searchStore.flightResults.totalPagesReturn > 1" class="flex justify-center items-center gap-4 mt-8">
                        <AppButton 
                            @click="changePage(searchStore.flightResults.currentPage - 1)"
                            :disabled="searchStore.flightResults.currentPage === 1"
                            variant="outline"
                            size="sm">
                            Anterior
                        </AppButton>
                        <span class="text-sm font-semibold text-content-subtle">
                            Página {{ searchStore.flightResults.currentPage }} de {{ searchStore.flightResults.totalPagesReturn }}
                        </span>
                        <AppButton 
                            @click="changePage(searchStore.flightResults.currentPage + 1)"
                            :disabled="searchStore.flightResults.currentPage === searchStore.flightResults.totalPagesReturn"
                            variant="outline"
                            size="sm">
                            Siguiente
                        </AppButton>
                    </div>
                </div>
            </div>
            
            <ErrorModal 
                v-model="showUnavailableModal" 
                title="Vuelo no disponible" 
                :message="unavailableMessage" 
                @close="onUnavailableModalClose" 
            />
        </main>
    </div>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import PublicNavBar from '../components/PublicNavBar.vue';
import FlightCard from '../components/FlightCard.vue';
import AppButton from '../../../shared/AppButton.vue';
import ErrorModal from '../../../shared/ErrorModal.vue';
import { useFlightSearchStore } from '../store/flightSearchStore';

const route = useRoute();
const router = useRouter();
const searchStore = useFlightSearchStore();

const selectedDepartureFlight = ref(null);
const showUnavailableModal = ref(false);
const unavailableMessage = ref('');

const onUnavailableModalClose = async () => {
    showUnavailableModal.value = false;
    await searchStore.performSearch(searchStore.searchParams);
};

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
        PageSize: 2
    };
    
    searchStore.performSearch(params);
});

const handleSelectDeparture = async (selection) => {
    const result = await searchStore.verifyFlightAvailability(selection.flight, searchStore.searchParams.Seats);
    
    const flightRouteSegments = await searchStore.getFlightRouteData(selection.flight);

    if (!result.isAvailable && !result.isError) {
        unavailableMessage.value = "Ya no hay espacios suficientes disponibles para el vuelo de ida seleccionado.";
        showUnavailableModal.value = true;
        return;
    }
    if (result.isError) return;

    if (!searchStore.searchParams.IsRoundTrip) {
        router.push({
            name: 'buyTicket',
            query: {
                flightData: JSON.stringify({ flight: selection.flight, flightClass: selection.travelClass }),
                seats: searchStore.searchParams.Seats,
                flightRouteData: JSON.stringify({ flightRouteSegments })
            }
        });
        return;
    }
    
    selectedDepartureFlight.value = selection;
    changePage(1);
};

const clearDepartureSelection = () => {
    selectedDepartureFlight.value = null;
    changePage(1);
};

const handleSelectReturn = async (selection) => {
    const result = await searchStore.verifyFlightAvailability(selection.flight, searchStore.searchParams.Seats);
    
    if (!result.isAvailable && !result.isError) {
        unavailableMessage.value = "Ya no hay espacios suficientes disponibles para el vuelo de regreso seleccionado.";
        showUnavailableModal.value = true;
        return;
    }
    if (result.isError) return;

    router.push({
        name: 'buyTicket',
        query: {
            flightData: JSON.stringify({
                flight: selectedDepartureFlight.value.flight,
                flightClass: selectedDepartureFlight.value.travelClass
            }),
            seats: searchStore.searchParams.Seats,
            roundTrip: 'true',
            returnFlight: JSON.stringify(selection.flight),
            flightRouteId: (() => {
                const value = selectedDepartureFlight.value?.flight?.segments?.[0]?.flightId
                    ?? selectedDepartureFlight.value?.flight?.pathIds?.split(',')?.[0];
                return value ? String(value) : undefined;
            })(),

        }
    });
};

const changePage = (page) => {
    searchStore.changePage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
};
</script>