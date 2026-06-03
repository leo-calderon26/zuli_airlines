<script setup>
    import { computed, onMounted } from 'vue';
    import { useRoute, useRouter } from 'vue-router';
    import PublicNavBar from '../../../shared/PublicNavBar.vue';
    import SuccessModal from '../../../shared/SuccessModal.vue';
    import ErrorModal from '../../../shared/ErrorModal.vue';
    import PassengerForm from '../components/PassengerForm.vue';
    import FlightSummary from '../components/FlightSummary.vue';
    import { useTicketStore } from '../store/ticketStore';
    import { useTicketPurchase } from '../composable/useTicketPurchase';

    const route = useRoute();
    const router = useRouter();
    const store = useTicketStore();

    const {
        errors,
        showSuccessModal,
        showErrorModal,
        isFormValid,
        submitPurchase
    } = useTicketPurchase();

    const passengerErrors = computed(() => errors.value.passengers || []);

    const contactErrors = computed(() => ({
        buyerFirstName: errors.value.buyerFirstName,
        buyerFirstLastName: errors.value.buyerFirstLastName,
        buyerSecondLastName: errors.value.buyerSecondLastName,
        buyerBirthDate: errors.value.buyerBirthDate,
        email: errors.value.email,
        phone: errors.value.phone
    }));

    onMounted(() => {
        if (!route.query.flightData) {
            return;
        }

        try {
            const flightData = JSON.parse(route.query.flightData);

            store.setFlight(
                flightData.flight,
                flightData.flightClass || 'Turista'
            );

            if (route.query.flightRouteData) {
                store.setFlightRoutes(JSON.parse(route.query.flightRouteData));
            } else {
                store.setFlightRoutes(flightData.flight);
            }

            store.setPassengers(parseInt(route.query.seats) || 1);

            if (route.query.roundTrip === 'true' && route.query.returnFlight) {
                const returnFlight = JSON.parse(route.query.returnFlight);

                store.isRoundTrip = true;
                store.returnFlight = JSON.parse(route.query.returnFlight);
            }
        } catch (e) {
            console.error('Error parsing flight data', e);
        }
    }
});

    function handleAddPassenger() {
        store.setPassengers(store.passengers.length + 1);
    }

    function handleRemovePassenger(index) {
        const updated = store.passengers.filter((_, i) => i !== index);
        store.passengers = updated;
    }

    const handlePurchase = async () => {
        const result = await submitPurchase();

        const reservationCode = result?.confirmationCode || result?.ConfirmationCode;

        if (!reservationCode) {
            return;
        }
        router.push({
            name: 'purchaseConfirmation',
            params: {
                reservationCode
            }
        });
    };
</script>

<template>
    <div class="flex flex-col min-h-screen bg-[#F7F3F2]">

        <main class="flex-1 w-full max-w-7xl mx-auto grid grid-cols-12 gap-10 py-10 px-4">
            <div v-if="!store.selectedFlight"
                 class="col-span-12 border border-border-soft bg-text-box p-12 text-center">
                <p class="text-font font-medium">
                    No hay un vuelo seleccionado. Por favor, busca y selecciona un vuelo primero.
                </p>

                <router-link to="/"
                             class="inline-block mt-4 bg-sumary text-white px-6 py-3 font-semibold transition hover:brightness-110">
                    Buscar Vuelos
                </router-link>
            </div>

            <template v-else>
                <div class="col-span-8 space-y-6">
                    <router-link to="/"
                                 class="text-2xl font-semibold text-font hover:opacity-80 transition flex items-center gap-1">
                        ← Volver a la búsqueda
                    </router-link>

                    <div v-if="errors._duplicate"
                         class="bg-error-soft border border-error-border text-error px-4 py-3 rounded-lg text-sm font-semibold">
                        Dos o más pasajeros tienen el mismo nombre, fecha de nacimiento y país del pasaporte. Verifica los datos.
                    </div>

                    <PassengerForm :passengers="store.passengers"
                                   :buyer-first-name="store.buyerFirstName"
                                   :buyer-first-last-name="store.buyerFirstLastName"
                                   :buyer-second-last-name="store.buyerSecondLastName"
                                   :buyer-birth-date="store.buyerBirthDate"
                                   :email="store.contactEmail"
                                   :phone="store.contactPhone"
                                   :flight-class="store.selectedClass"
                                   :payment-method="store.paymentMethod"
                                   :checked-price="store.selectedFlight?.checkedPrice || 0"
                                   :carry-on-price="store.selectedFlight?.carryOnPrice || 0"
                                   :max-weight-per-bag="store.selectedFlight?.maxWeightPerBag || 23"
                                   :errors="passengerErrors"
                                   :contactErrors="contactErrors"
                                   @update:passengers="store.passengers = $event"
                                   @update:buyerFirstName="store.buyerFirstName = $event"
                                   @update:buyerFirstLastName="store.buyerFirstLastName = $event"
                                   @update:buyerSecondLastName="store.buyerSecondLastName = $event"
                                   @update:buyerBirthDate="store.buyerBirthDate = $event"
                                   @update:email="store.contactEmail = $event"
                                   @update:phone="store.contactPhone = $event"
                                   @update:flightClass="store.selectedClass = $event"
                                   @update:paymentMethod="store.paymentMethod = $event"
                                   @add-passenger="handleAddPassenger"
                                   @remove-passenger="handleRemovePassenger" />
                </div>

                <div class="col-span-4">
                    <FlightSummary :flight="store.selectedFlight"
                                   :flight-class="store.selectedClass"
                                   :passenger-count="store.passengers.length"
                                   :stops="store.selectedFlight.stops"
                                   :total="store.totalPrice.toFixed(2)"
                                   :loading="store.isLoading"
                                   :valid="isFormValid"
                                   :return-flight="store.returnFlight"
                                   :is-round-trip="store.isRoundTrip"
                                   :checked-price="store.selectedFlight?.checkedPrice || 0"
                                   :carry-on-price="store.selectedFlight?.carryOnPrice || 0"
                                   :checked-bag-multiplier="store.selectedFlight?.checkedBagMultiplier || 1"
                                   :return-checked-price="store.returnFlight?.checkedPrice || 0"
                                   :return-carry-on-price="store.returnFlight?.carryOnPrice || 0"
                                   :return-checked-bag-multiplier="store.returnFlight?.checkedBagMultiplier || 1"
                                   :passengers="store.passengers"
                                   :outbound-flight-total="store.outboundFlightTotal"
                                   :return-flight-total="store.returnFlightTotal"
                                   :baggage-total="store.baggageTotal"
                                   @purchase="handlePurchase" />
                </div>
            </template>
        </main>

        <SuccessModal v-model="showSuccessModal" />

        <ErrorModal
            v-model="showErrorModal"
            :message="store.error?.message || 'Error al procesar la compra'"
            :errors="store.error?.validationErrors"
        />
    </div>
</template>