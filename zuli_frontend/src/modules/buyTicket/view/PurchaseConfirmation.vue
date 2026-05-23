<template>
    <div class="flex flex-col min-h-screen bg-body">
        <PublicNavBar />

        <main class="flex-1 w-full max-w-3xl mx-auto px-4 py-12">
            <div v-if="store.purchaseResult" class="bg-white rounded-2xl shadow-lg border border-gray-200 overflow-hidden">
                <div class="bg-green-600 px-6 py-8 text-center">
                    <div class="w-16 h-16 bg-white rounded-full flex items-center justify-center mx-auto mb-4">
                        <svg class="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                        </svg>
                    </div>
                    <h1 class="text-2xl font-bold text-white">Compra Exitosa</h1>
                    <p class="text-green-100 mt-1">Tu reservación ha sido confirmada</p>
                </div>

                <div class="p-8 space-y-6">
                    <div class="bg-gray-50 rounded-xl p-6 text-center border border-gray-200">
                        <p class="text-sm font-semibold text-gray-500 mb-1">Código de Confirmación</p>
                        <p class="text-3xl font-bold text-primary tracking-widest">{{ store.purchaseResult.confirmationCode }}</p>
                    </div>

                    <div class="border-t border-gray-200 pt-6">
                        <h3 class="font-bold text-gray-800 mb-4">Detalles de la Compra</h3>
                        <div class="space-y-3">
                            <div class="flex justify-between text-sm">
                                <span class="text-gray-500">Vuelo</span>
                                <span class="font-semibold text-gray-800">
                                    {{ store.selectedFlight?.origin }} → {{ store.selectedFlight?.destination }}
                                </span>
                            </div>
                            <div class="flex justify-between text-sm">
                                <span class="text-gray-500">Fecha</span>
                                <span class="font-semibold text-gray-800">{{ store.selectedFlight?.departureTimeText }}</span>
                            </div>
                            <div class="flex justify-between text-sm">
                                <span class="text-gray-500">Clase</span>
                                <span class="font-semibold text-gray-800">{{ store.selectedClass }}</span>
                            </div>
                            <div class="flex justify-between text-sm">
                                <span class="text-gray-500">Pasajeros</span>
                                <span class="font-semibold text-gray-800">{{ store.passengers.length }}</span>
                            </div>
                            <div class="flex justify-between text-lg font-bold border-t border-gray-200 pt-3">
                                <span class="text-gray-800">Total Pagado</span>
                                <span class="text-primary">${{ store.totalPrice.toFixed(2) }}</span>
                            </div>
                        </div>
                    </div>

                    <div class="border-t border-gray-200 pt-6">
                        <h3 class="font-bold text-gray-800 mb-4">Pasajeros</h3>
                        <div class="space-y-2">
                            <div v-for="(p, i) in store.passengers" :key="i"
                                class="flex items-center gap-3 bg-gray-50 rounded-lg p-3 border border-gray-200"
                            >
                                <span class="bg-primary text-white w-7 h-7 rounded-full flex items-center justify-center text-sm font-bold shrink-0">
                                    {{ i + 1 }}
                                </span>
                                <div>
                                    <p class="font-semibold text-gray-800">{{ p.firstName }} {{ p.lastName }}</p>
                                    <p class="text-xs text-gray-500">{{ p.documentType }}: {{ p.documentNumber }}</p>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="flex justify-center gap-4 pt-4">
                        <router-link to="/"
                            class="rounded-lg bg-primary px-8 py-3 text-base font-bold text-white transition hover:bg-select shadow-md"
                        >
                            Nueva Búsqueda
                        </router-link>
                        <button @click="windowPrint"
                            class="rounded-lg border-2 border-primary px-8 py-3 text-base font-bold text-primary transition hover:bg-primary hover:text-white"
                        >
                            Imprimir
                        </button>
                    </div>
                </div>
            </div>

            <div v-else class="text-center py-20">
                <p class="text-gray-500 font-medium">No hay información de compra disponible.</p>
                <router-link to="/" class="inline-block mt-4 rounded-lg bg-primary px-6 py-2 text-sm font-bold text-white transition hover:bg-select">
                    Buscar Vuelos
                </router-link>
            </div>
        </main>

        <PublicBottomBar />
    </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
import PublicNavBar from '../../landing/components/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';
import { useTicketStore } from '../store/ticketStore';

const route = useRoute();
const store = useTicketStore();

const windowPrint = () => window.print();

onMounted(() => {
    if (route.params.code && !store.purchaseResult) {
        console.log('Loading confirmation for code:', route.params.code);
    }
});
</script>
