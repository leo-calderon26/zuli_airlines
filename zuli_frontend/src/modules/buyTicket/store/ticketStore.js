import { defineStore } from "pinia";
import { ref, computed } from "vue";
import { purchaseTickets, getFlightDetails } from "../service/ticketService";

export const useTicketStore = defineStore("ticket", () => {
    const selectedFlight = ref(null);
    const passengers = ref([]);
    const contactName = ref("");
    const contactEmail = ref("");
    const contactPhone = ref("");
    const paymentMethod = ref("card");
    const isRoundTrip = ref(false);
    const returnFlight = ref(null);
    const selectedClass = ref("Turista");
    const seatsCount = ref(1);

    const purchaseResult = ref(null);
    const isLoading = ref(false);
    const error = ref(null);

    const totalPrice = computed(() => {
        const flight = selectedFlight.value;
        if (!flight) return 0;
        const isFirst = selectedClass.value === "Primera Clase";
        const classPrice = isFirst
            ? (flight.firstClassPrice || flight.FirstClassPrice)
            : (flight.touristPrice || flight.TouristPrice);
        const basePrice = classPrice || (flight.totalPrice * (isFirst ? 1.8 : 1)) || 0;
        const flightTotal = basePrice * passengers.value.length;
        const checkedPrice = flight.checkedPrice || flight.CheckedPrice || 0;
        const carryOnPrice = flight.carryOnPrice || flight.CarryOnPrice || 0;
        let checkedQty = 0;
        let carryOnQty = 0;
        for (const p of passengers.value) {
            checkedQty += p.checkedBaggage || 0;
            carryOnQty += p.carryOn || 0;
        }
        return flightTotal + (checkedQty * checkedPrice) + (carryOnQty * carryOnPrice);
    });

    const setFlight = (flight, flightClass) => {
        selectedFlight.value = flight;
        selectedClass.value = flightClass;
        seatsCount.value = flight.availableSeats || 1;
    };

    const setPassengers = (count) => {
        while (passengers.value.length < count) {
            passengers.value.push({
                firstName: "",
                lastName: "",
                birthDate: "",
                gender: "",
                passportCountry: "",
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
            const flightId = selectedFlight.value.flightId || selectedFlight.value.pathIds;
            const details = await getFlightDetails(flightId);
            if (details.availableSeats < passengers.value.length) {
                throw new Error("No hay suficientes asientos disponibles para todos los pasajeros.");
            }
            return true;
        } catch (err) {
            error.value = err.message || "Error al verificar disponibilidad";
            return false;
        }
    };

    const purchase = async () => {
        isLoading.value = true;
        error.value = null;
        purchaseResult.value = null;

        const available = await checkAvailability();
        if (!available) {
            isLoading.value = false;
            return;
        }

        try {
            const payload = {
                flightId: selectedFlight.value.flightId || selectedFlight.value.pathIds,
                flightClass: selectedClass.value,
                passengers: passengers.value.map((p) => ({
                    firstName: p.firstName,
                    lastName: p.lastName,
                    birthDate: p.birthDate,
                    gender: p.gender,
                    passportCountry: p.passportCountry,
                    checkedBaggage: p.checkedBaggage,
                    carryOn: p.carryOn
                })),
                contactName: contactName.value,
                contactEmail: contactEmail.value,
                contactPhone: contactPhone.value,
                paymentMethod: paymentMethod.value
            };

            if (isRoundTrip.value && returnFlight.value) {
                payload.returnFlightId = returnFlight.value.flightId || returnFlight.value.pathIds;
            }

            purchaseResult.value = await purchaseTickets(payload);
        } catch (err) {
            error.value = err.response?.data?.detail || "Error al procesar la compra";
        } finally {
            isLoading.value = false;
        }
    };

    const reset = () => {
        selectedFlight.value = null;
        passengers.value = [];
        contactName.value = "";
        contactEmail.value = "";
        contactPhone.value = "";
        paymentMethod.value = "card";
        isRoundTrip.value = false;
        returnFlight.value = null;
        selectedClass.value = "Turista";
        seatsCount.value = 1;
        purchaseResult.value = null;
        isLoading.value = false;
        error.value = null;
    };

    return {
        selectedFlight,
        passengers,
        contactName,
        contactEmail,
        contactPhone,
        paymentMethod,
        isRoundTrip,
        returnFlight,
        selectedClass,
        seatsCount,
        purchaseResult,
        isLoading,
        error,
        totalPrice,
        setFlight,
        setPassengers,
        checkAvailability,
        purchase,
        reset
    };
});
