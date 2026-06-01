import { ref, computed } from "vue";
import { useTicketStore } from "../store/ticketStore";

export function useTicketPurchase() {
    const store = useTicketStore();

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

    function phoneDigitCount(value) {
        return value.replace(/\D/g, '').length;
    }

    function hasNoDuplicates(passengers) {
        for (let i = 0; i < passengers.length; i++) {
            for (let j = i + 1; j < passengers.length; j++) {
                const a = passengers[i];
                const b = passengers[j];
                if (
                    a.firstName?.toLowerCase() === b.firstName?.toLowerCase() &&
                    a.firstLastName?.toLowerCase() === b.firstLastName?.toLowerCase() &&
                    a.secondLastName?.toLowerCase() === b.secondLastName?.toLowerCase() &&
                    a.birthDate === b.birthDate
                ) {
                    return false;
                }
            }
        }
        return true;
    }

    const errors = computed(() => {
        const newErrors = {};

        if (!store.buyerFirstName) newErrors.buyerFirstName = "Nombre requerido";
        else if (containsNumbers(store.buyerFirstName)) newErrors.buyerFirstName = "No se permiten números";

        if (!store.buyerFirstLastName) newErrors.buyerFirstLastName = "Primer apellido requerido";
        else if (containsNumbers(store.buyerFirstLastName)) newErrors.buyerFirstLastName = "No se permiten números";

        if (!store.buyerSecondLastName) newErrors.buyerSecondLastName = "Segundo apellido requerido";
        else if (containsNumbers(store.buyerSecondLastName)) newErrors.buyerSecondLastName = "No se permiten números";

        if (!store.buyerBirthDate) newErrors.buyerBirthDate = "Fecha de nacimiento requerida";

        if (!store.contactEmail) newErrors.email = "Correo requerido";
        else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(store.contactEmail)) newErrors.email = "Correo inválido";

        if (!store.contactPhone) newErrors.phone = "Teléfono requerido";
        else if (phoneDigitCount(store.contactPhone) > 15) newErrors.phone = "Máximo 15 dígitos";

        const passengerErrors = [];
        for (let i = 0; i < store.passengers.length; i++) {
            const p = store.passengers[i];
            const pe = {};
            if (!p.firstName) pe.firstName = "Requerido";
            else if (containsNumbers(p.firstName)) pe.firstName = "No se permiten números";
            if (!p.firstLastName) pe.firstLastName = "Requerido";
            else if (containsNumbers(p.firstLastName)) pe.firstLastName = "No se permiten números";
            if (!p.secondLastName) pe.secondLastName = "Requerido";
            else if (containsNumbers(p.secondLastName)) pe.secondLastName = "No se permiten números";
            if (!p.birthDate) pe.birthDate = "Requerido";
            if (!p.gender) pe.gender = "Seleccione un género";
            if (!p.passportCountry) pe.passportCountry = "Requerido";
            if (!p.passportDueDate) pe.passportDueDate = "Requerido";
            else if (isExpired(p.passportDueDate)) pe.passportDueDate = "Pasaporte vencido";
            passengerErrors.push(pe);
        }

        newErrors.passengers = passengerErrors;
        const contactOk = Object.keys(newErrors).filter(k => k !== "passengers").length === 0;
        const passengersOk = passengerErrors.every(pe => Object.keys(pe).length === 0);

        if (contactOk && passengersOk && !hasNoDuplicates(store.passengers)) {
            newErrors._duplicate = true;
        }

        return newErrors;
    });

    const isFormValid = computed(() => {
        const contactOk = !errors.value.buyerFirstName && !errors.value.buyerFirstLastName &&
            !errors.value.buyerSecondLastName && !errors.value.buyerBirthDate &&
            !errors.value.email && !errors.value.phone;
        const passengersOk = errors.value.passengers?.every(pe =>
            !pe.firstName && !pe.firstLastName && !pe.secondLastName &&
            !pe.birthDate && !pe.gender && !pe.passportCountry && !pe.passportDueDate
        );
        return contactOk && passengersOk && !errors.value._duplicate;
    });

    const submitPurchase = async () => {
        if (!isFormValid.value) return;
        await store.purchase();
        if (store.error) {
            showErrorModal.value = true;
        } else {
            showSuccessModal.value = true;
        }
    };

    return {
        errors,
        showSuccessModal,
        showErrorModal,
        isFormValid,
        submitPurchase
    };
}
