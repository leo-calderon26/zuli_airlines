<template>
    <div class="bg-white rounded-xl shadow-md border border-gray-200 overflow-hidden">
        <div class="bg-primary px-6 py-4">
            <h3 class="text-lg font-bold text-white">Resumen de Pago</h3>
        </div>
        <div class="p-6 space-y-4">
            <div v-if="flight" class="space-y-2">
                <div class="flex justify-between text-sm">
                    <span class="text-gray-500">Vuelo</span>
                    <span class="font-semibold text-gray-800">{{ flight.origin }} → {{ flight.destination }}</span>
                </div>
                <div class="flex justify-between text-sm">
                    <span class="text-gray-500">Fecha</span>
                    <span class="font-semibold text-gray-800">{{ flight.departureTimeText }}</span>
                </div>
            </div>

            <div class="border-t border-gray-200 pt-4 space-y-2">
                <div class="flex justify-between text-sm">
                    <span class="text-gray-500">Precio por pasajero ({{ flightClass }})</span>
                    <span class="font-semibold text-gray-800">${{ unitPrice }}</span>
                </div>
                <div class="flex justify-between text-sm">
                    <span class="text-gray-500">Pasajeros</span>
                    <span class="font-semibold text-gray-800">{{ passengerCount }}</span>
                </div>
            </div>

            <div class="border-t-2 border-primary pt-4">
                <div class="flex justify-between items-center">
                    <span class="text-lg font-bold text-gray-800">Total a Pagar</span>
                    <span class="text-3xl font-bold text-primary">${{ total }}</span>
                </div>
            </div>

            <button
                @click="$emit('purchase')"
                :disabled="loading || !valid"
                class="w-full mt-4 rounded-lg bg-primary px-8 py-3 text-base font-bold text-white transition hover:bg-select shadow-md active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            >
                <span v-if="loading" class="inline-block h-5 w-5 animate-spin rounded-full border-2 border-white border-t-transparent"></span>
                {{ loading ? 'Procesando...' : 'Confirmar Compra' }}
            </button>
        </div>
    </div>
</template>

<script setup>
defineProps({
    flight: { type: Object, default: null },
    flightClass: { type: String, default: 'Turista' },
    passengerCount: { type: Number, default: 1 },
    unitPrice: { type: [Number, String], default: 0 },
    total: { type: [Number, String], default: 0 },
    loading: { type: Boolean, default: false },
    valid: { type: Boolean, default: false }
});

defineEmits(['purchase']);
</script>