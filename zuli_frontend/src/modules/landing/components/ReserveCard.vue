<template>
    <div class="w-full max-w-5xl overflow-visible rounded-2xl bg-white shadow-[0_12px_30px_rgba(0,0,0,0.12)] relative z-10">
        <ul class="m-0 flex w-full list-none bg-primary p-0 rounded-t-2xl overflow-hidden">
            <li class="flex-1">
                <button type="button" class="flex w-full items-center justify-center px-3 py-4 text-center text-sm font-semibold text-white transition"
                    :class="activeTab === 'oneWay' ? 'bg-black/25' : 'hover:bg-white/10'" @click="activeTab = 'oneWay'">
                    Viaje solo ida
                </button>
            </li>
            <li class="flex-1">
                <button type="button" class="flex w-full items-center justify-center px-3 py-4 text-center text-sm font-semibold text-white transition"
                    :class="activeTab === 'roundTrip' ? 'bg-black/25' : 'hover:bg-white/10'" @click="showMaintenanceModal = true">
                    Viaje ida y vuelta
                </button>
            </li>
        </ul>

        <div class="flex flex-wrap items-center gap-6 px-8 pt-6 text-sm text-content-subtle">
            <label class="inline-flex items-center gap-2 cursor-pointer">
                <input v-model="selectedFlightOption" type="radio" value="direct" class="accent-primary w-4 h-4" />
                Vuelo directo
            </label>
            <label class="inline-flex items-center gap-2 cursor-pointer">
                <input v-model="selectedFlightOption" type="radio" value="layover" class="accent-primary w-4 h-4" />
                Vuelo con escalas
            </label>
        </div>

        <div class="px-8 py-6">
            <div class="grid gap-4 lg:grid-cols-[minmax(0,2fr)_minmax(0,2fr)_10rem_7rem_minmax(0,1.8fr)] lg:items-end">
                
                <div class="relative flex flex-col gap-1 z-50">
                    <label class="text-xs font-semibold text-content-subtle">Desde</label>
                    <input v-model="fromCitySearch" type="text" placeholder="Ciudad o Aeropuerto..." autocomplete="off"
                        class="rounded-md border bg-surface-input px-3 py-2 text-sm text-content focus:outline-none focus:border-gold transition"
                        :class="errors.fromCity ? 'border-error' : 'border-border-light'"
                        @input="handleAirportInput('origin')" @blur="clearSuggestions('origin')" />
                    
                    <div v-if="originSuggestions.length" class="absolute left-0 right-0 top-full mt-1 bg-white border border-border-light rounded-md shadow-lg max-h-48 overflow-y-auto">
                        <button v-for="sug in originSuggestions" :key="sug.airportCode" type="button"
                            class="w-full text-left px-3 py-2 text-sm hover:bg-body transition"
                            @mousedown.prevent="selectAirport('origin', sug)">
                            <span class="font-bold text-heading">{{ sug.airportCode }}</span> - <span class="text-content">{{ sug.displayName }}</span>
                        </button>
                    </div>
                </div>

                <div class="relative flex flex-col gap-1 z-40">
                    <label class="text-xs font-semibold text-content-subtle">Hacia</label>
                    <input v-model="toCitySearch" type="text" placeholder="Ciudad o Aeropuerto..." autocomplete="off"
                        class="rounded-md border bg-surface-input px-3 py-2 text-sm text-content focus:outline-none focus:border-gold transition"
                        :class="errors.toCity ? 'border-error' : 'border-border-light'"
                        @input="handleAirportInput('destination')" @blur="clearSuggestions('destination')" />
                    
                    <div v-if="destinationSuggestions.length" class="absolute left-0 right-0 top-full mt-1 bg-white border border-border-light rounded-md shadow-lg max-h-48 overflow-y-auto">
                        <button v-for="sug in destinationSuggestions" :key="sug.airportCode" type="button"
                            class="w-full text-left px-3 py-2 text-sm hover:bg-body transition"
                            @mousedown.prevent="selectAirport('destination', sug)">
                            <span class="font-bold text-heading">{{ sug.airportCode }}</span> - <span class="text-content">{{ sug.displayName }}</span>
                        </button>
                    </div>
                </div>

                <div class="flex min-w-0 flex-col gap-1">
                    <label class="text-xs font-semibold text-content-subtle">Salida</label>
                    <input v-model="departureDate" type="date"
                        class="rounded-md border bg-surface-input px-3 py-2 text-sm text-content focus:outline-none focus:border-gold transition"
                        :class="errors.departureDate ? 'border-error' : 'border-border-light'" />
                </div>

                <div class="flex min-w-0 flex-col gap-1">
                    <label class="text-xs font-semibold text-content-subtle">Asientos</label>
                    <input v-model="seatsCount" type="number" min="1" max="10"
                        class="rounded-md border border-border-light bg-surface-input px-3 py-2 text-center text-sm text-content focus:outline-none focus:border-gold transition" />
                </div>

                <div class="flex min-w-0 flex-col gap-1">
                    <label class="text-xs font-semibold text-content-subtle">Clase</label>
                    <select v-model="travelClass" class="rounded-md border border-border-light bg-surface-input px-3 py-2 text-sm text-content focus:outline-none focus:border-gold transition">
                        <option value="Turista">Turista</option>
                        <option value="Primera Clase">Primera Clase</option>
                    </select>
                </div>
            </div>

            <div class="mt-4 grid gap-4 lg:grid-cols-[minmax(0,2fr)_minmax(0,2fr)_10rem_7rem_minmax(0,1.8fr)] lg:items-end">
                <div class="flex min-w-0 flex-col gap-1 lg:col-start-3 transition-all duration-300"
                     :class="activeTab === 'roundTrip' ? 'opacity-100 visible pointer-events-auto' : 'opacity-0 invisible pointer-events-none'">
                    <label class="text-xs font-semibold text-content-subtle">Regreso</label>
                    <input v-model="returnDate" type="date"
                        class="rounded-md border bg-surface-input px-3 py-2 text-sm text-content focus:outline-none focus:border-gold transition"
                        :class="errors.returnDate ? 'border-error' : 'border-border-light'" />
                </div>
            </div>

            <div class="mt-8 flex justify-end">
                <AppButton @click="validateAndSearch" variant="primary" size="lg">
                    Buscar vuelos
                </AppButton>
            </div>
        </div>
        <ErrorModal 
            v-model="showMaintenanceModal" 
            title="En Mantenimiento" 
            message="El módulo de Ida y Vuelta se encuentra actualmente en mantenimiento. Por favor, intente más tarde." 
            buttonText="Entendido"
        />
    </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { searchAirportSuggestions } from '../service/flightSearchService';
import AppButton from '../../../shared/AppButton.vue';
import ErrorModal from '../../../shared/ErrorModal.vue';

const router = useRouter();
const showMaintenanceModal = ref(false);

const activeTab = ref('oneWay');
const selectedFlightOption = ref('layover');

const originCode = ref('');
const destinationCode = ref('');

const fromCitySearch = ref('');
const toCitySearch = ref('');

const seatsCount = ref(1);
const travelClass = ref('Turista');
const departureDate = ref('');
const returnDate = ref('');

const originSuggestions = ref([]);
const destinationSuggestions = ref([]);

let originTimer = null;
let destTimer = null;

const errors = reactive({
    fromCity: false,
    toCity: false,
    departureDate: false,
    returnDate: false
});

const handleAirportInput = (field) => {
    const term = field === 'origin' ? fromCitySearch.value : toCitySearch.value;
    
    if (field === 'origin') {
        originCode.value = ''; 
        clearTimeout(originTimer);
        if (term.length < 2) return clearSuggestions('origin');
        originTimer = setTimeout(async () => {
            originSuggestions.value = await searchAirportSuggestions(term);
        }, 500);
    } else {
        destinationCode.value = '';
        clearTimeout(destTimer);
        if (term.length < 2) return clearSuggestions('destination');
        destTimer = setTimeout(async () => {
            destinationSuggestions.value = await searchAirportSuggestions(term);
        }, 500);
    }
};

const selectAirport = (field, suggestion) => {
    if (field === 'origin') {
        originCode.value = suggestion.airportCode;
        fromCitySearch.value = `${suggestion.airportCode} - ${suggestion.displayName.split(' - ')[1]}`;
        originSuggestions.value = [];
    } else {
        destinationCode.value = suggestion.airportCode;
        toCitySearch.value = `${suggestion.airportCode} - ${suggestion.displayName.split(' - ')[1]}`;
        destinationSuggestions.value = [];
    }
};

const clearSuggestions = (field) => {
    setTimeout(() => {
        if (field === 'origin') originSuggestions.value = [];
        else destinationSuggestions.value = [];
    }, 200);
};

const validateAndSearch = () => {
    const iataRegex = /^[A-Za-z]{3}$/;

    if (!originCode.value && iataRegex.test(fromCitySearch.value.trim())) {
        originCode.value = fromCitySearch.value.trim().toUpperCase();
    }
    
    if (!destinationCode.value && iataRegex.test(toCitySearch.value.trim())) {
        destinationCode.value = toCitySearch.value.trim().toUpperCase();
    }

    errors.fromCity = !originCode.value;
    errors.toCity = !destinationCode.value;
    errors.departureDate = !departureDate.value;
    errors.returnDate = activeTab.value === 'roundTrip' && !returnDate.value;

    if (errors.fromCity || errors.toCity || errors.departureDate || errors.returnDate) {
        return;
    }

    router.push({
        name: 'buscarVuelos',
        query: {
            origin: originCode.value,
            destination: destinationCode.value,
            date: departureDate.value,
            returnDate: activeTab.value === 'roundTrip' ? returnDate.value : undefined,
            seats: seatsCount.value,
            roundTrip: activeTab.value === 'roundTrip',
            directOnly: selectedFlightOption.value === 'direct',
            flightClass: travelClass.value
        }
    });
};
</script>