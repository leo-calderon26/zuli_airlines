import { defineStore } from "pinia";
import { ref, computed, watch } from "vue";
import { purchaseTickets, getFlightDetails } from "../service/ticketService";

export const useTicketStore = defineStore("ticket", () => {
    const selectedFlight = ref(null);
    const passengers = ref([]);
    const buyerFirstName = ref("");
    const buyerFirstLastName = ref("");
    const buyerSecondLastName = ref("");
    const buyerBirthDate = ref("");
    const contactEmail = ref("");
    const contactPhone = ref("");
    const paymentMethod = ref("card");
    const isRoundTrip = ref(false);
    const returnFlight = ref(null);
    const selectedClass = ref("Ecomica");
    const seatsCount = ref(1);
    const flightRouteId = ref(null);
    const returnFlightRouteId = ref(null);

    const purchaseResult = ref(null);
    const isLoading = ref(false);
    const error = ref(null);

    const totalPrice = ref(0);
    const outboundFlightTotal = ref(0);
    const returnFlightTotal = ref(0);
    const baggageTotal = ref(0);

    function getClassPrice(flight) {
        if (!flight) return 0;
        const isFirst = selectedClass.value === "Primera Clase";
        return isFirst
            ? (flight.totalFirstClassPrice || flight.firstClassPrice || flight.FirstClassPrice || 0)
            : (flight.totalTouristPrice || flight.touristPrice || flight.TouristPrice || 0);
    }

    function recalculateTotal() {
        const flight = selectedFlight.value;
        if (!flight) {
            totalPrice.value = 0;
            outboundFlightTotal.value = 0;
            returnFlightTotal.value = 0;
            baggageTotal.value = 0;
            return;
        }
        const priceOut = getClassPrice(flight);
        outboundFlightTotal.value = priceOut * passengers.value.length;

        let retTotal = 0;
        if (isRoundTrip.value && returnFlight.value) {
            const priceRet = getClassPrice(returnFlight.value);
            retTotal = priceRet * passengers.value.length;
        }
        returnFlightTotal.value = retTotal;

        const outCheckedPrice = flight.checkedPrice || flight.CheckedPrice || 0;
        const outCarryOnPrice = flight.carryOnPrice || flight.CarryOnPrice || 0;
        const outMultiplier = flight.checkedBagMultiplier || 1.0;

        let baggage = 0;
        for (const p of passengers.value) {
            const checkedBags = p.checkedBaggage || 0;
            for (let i = 1; i <= checkedBags; i++) {
                baggage += (outCheckedPrice || 0) * Math.pow(outMultiplier, i);
            }
            baggage += (p.carryOn || 0) * outCarryOnPrice;
        }
        if (isRoundTrip.value && returnFlight.value) {
            const ret = returnFlight.value;
            const retCheckedPrice = ret.checkedPrice || ret.CheckedPrice || 0;
            const retCarryOnPrice = ret.carryOnPrice || ret.CarryOnPrice || 0;
            const retMultiplier = ret.checkedBagMultiplier || 1.0;
            for (const p of passengers.value) {
                const checkedBags = p.checkedBaggage || 0;
                for (let i = 1; i <= checkedBags; i++) {
                    baggage += retCheckedPrice * Math.pow(retMultiplier, i);
                }
                baggage += (p.carryOn || 0) * retCarryOnPrice;
            }
        }
        baggageTotal.value = baggage;

        totalPrice.value = outboundFlightTotal.value + returnFlightTotal.value + baggageTotal.value;
    }
    watch([selectedFlight, returnFlight, selectedClass, passengers, isRoundTrip], recalculateTotal, { immediate: true, deep: true });

    const setFlight = (flight, flightClass) => {
        selectedFlight.value = flight;
        selectedClass.value = flightClass;
        seatsCount.value = flight.availableSeats || 1;
    };

    const setFlightRouteId = (routeId) => {
        const parsed = Number(routeId);
        flightRouteId.value = Number.isFinite(parsed) ? parsed : null;
    };

    const setReturnFlightRouteId = (routeId) => {
        const parsed = Number(routeId);
        returnFlightRouteId.value = Number.isFinite(parsed) ? parsed : null;
    };

    const setPassengers = (count) => {
        while (passengers.value.length < count) {
            passengers.value.push({
                firstName: "",
                firstLastName: "",
                secondLastName: "",
                birthDate: "",
                gender: "",
                passportCountry: "",
                passportDueDate: "",
                checkedBaggage: 0,
                carryOn: 0
            });
        }
        while (passengers.value.length > count) {
            passengers.value.pop();
        }
    };

    const checkAvailability = async () => {
        try {
            const flightId = selectedFlight.value.flightId;
            const details = await getFlightDetails(flightId);
            if (details.availableSeats < passengers.value.length) {
                throw new Error("No hay suficientes asientos disponibles para todos los pasajeros.");
            }
            return true;
        } catch (err) {
            error.value = { message: err.message || "Error al verificar disponibilidad", validationErrors: null };
            return false;
        }
    };

    const purchase = async () => {
        isLoading.value = true;
        error.value = null;
        purchaseResult.value = null;

        try {
            console.log('[ticketStore] building payload');
            const payload = {
                flightId: selectedFlight.value.flightId,
                flightClass: selectedClass.value,
                flightRouteId: flightRouteId.value,
                passengers: passengers.value.map((p) => {
                    const baggageItems = [];
                    const checkedCount = p.checkedBaggage || 0;
                    for (let i = 0; i < checkedCount; i++) {
                        baggageItems.push({
                            weight: 23.0,
                            size: 'Mediano',
                            type: 'Maleta'
                        });
                    }
                    return {
                        firstName: p.firstName,
                        firstLastName: p.firstLastName,
                        secondLastName: p.secondLastName,
                        birthDate: p.birthDate,
                        gender: p.gender,
                        passportCountry: p.passportCountry,
                        passportDueDate: p.passportDueDate,
                        checkedBaggage: Math.min(Math.max(checkedCount, 0), 10),
                        baggageItems,
                        carryOn: p.carryOn
                    };
                }),
                buyer: {
                    firstName: buyerFirstName.value,
                    firstLastName: buyerFirstLastName.value,
                    secondLastName: buyerSecondLastName.value,
                    birthDate: buyerBirthDate.value,
                    email: contactEmail.value,
                    phone: contactPhone.value
                },
                paymentMethod: paymentMethod.value,
                reservationOrigin: 'Web'
            };

            if (isRoundTrip.value && returnFlight.value) {
                payload.returnFlightId = returnFlight.value.flightId ?? returnFlight.value.segments?.[0]?.flightId;
                payload.returnFlightRouteId = returnFlightRouteId.value;
            }

            purchaseResult.value = await purchaseTickets(payload);
        } catch (err) {
            const data = err.response?.data;
            if (data?.detail) {
                error.value = { message: data.detail, validationErrors: null };
            } else if (data?.errors) {
                error.value = { message: 'Errores de validación', validationErrors: data.errors };
            } else {
                error.value = { message: 'Error al procesar la compra', validationErrors: null };
            }
        } finally {
            isLoading.value = false;
        }
    };

    const reset = () => {
        selectedFlight.value = null;
        passengers.value = [];
        buyerFirstName.value = "";
        buyerFirstLastName.value = "";
        buyerSecondLastName.value = "";
        buyerBirthDate.value = "";
        contactEmail.value = "";
        contactPhone.value = "";
        paymentMethod.value = "card";
        isRoundTrip.value = false;
        returnFlight.value = null;
        selectedClass.value = "";
        seatsCount.value = 1;
        flightRouteId.value = null;
        returnFlightRouteId.value = null;
        purchaseResult.value = null;
        isLoading.value = false;
        error.value = null;
    };

    return {
        selectedFlight,
        passengers,
        buyerFirstName,
        buyerFirstLastName,
        buyerSecondLastName,
        buyerBirthDate,
        contactEmail,
        contactPhone,
        paymentMethod,
        isRoundTrip,
        returnFlight,
        selectedClass,
        seatsCount,
        flightRouteId,
        returnFlightRouteId,
        setFlightRouteId,
        setReturnFlightRouteId,
        purchaseResult,
        isLoading,
        error,
        totalPrice,
        outboundFlightTotal,
        returnFlightTotal,
        baggageTotal,
        setFlight,
        setPassengers,
        checkAvailability,
        purchase,
        reset
    };
});
