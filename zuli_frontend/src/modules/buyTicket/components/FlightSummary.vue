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

    function computePassengerBaggage(flightOption, defaultCheckedPrice, defaultCarryOnPrice, defaultMult) {
        const segments = flightOption?.segments || [];
        return props.passengers.map(passenger => {
            let checkedTotal = 0;
            let carryOnTotal = 0;
            const checkedBagsCount = passenger.checkedBaggage || 0;
            const carryOnsCount = passenger.carryOn || 0;
            
            const segmentsBreakdown = [];

            if (segments.length === 0) {
                let segCheckedTotal = 0;
                const checkedBags = [];
                for (let i = 1; i <= checkedBagsCount; i++) {
                    const price = (defaultCheckedPrice || 0) * Math.pow(defaultMult || 1, i);
                    segCheckedTotal += price;
                    checkedBags.push({ number: i, price });
                }
                const segCarryOnTotal = carryOnsCount * (defaultCarryOnPrice || 0);
                
                checkedTotal += segCheckedTotal;
                carryOnTotal += segCarryOnTotal;

                segmentsBreakdown.push({
                    label: 'Vuelo directo',
                    checkedBags,
                    carryOnTotal: segCarryOnTotal,
                    segmentTotal: segCheckedTotal + segCarryOnTotal
                });
            } else {
                for (const seg of segments) {
                    let segCheckedTotal = 0;
                    const checkedBags = [];
                    const cp = seg.checkedPrice !== undefined && seg.checkedPrice !== null ? seg.checkedPrice : (defaultCheckedPrice || 0);
                    const cop = seg.carryOnPrice !== undefined && seg.carryOnPrice !== null ? seg.carryOnPrice : (defaultCarryOnPrice || 0);
                    const mult = seg.checkedBagMultiplier !== undefined && seg.checkedBagMultiplier !== null ? seg.checkedBagMultiplier : (defaultMult || 1);
                    
                    for (let i = 1; i <= checkedBagsCount; i++) {
                        const price = cp * Math.pow(mult, i);
                        segCheckedTotal += price;
                        checkedBags.push({ number: i, price });
                    }
                    const segCarryOnTotal = carryOnsCount * cop;
                    
                    checkedTotal += segCheckedTotal;
                    carryOnTotal += segCarryOnTotal;

                    segmentsBreakdown.push({
                        label: `${seg.origin} ➝ ${seg.destination}`,
                        checkedBags,
                        carryOnTotal: segCarryOnTotal,
                        segmentTotal: segCheckedTotal + segCarryOnTotal
                    });
                }
            }

            return { checkedTotal, carryOnTotal, segmentsBreakdown };
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

            <div v-if="passengers.length > 0" class="space-y-3">
                <div v-for="(passenger, pIdx) in passengers" :key="pIdx" class="text-sm">
                    <p class="opacity-90 font-semibold text-xs mb-1">{{ passenger.firstName || 'Pasajero' }} {{ passenger.firstLastName || '' }}</p>
                    
                    <template v-if="passenger.checkedBaggage > 0 || passenger.carryOn > 0">
                        <div v-for="(seg, sIdx) in outboundPassengerBaggage[pIdx].segmentsBreakdown" :key="sIdx" class="ml-2 mb-2 border-l border-white/20 pl-3">
                            <p class="text-[10px] font-bold text-gold uppercase mb-1">{{ seg.label }}</p>
                            <div v-for="bag in seg.checkedBags" :key="bag.number" class="flex justify-between">
                                <span class="opacity-70 text-[11px]">↳ Maleta #{{ bag.number }}</span>
                                <span class="font-semibold text-[11px]">${{ bag.price.toFixed(2) }}</span>
                            </div>
                            <div v-if="seg.carryOnTotal > 0" class="flex justify-between">
                                <span class="opacity-70 text-[11px]">↳ Equipaje de mano (×{{ passenger.carryOn }})</span>
                                <span class="font-semibold text-[11px]">${{ seg.carryOnTotal.toFixed(2) }}</span>
                            </div>
                            <div class="flex justify-between mt-1 border-t border-white/10 pt-1">
                                <span class="opacity-70 text-[10px]">Subtotal segmento</span>
                                <span class="font-semibold text-[10px]">${{ seg.segmentTotal.toFixed(2) }}</span>
                            </div>
                        </div>
                    </template>
                    <div v-else class="opacity-70 text-xs pl-2">Sin equipaje adicional</div>
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

                <div v-if="passengers.length > 0" class="space-y-3">
                    <div v-for="(passenger, pIdx) in passengers" :key="pIdx" class="text-sm">
                        <p class="opacity-90 font-semibold text-xs mb-1">{{ passenger.firstName || 'Pasajero' }} {{ passenger.firstLastName || '' }}</p>
                        
                        <template v-if="passenger.checkedBaggage > 0 || passenger.carryOn > 0">
                            <div v-for="(seg, sIdx) in returnPassengerBaggage[pIdx].segmentsBreakdown" :key="sIdx" class="ml-2 mb-2 border-l border-white/20 pl-3">
                                <p class="text-[10px] font-bold text-gold uppercase mb-1">{{ seg.label }}</p>
                                <div v-for="bag in seg.checkedBags" :key="bag.number" class="flex justify-between">
                                    <span class="opacity-70 text-[11px]">↳ Maleta #{{ bag.number }}</span>
                                    <span class="font-semibold text-[11px]">${{ bag.price.toFixed(2) }}</span>
                                </div>
                                <div v-if="seg.carryOnTotal > 0" class="flex justify-between">
                                    <span class="opacity-70 text-[11px]">↳ Equipaje de mano (×{{ passenger.carryOn }})</span>
                                    <span class="font-semibold text-[11px]">${{ seg.carryOnTotal.toFixed(2) }}</span>
                                </div>
                                <div class="flex justify-between mt-1 border-t border-white/10 pt-1">
                                    <span class="opacity-70 text-[10px]">Subtotal segmento</span>
                                    <span class="font-semibold text-[10px]">${{ seg.segmentTotal.toFixed(2) }}</span>
                                </div>
                            </div>
                        </template>
                        <div v-else class="opacity-70 text-xs pl-2">Sin equipaje adicional</div>
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
                        Boletos ida (×{{ passengerCount }})
                    </span>
                    <span class="font-semibold">${{ outboundFlightTotal.toFixed(2) }}</span>
                </div>
                <div v-if="outboundBaggageTotal > 0" class="flex justify-between text-sm pl-2">
                    <span class="opacity-70 text-xs">↳ Total equipaje</span>
                    <span class="font-semibold text-xs">${{ outboundBaggageTotal.toFixed(2) }}</span>
                </div>
            </div>

            <div v-if="isRoundTrip && returnFlight" class="space-y-1 border-t border-white/10 pt-2">
                <div class="flex justify-between text-sm">
                    <span class="opacity-70">
                        Boletos vuelta (×{{ passengerCount }})
                    </span>
                    <span class="font-semibold">${{ returnFlightTotal.toFixed(2) }}</span>
                </div>
                <div v-if="returnBaggageTotal > 0" class="flex justify-between text-sm pl-2">
                    <span class="opacity-70 text-xs">↳ Total equipaje</span>
                    <span class="font-semibold text-xs">${{ returnBaggageTotal.toFixed(2) }}</span>
                </div>
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