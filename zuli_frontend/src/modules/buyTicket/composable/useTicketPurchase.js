import { ref, computed } from "vue";
import { useTicketStore } from "../store/ticketStore";

export function useTicketPurchase() {
    const store = useTicketStore();

    const wasSubmitted = ref(false);
    const showSuccessModal = ref(false);
    const showErrorModal = ref(false);

    function containsNumbers(value) {
        return /\d/.test(value);
    }

    function isExpired(dateString) {
        if (!dateString) return true;

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const date = new Date(dateString);
        date.setHours(0, 0, 0, 0);

        return date < today;
    }

    function isFutureDate(dateString) {
        if (!dateString) return false;

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const date = new Date(dateString);
        date.setHours(0, 0, 0, 0);

        return date > today;
    }

    function phoneDigitCount(value) {
        return value.replace(/\D/g, "").length;
    }

    function normalize(value) {
        return (value || "").trim().toLowerCase();
    }

    function buildPassengerKey(passenger) {
        return [
            normalize(passenger.firstName),
            normalize(passenger.firstLastName),
            normalize(passenger.secondLastName),
            passenger.birthDate || "",
            normalize(passenger.passportCountry)
        ].join("|");
    }

    function hasNoDuplicates(passengers) {
        const keys = new Set();

        for (const passenger of passengers) {
            const passengerKey = buildPassengerKey(passenger);

            if (keys.has(passengerKey)) {
                return false;
            }

            keys.add(passengerKey);
        }

        return true;
    }

    const validationErrors = computed(() => {
        const newErrors = {};

        if (!store.buyerFirstName) {
            newErrors.buyerFirstName = "Nombre requerido";
        } else if (containsNumbers(store.buyerFirstName)) {
            newErrors.buyerFirstName = "No se permiten números";
        }

        if (!store.buyerFirstLastName) {
            newErrors.buyerFirstLastName = "Primer apellido requerido";
        } else if (containsNumbers(store.buyerFirstLastName)) {
            newErrors.buyerFirstLastName = "No se permiten números";
        }

        if (!store.buyerSecondLastName) {
            newErrors.buyerSecondLastName = "Segundo apellido requerido";
        } else if (containsNumbers(store.buyerSecondLastName)) {
            newErrors.buyerSecondLastName = "No se permiten números";
        }

        if (!store.buyerBirthDate) {
            newErrors.buyerBirthDate = "Fecha de nacimiento requerida";
        } else if (isFutureDate(store.buyerBirthDate)) {
            newErrors.buyerBirthDate = "La fecha de nacimiento no puede ser futura";
        }

        if (!store.contactEmail) {
            newErrors.email = "Correo requerido";
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(store.contactEmail)) {
            newErrors.email = "Correo inválido";
        }

        if (!store.contactPhone) {
            newErrors.phone = "Teléfono requerido";
        } else if (phoneDigitCount(store.contactPhone) > 15) {
            newErrors.phone = "Máximo 15 dígitos";
        }

        const passengerErrors = [];

        for (const passenger of store.passengers) {
            const passengerError = {};

            if (!passenger.firstName) {
                passengerError.firstName = "Requerido";
            } else if (containsNumbers(passenger.firstName)) {
                passengerError.firstName = "No se permiten números";
            }

            if (!passenger.firstLastName) {
                passengerError.firstLastName = "Requerido";
            } else if (containsNumbers(passenger.firstLastName)) {
                passengerError.firstLastName = "No se permiten números";
            }

            if (!passenger.secondLastName) {
                passengerError.secondLastName = "Requerido";
            } else if (containsNumbers(passenger.secondLastName)) {
                passengerError.secondLastName = "No se permiten números";
            }

            if (!passenger.birthDate) {
                passengerError.birthDate = "Requerido";
            } else if (isFutureDate(passenger.birthDate)) {
                passengerError.birthDate = "No puede ser futura";
            }

            if (!passenger.gender) {
                passengerError.gender = "Seleccione un género";
            }

            if (!passenger.passportCountry) {
                passengerError.passportCountry = "Requerido";
            }

            if (!passenger.passportDueDate) {
                passengerError.passportDueDate = "Requerido";
            } else if (isExpired(passenger.passportDueDate)) {
                passengerError.passportDueDate = "Pasaporte vencido";
            }

            passengerErrors.push(passengerError);
        }

        newErrors.passengers = passengerErrors;

        const contactOk = Object
            .keys(newErrors)
            .filter((key) => key !== "passengers")
            .length === 0;

        const passengersOk = passengerErrors.every(
            (passengerError) => Object.keys(passengerError).length === 0
        );

        if (contactOk && passengersOk && !hasNoDuplicates(store.passengers)) {
            newErrors._duplicate = true;
        }

        return newErrors;
    });

    const errors = computed(() => {
        if (!wasSubmitted.value) {
            return {
                passengers: []
            };
        }

        return validationErrors.value;
    });

    const isFormValid = computed(() => {
        const currentErrors = validationErrors.value;

        const contactOk = !currentErrors.buyerFirstName
            && !currentErrors.buyerFirstLastName
            && !currentErrors.buyerSecondLastName
            && !currentErrors.buyerBirthDate
            && !currentErrors.email
            && !currentErrors.phone;

        const passengersOk = currentErrors.passengers?.every((passengerError) =>
            !passengerError.firstName
            && !passengerError.firstLastName
            && !passengerError.secondLastName
            && !passengerError.birthDate
            && !passengerError.gender
            && !passengerError.passportCountry
            && !passengerError.passportDueDate
        );

        return contactOk && passengersOk && !currentErrors._duplicate;
    });

    const submitPurchase = async () => {
        wasSubmitted.value = true;

        if (!isFormValid.value) {
            return null;
        }

        await store.purchase();

        if (store.error) {
            showErrorModal.value = true;
            return null;
        }

        showSuccessModal.value = true;
        return store.purchaseResult;
    };

    return {
        errors,
        wasSubmitted,
        showSuccessModal,
        showErrorModal,
        isFormValid,
        submitPurchase
    };
}