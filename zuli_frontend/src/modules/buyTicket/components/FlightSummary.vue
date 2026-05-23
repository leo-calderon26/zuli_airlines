<script setup>
import AppButton from '../../../shared/AppButton.vue'

defineProps({
    flight: { type: Object, required: true },
    flightClass: { type: String, default: 'Turista' },
    passengerCount: { type: Number, default: 1 },
    stops: { type: Number, default: 0 },
    total: { type: [Number, String], default: 0 },
    loading: { type: Boolean, default: false },
    valid: { type: Boolean, default: false }
});

defineEmits(['purchase']);
</script>
<template>
    <div class="bg-sumary text-white p-10 sticky top-10">
        <h3 class="text-lg font-bold mb-8">Resumen del Vuelo</h3>

        <div class="flex items-center justify-between mb-6">
            <div class="text-center">
                <p class="text-2xl font-bold">{{ flight.departureTimeText }}</p>
                <p class="text-sm opacity-80">{{ flight.origin }}</p>
            </div>
            <div class="flex-1 px-4 flex items-center justify-center">
                <div class="w-full h-px bg-white/30"></div>
            </div>
            <div class="text-center">
                <p class="text-2xl font-bold">{{ flight.arrivalTimeText }}</p>
                <p class="text-sm opacity-80">{{ flight.destination }}</p>
            </div>
        </div>

        <div class="border-t border-white/20 pt-6 space-y-4">
            <div class="flex justify-between text-sm">
                <span class="opacity-70">Duración</span>
                <span class="font-semibold">{{ flight.totalDurationText }}</span>
            </div>
            <div class="flex justify-between text-sm">
                <span class="opacity-70">Clase</span>
                <span class="font-semibold">{{ flightClass }}</span>
            </div>
            <div class="flex justify-between text-sm">
                <span class="opacity-70">Pasajeros</span>
                <span class="font-semibold">{{ passengerCount }}</span>
            </div>
            <div v-if="stops !== undefined" class="flex justify-between text-sm">
                <span class="opacity-70">Escalas</span>
                <span class="font-semibold">{{ stops === 0 ? 'Directo' : stops + ' escala(s)' }}</span>
            </div>
        </div>

        <div class="border-t border-white/20 pt-6 mt-6">
            <div class="flex justify-between items-end">
                <div>
                    <p class="text-sm opacity-70">Total a Pagar</p>
                    <p class="text-5xl font-bold mt-2">${{ total }}</p>
                </div>
            </div>
        </div>

        <AppButton
            variant="primary"
            size="lg"
            :loading="loading"
            :disabled="loading || !valid"
            class="!w-full !bg-gold !text-black !rounded-none !mt-8"
            @click="$emit('purchase')"
        >{{ loading ? 'Procesando...' : 'Pagar Ahora' }}</AppButton>
    </div>
</template>
