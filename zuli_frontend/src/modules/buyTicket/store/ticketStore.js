import { defineStore } from "pinia";
import { ref, watch } from "vue";
import { purchaseTickets } from "../service/ticketService";
import { checkFlightAvailability } from "../../landing/service/flightSearchService";


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
    const maxPassengers = ref(1);
    const flightRoutes = ref([]);

    const purchaseResult = ref(null);
    const isLoading = ref(false);
    const error = ref(null);

    const totalPrice = ref(0);
    const outboundFlightTotal = ref(0);
    const returnFlightTotal = ref(0);
    const baggageTotal = ref(0);

    const maxCheckedBaggagePerPassenger = 5;

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

        function calculateBaggageForFlight(flightOption) {
            if (!flightOption) return 0;

            const segments = flightOption.segments || [];
            let flightBaggage = 0;

            for (const passenger of passengers.value) {
                const checkedBags = passenger.checkedBaggage || 0;
                const carryOns = passenger.carryOn || 0;

                if (segments.length === 0) {
                    const checkedPrice = flightOption.checkedPrice || flightOption.CheckedPrice || 0;
                    const carryOnPrice = flightOption.carryOnPrice || flightOption.CarryOnPrice || 0;
                    const multiplier = flightOption.checkedBagMultiplier || 1.0;

                    for (let i = 1; i <= checkedBags; i++) {
                        flightBaggage += checkedPrice * Math.pow(multiplier, i - 1);
                    }

                    flightBaggage += carryOns * carryOnPrice;
                } else {
                    for (const segment of segments) {
                        const checkedPrice = segment.checkedPrice || segment.CheckedPrice || 0;
                        const carryOnPrice = segment.carryOnPrice || segment.CarryOnPrice || 0;
                        const multiplier = segment.checkedBagMultiplier || 1.0;

                        for (let i = 1; i <= checkedBags; i++) {
                            flightBaggage += checkedPrice * Math.pow(multiplier, i - 1);
                        }

                        flightBaggage += carryOns * carryOnPrice;
                    }
                }
            }

            return flightBaggage;
        }

        let baggage = calculateBaggageForFlight(flight);

        if (isRoundTrip.value && returnFlight.value) {
            baggage += calculateBaggageForFlight(returnFlight.value);
        }

        baggageTotal.value = baggage;
        totalPrice.value = outboundFlightTotal.value + returnFlightTotal.value + baggageTotal.value;
    }

    watch(
        [selectedFlight, returnFlight, selectedClass, passengers, isRoundTrip],
        recalculateTotal,
        {
            immediate: true,
            deep: true
        }
    );

    function getAvailableSeats(flight) {
        return flight?.availableSeats
            ?? flight?.AvailableSeats
            ?? flight?.seatsAvailable
            ?? flight?.SeatsAvailable
            ?? 1;
    }

    function mapFlightRouteSegment(segment) {
        return {
            flightRouteId: segment.flightRouteId ?? segment.flightId,
            departureDate: segment.departureDate ?? segment.departureDateText
        };
    }

    function mapFlightSegments(flight) {
        return (flight?.segments || []).map(mapFlightRouteSegment);
    }

    const setFlight = (flight, flightClass) => {
        selectedFlight.value = flight;
        selectedClass.value = flightClass;
        seatsCount.value = getAvailableSeats(flight);
    };

    const setFlightRoutes = (routeData) => {
        if (!routeData) {
            flightRoutes.value = [];
            return;
        }

        if (Array.isArray(routeData)) {
            flightRoutes.value = routeData.map(mapFlightRouteSegment);
            return;
        }

        if (routeData.flightRouteSegments) {
            flightRoutes.value = routeData.flightRouteSegments.map(mapFlightRouteSegment);
            return;
        }

        if (routeData.segments) {
            flightRoutes.value = mapFlightSegments(routeData);
            return;
        }

        flightRoutes.value = [];
    };

    const addFlightRoutes = (routeData) => {
        if (!routeData) {
            return;
        }

        if (Array.isArray(routeData)) {
            flightRoutes.value.push(...routeData.map(mapFlightRouteSegment));
            return;
        }

        if (routeData.flightRouteSegments) {
            flightRoutes.value.push(...routeData.flightRouteSegments.map(mapFlightRouteSegment));
            return;
        }

        if (routeData.segments) {
            flightRoutes.value.push(...mapFlightSegments(routeData));
        }
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
        if (flightRoutes.value.length === 0) {
            error.value = {
                message: "No se encontraron rutas de vuelo para verificar disponibilidad.",
                validationErrors: null
            };

            return false;
        }

        try {
            await checkFlightAvailability({
                seats: passengers.value.length,
                segments: flightRoutes.value
            });

            return true;
        } catch (err) {
            error.value = {
                message: err.response?.data?.detail
                    || err.message
                    || "Error al verificar disponibilidad",
                validationErrors: null
            };

            return false;
        }
    };
    function getValidationMessages(errors) {
        if (!errors) {
            return [];
        }

        return Object.values(errors)
            .flatMap((error) => Array.isArray(error) ? error : [error])
            .filter(Boolean);
    }

    function getFirstValidationMessage(errors) {
        const messages = getValidationMessages(errors);
        return messages[0] || "Errores de validación";
    }

    const purchase = async () => {
        isLoading.value = true;
        error.value = null;
        purchaseResult.value = null;

        const hasAvailability = await checkAvailability();

        if (!hasAvailability) {
            isLoading.value = false;
            return;
        }

        try {
            const payload = {
                flightClass: selectedClass.value,
                flightRoutes: flightRoutes.value.map((flightRoute) => ({
                    flightRouteId: flightRoute.flightRouteId,
                    departureDate: flightRoute.departureDate
                })),
                passengers: passengers.value.map((passenger) => {const baggageItems = [];
                    const checkedCount = Math.min(
                        Math.max(Number(passenger.checkedBaggage) || 0, 0),
                        maxCheckedBaggagePerPassenger
                    );

                    for (let i = 0; i < checkedCount; i++) {
                        baggageItems.push({
                            weight: 23.0,
                            size: "Mediano",
                            type: "Maleta"
                        });
                    }

                    return {
                        firstName: passenger.firstName,
                        firstLastName: passenger.firstLastName,
                        secondLastName: passenger.secondLastName,
                        birthDate: passenger.birthDate,
                        gender: passenger.gender,
                        passportCountry: passenger.passportCountry,
                        passportDueDate: passenger.passportDueDate,
                        checkedBaggage: checkedCount,
                        baggageItems,
                        carryOn: Math.min(
                            Math.max(Number(passenger.carryOn) || 0, 0),
                            1
                        )
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
                    reservationOrigin: "Web"
                };

            if (isRoundTrip.value && returnFlight.value) {
                payload.returnFlightId = returnFlight.value.flightId ?? returnFlight.value.segments?.[0]?.flightId;
            }

            purchaseResult.value = await purchaseTickets(payload);
        } catch (err) {
            const data = err.response?.data;

            if (data?.errors) {
                error.value = {
                    message: getFirstValidationMessage(data.errors),
                    validationErrors: data.errors
                };
            } else if (data?.detail) {
                error.value = {
                    message: data.detail,
                    validationErrors: null
                };
            } else {
                error.value = {
                    message: "Error al procesar la compra",
                    validationErrors: null
                };
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
        maxPassengers.value = 1;
        flightRoutes.value = [];
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
        maxPassengers,
        flightRoutes,
        purchaseResult,
        isLoading,
        error,
        totalPrice,
        outboundFlightTotal,
        returnFlightTotal,
        baggageTotal,
        setFlight,
        setFlightRoutes,
        addFlightRoutes,
        setPassengers,
        checkAvailability,
        purchase,
        reset
    };
});