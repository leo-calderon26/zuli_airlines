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
        props.passengers.reduce((sum, passenger) => sum + (passenger.checkedBaggage || 0), 0)
    );

    const totalCarryOns = computed(() =>
        props.passengers.reduce((sum, passenger) => sum + (passenger.carryOn || 0), 0)
    );

    function computePassengerBaggage(flightOption, checkedPrice, carryOnPrice, checkedBagMultiplier) {
        const segments = flightOption?.segments || [];
        return props.passengers.map(passenger => {
            let checkedTotal = 0;
            let carryOnTotal = 0;
            const checkedBags = passenger.checkedBaggage || 0;
            const carryOns = passenger.carryOn || 0;
            const checkedBagPrices = [];

            if (segments.length === 0) {
                const cp = checkedPrice || 0;
                const cop = carryOnPrice || 0;
                const mult = checkedBagMultiplier || 1;
                for (let i = 1; i <= checkedBags; i++) {
                    const price = cp * Math.pow(mult, i);
                    checkedTotal += price;
                    checkedBagPrices.push({ number: i, price });
                }
                carryOnTotal = carryOns * cop;
            } else {
                for (const seg of segments) {
                    const cp = seg.checkedPrice || seg.CheckedPrice || 0;
                    const cop = seg.carryOnPrice || seg.CarryOnPrice || 0;
                    const mult = seg.checkedBagMultiplier || 1;
                    for (let i = 1; i <= checkedBags; i++) {
                        const price = cp * Math.pow(mult, i);
                        checkedTotal += price;
                        checkedBagPrices.push({ number: i, price });
                    }
                    carryOnTotal += carryOns * cop;
                }
            }

            return { checkedTotal, carryOnTotal, checkedBagPrices };
        });
    }

    const outboundPassengerBaggage = computed(() =>
        computePassengerBaggage(props.flight, props.checkedPrice, props.carryOnPrice, props.checkedBagMultiplier)
    );

    const returnPassengerBaggage = computed(() =>
        computePassengerBaggage(props.returnFlight, props.returnCheckedPrice, props.returnCarryOnPrice, props.returnCheckedBagMultiplier)
    );

    const outboundBaggageTotal = computed(() =>
        outboundPassengerBaggage.value.reduce((sum, p) => sum + p.checkedTotal + p.carryOnTotal, 0)
    );

    const returnBaggageTotal = computed(() =>
        returnPassengerBaggage.value.reduce((sum, p) => sum + p.checkedTotal + p.carryOnTotal, 0)
    );

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
                <span class="font-semibold">
                    {{ stops === 0 ? 'Directo' : stops + ' escala(s)' }}
                </span>
            </div>
        </div>

        <div class="border-t border-white/20 pt-4 mt-4 space-y-2">
            <p class="text-xs font-semibold opacity-60 uppercase tracking-wide">
                Equipaje (ida)
            </p>

            <div v-if="passengers.length > 0" class="space-y-2">
                <div v-for="(passenger, pIdx) in passengers" :key="pIdx" class="text-sm">
                    <p class="opacity-90 font-semibold text-xs mb-1">{{ passenger.firstName || 'Pasajero' }} {{ passenger.firstLastName || '' }}</p>
                    <div v-if="passenger.checkedBaggage > 0" class="flex justify-between pl-2">
                        <span class="opacity-70 text-xs">Maletas documentadas (×{{ passenger.checkedBaggage }})</span>
                        <span class="font-semibold text-xs">${{ outboundPassengerBaggage[pIdx].checkedTotal.toFixed(2) }}</span>
                    </div>
                    <div v-if="passenger.carryOn > 0" class="flex justify-between pl-2">
                        <span class="opacity-70 text-xs">Equipaje de mano (×{{ passenger.carryOn }})</span>
                        <span class="font-semibold text-xs">${{ outboundPassengerBaggage[pIdx].carryOnTotal.toFixed(2) }}</span>
                    </div>
                    <div v-if="!passenger.checkedBaggage && !passenger.carryOn" class="opacity-70 text-xs pl-2">Sin equipaje adicional</div>
                </div>
            </div>
            <div v-else class="opacity-70 text-sm">Sin equipaje seleccionado</div>
        </div>

        <div v-if="isRoundTrip && returnFlight" class="border-t border-white/20 pt-5 mt-5">
            <p class="text-xs font-semibold opacity-60 uppercase tracking-wide mb-3">
                Vuelo de Regreso
            </p>

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
                    <span class="font-semibold">
                        {{ returnFlight.stops === 0 ? 'Directo' : returnFlight.stops + ' escala(s)' }}
                    </span>
                </div>
            </div>

            <div class="border-t border-white/20 pt-4 mt-4 space-y-2">
                <p class="text-xs font-semibold opacity-60 uppercase tracking-wide">
                    Equipaje (vuelta)
                </p>

                <div v-if="passengers.length > 0" class="space-y-2">
                    <div v-for="(passenger, pIdx) in passengers" :key="pIdx" class="text-sm">
                        <p class="opacity-90 font-semibold text-xs mb-1">{{ passenger.firstName || 'Pasajero' }} {{ passenger.firstLastName || '' }}</p>
                        <div v-if="passenger.checkedBaggage > 0" class="flex justify-between pl-2">
                            <span class="opacity-70 text-xs">Maletas documentadas (×{{ passenger.checkedBaggage }})</span>
                            <span class="font-semibold text-xs">${{ returnPassengerBaggage[pIdx].checkedTotal.toFixed(2) }}</span>
                        </div>
                        <div v-if="passenger.carryOn > 0" class="flex justify-between pl-2">
                            <span class="opacity-70 text-xs">Equipaje de mano (×{{ passenger.carryOn }})</span>
                            <span class="font-semibold text-xs">${{ returnPassengerBaggage[pIdx].carryOnTotal.toFixed(2) }}</span>
                        </div>
                        <div v-if="!passenger.checkedBaggage && !passenger.carryOn" class="opacity-70 text-xs pl-2">Sin equipaje adicional</div>
                    </div>
                </div>
                <div v-else class="opacity-70 text-sm">Sin equipaje seleccionado</div>
            </div>
        </div>

        <div class="border-t border-white/20 pt-5 mt-5 space-y-3">
            <p class="text-xs font-semibold opacity-60 uppercase tracking-wide mb-2">
                Resumen de Precios
            </p>

            <div class="space-y-1">
                <div class="flex justify-between text-sm">
                    <span class="opacity-70">
                        Vuelo ({{ passengerCount }} pasajero{{ passengerCount !== 1 ? 's' : '' }})
                    </span>
                    <span class="font-semibold">${{ outboundFlightTotal.toFixed(2) }}</span>
                </div>
                <template v-for="(passenger, pIdx) in passengers" :key="pIdx">
                    <template v-if="outboundPassengerBaggage[pIdx].checkedBagPrices.length > 0">
                        <div v-for="bag in outboundPassengerBaggage[pIdx].checkedBagPrices" :key="bag.number" class="flex justify-between text-xs pl-3">
                            <span class="opacity-70">↳ Maleta #{{ bag.number }} - {{ passenger.firstName || 'Pasajero' }} {{ passenger.firstLastName || '' }}</span>
                            <span class="font-semibold">${{ bag.price.toFixed(2) }}</span>
                        </div>
                    </template>
                    <div v-if="passenger.carryOn > 0" class="flex justify-between text-xs pl-3">
                        <span class="opacity-70">↳ Equipaje de mano - {{ passenger.firstName || 'Pasajero' }} {{ passenger.firstLastName || '' }}</span>
                        <span class="font-semibold">${{ outboundPassengerBaggage[pIdx].carryOnTotal.toFixed(2) }}</span>
                    </div>
                </template>
            </div>

            <div v-if="isRoundTrip && returnFlight" class="space-y-1">
                <div class="flex justify-between text-sm">
                    <span class="opacity-70">
                        Vuelo vuelta ({{ passengerCount }} pasajero{{ passengerCount !== 1 ? 's' : '' }})
                    </span>
                    <span class="font-semibold">${{ returnFlightTotal.toFixed(2) }}</span>
                </div>
                <template v-for="(passenger, pIdx) in passengers" :key="pIdx">
                    <template v-if="returnPassengerBaggage[pIdx].checkedBagPrices.length > 0">
                        <div v-for="bag in returnPassengerBaggage[pIdx].checkedBagPrices" :key="bag.number" class="flex justify-between text-xs pl-3">
                            <span class="opacity-70">↳ Maleta #{{ bag.number }} - {{ passenger.firstName || 'Pasajero' }} {{ passenger.firstLastName || '' }}</span>
                            <span class="font-semibold">${{ bag.price.toFixed(2) }}</span>
                        </div>
                    </template>
                    <div v-if="passenger.carryOn > 0" class="flex justify-between text-xs pl-3">
                        <span class="opacity-70">↳ Equipaje de mano - {{ passenger.firstName || 'Pasajero' }} {{ passenger.firstLastName || '' }}</span>
                        <span class="font-semibold">${{ returnPassengerBaggage[pIdx].carryOnTotal.toFixed(2) }}</span>
                    </div>
                </template>
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

        <AppButton variant="primary"
                   size="lg"
                   :loading="loading"
                   :disabled="loading"
                   class="!w-full !bg-gold !text-black !rounded-none !mt-8"
                   @click="handlePurchase">
            {{ loading ? 'Procesando...' : 'Pagar Ahora' }}
        </AppButton>
    </div>
</template>