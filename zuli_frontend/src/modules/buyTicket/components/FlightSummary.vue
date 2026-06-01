<script setup>
import { computed } from 'vue'
import AppButton from '../../../shared/AppButton.vue'

const props = defineProps({
    flight: { type: Object, required: true },
    flightClass: { type: String, default: 'Turista' },
    passengerCount: { type: Number, default: 1 },
    stops: { type: Number, default: 0 },
    total: { type: [Number, String], default: 0 },
    loading: { type: Boolean, default: false },
    valid: { type: Boolean, default: false },
    returnFlight: { type: Object, default: null },
    isRoundTrip: { type: Boolean, default: false },
    checkedPrice: { type: Number, default: 0 },
    carryOnPrice: { type: Number, default: 0 },
    checkedBagMultiplier: { type: Number, default: 1 },
    returnCheckedPrice: { type: Number, default: 0 },
    returnCarryOnPrice: { type: Number, default: 0 },
    returnCheckedBagMultiplier: { type: Number, default: 1 },
    passengers: { type: Array, default: () => [] },
    outboundFlightTotal: { type: Number, default: 0 },
    returnFlightTotal: { type: Number, default: 0 },
    baggageTotal: { type: Number, default: 0 }
});

const emit = defineEmits(['purchase']);

const totalCheckedBags = computed(() =>
    props.passengers.reduce((s, p) => s + (p.checkedBaggage || 0), 0)
);

const totalCarryOns = computed(() =>
    props.passengers.reduce((s, p) => s + (p.carryOn || 0), 0)
);

const checkedBagPrices = computed(() => {
    if (props.checkedBagMultiplier <= 0 || props.checkedPrice <= 0) return [];
    const prices = [];
    for (let i = 1; i <= 10; i++) {
        prices.push((props.checkedPrice * Math.pow(props.checkedBagMultiplier, i)).toFixed(2));
    }
    return prices;
});

const returnCheckedBagPrices = computed(() => {
    if (props.returnCheckedBagMultiplier <= 0 || props.returnCheckedPrice <= 0) return [];
    const prices = [];
    for (let i = 1; i <= 10; i++) {
        prices.push((props.returnCheckedPrice * Math.pow(props.returnCheckedBagMultiplier, i)).toFixed(2));
    }
    return prices;
});

function handlePurchase() {
    emit('purchase');
}
</script>
<template>
    <div class="bg-sumary text-white p-10 sticky top-10">
        <h3 class="text-lg font-bold mb-6">Resumen del Vuelo</h3>

        <div class="flex items-center justify-between mb-4">
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

        <div class="border-t border-white/20 pt-4 space-y-3">
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

        <div class="border-t border-white/20 pt-4 mt-4 space-y-2">
            <p class="text-xs font-semibold opacity-60 uppercase tracking-wide">Equipaje (ida)</p>
            <div v-if="totalCheckedBags > 0" class="flex justify-between text-sm">
                <span class="opacity-70">Maletas documentadas</span>
                <span class="font-semibold">
                    <template v-for="(price, idx) in checkedBagPrices.slice(0, totalCheckedBags)" :key="idx">
                        {{ idx + 1 }}ra: ${{ price }}{{ idx < totalCheckedBags - 1 ? ', ' : '' }}
                    </template>
                </span>
            </div>
            <div class="flex justify-between text-sm">
                <span class="opacity-70">Equipaje de mano</span>
                <span class="font-semibold">${{ carryOnPrice }} c/u</span>
            </div>
            <div v-if="totalCheckedBags > 0" class="flex justify-between text-sm">
                <span class="opacity-70">Maletas documentadas</span>
                <span class="font-semibold">{{ totalCheckedBags }} uds.</span>
            </div>
            <div v-if="totalCarryOns > 0" class="flex justify-between text-sm">
                <span class="opacity-70">Equipaje de mano</span>
                <span class="font-semibold">{{ totalCarryOns }} uds.</span>
            </div>
        </div>

        <div v-if="isRoundTrip && returnFlight" class="border-t border-white/20 pt-5 mt-5">
            <p class="text-xs font-semibold opacity-60 uppercase tracking-wide mb-3">Vuelo de Regreso</p>
            <div class="flex items-center justify-between mb-3">
                <div class="text-center">
                    <p class="text-xl font-bold">{{ returnFlight.departureTimeText }}</p>
                    <p class="text-sm opacity-80">{{ returnFlight.origin }}</p>
                </div>
                <div class="flex-1 px-4 flex items-center justify-center">
                    <div class="w-full h-px bg-white/30"></div>
                </div>
                <div class="text-center">
                    <p class="text-xl font-bold">{{ returnFlight.arrivalTimeText }}</p>
                    <p class="text-sm opacity-80">{{ returnFlight.destination }}</p>
                </div>
            </div>
            <div class="space-y-2">
                <div class="flex justify-between text-sm">
                    <span class="opacity-70">Duración</span>
                    <span class="font-semibold">{{ returnFlight.totalDurationText }}</span>
                </div>
                <div v-if="returnFlight.stops !== undefined" class="flex justify-between text-sm">
                    <span class="opacity-70">Escalas</span>
                    <span class="font-semibold">{{ returnFlight.stops === 0 ? 'Directo' : returnFlight.stops + ' escala(s)' }}</span>
                </div>
            </div>
            <div class="border-t border-white/20 pt-4 mt-4 space-y-2">
                <p class="text-xs font-semibold opacity-60 uppercase tracking-wide">Equipaje (vuelta)</p>
                <div class="flex justify-between text-sm">
                    <span class="opacity-70">Equipaje de mano</span>
                    <span class="font-semibold">${{ returnCarryOnPrice }} c/u</span>
                </div>
                <div v-if="totalCheckedBags > 0" class="flex justify-between text-sm">
                    <span class="opacity-70">Maletas documentadas</span>
                    <span class="font-semibold">
                        <template v-for="(price, idx) in returnCheckedBagPrices.slice(0, totalCheckedBags)" :key="idx">
                            {{ idx + 1 }}ra: ${{ price }}{{ idx < totalCheckedBags - 1 ? ', ' : '' }}
                        </template>
                    </span>
                </div>
                <div v-if="totalCarryOns > 0" class="flex justify-between text-sm">
                    <span class="opacity-70">Equipaje de mano</span>
                    <span class="font-semibold">{{ totalCarryOns }} uds.</span>
                </div>
            </div>
        </div>

        <div class="border-t border-white/20 pt-5 mt-5 space-y-3">
            <p class="text-xs font-semibold opacity-60 uppercase tracking-wide mb-2">Resumen de Precios</p>
            <div class="flex justify-between text-sm">
                <span class="opacity-70">Vuelo ida ({{ passengerCount }} pasajero{{ passengerCount !== 1 ? 's' : '' }})</span>
                <span class="font-semibold">${{ outboundFlightTotal.toFixed(2) }}</span>
            </div>
            <div v-if="isRoundTrip && returnFlightTotal > 0" class="flex justify-between text-sm">
                <span class="opacity-70">Vuelo vuelta ({{ passengerCount }} pasajero{{ passengerCount !== 1 ? 's' : '' }})</span>
                <span class="font-semibold">${{ returnFlightTotal.toFixed(2) }}</span>
            </div>
            <div v-if="baggageTotal > 0" class="flex justify-between text-sm">
                <span class="opacity-70">Equipaje {{ isRoundTrip ? '(ida y vuelta)' : '(ida)' }}</span>
                <span class="font-semibold">${{ baggageTotal.toFixed(2) }}</span>
            </div>
        </div>

        <div class="border-t border-white/20 pt-5 mt-5">
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
            @click="handlePurchase"
        >{{ loading ? 'Procesando...' : 'Pagar Ahora' }}</AppButton>
    </div>
</template>
