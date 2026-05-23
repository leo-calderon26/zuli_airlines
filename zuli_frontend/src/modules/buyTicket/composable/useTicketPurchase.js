import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { useTicketStore } from "../store/ticketStore";

export function useTicketPurchase() {
    const router = useRouter();
    const store = useTicketStore();

    const errors = ref({});
    const showSuccessModal = ref(false);
    const showErrorModal = ref(false);

    const isFormValid = computed(() => {
        if (!store.contactName) return false;
        if (!store.contactEmail) return false;
        if (!store.contactPhone) return false;
        for (const p of store.passengers) {
            if (!p.firstName || !p.lastName || !p.birthDate || !p.gender || !p.passportCountry) return false;
        }
        return hasNoDuplicates(store.passengers);
    });

    function hasNoDuplicates(passengers) {
        for (let i = 0; i < passengers.length; i++) {
            for (let j = i + 1; j < passengers.length; j++) {
                const a = passengers[i];
                const b = passengers[j];
                if (
                    a.firstName?.toLowerCase() === b.firstName?.toLowerCase() &&
                    a.lastName?.toLowerCase() === b.lastName?.toLowerCase() &&
                    a.birthDate === b.birthDate &&
                    a.passportCountry?.toLowerCase() === b.passportCountry?.toLowerCase()
                ) {
                    return false;
                }
            }
        }
        return true;
    }

    const validate = () => {
        const newErrors = {};

        if (!store.contactName) newErrors.buyerName = "Nombre requerido";

        if (!store.contactEmail) newErrors.email = "Correo requerido";
        else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(store.contactEmail)) newErrors.email = "Correo inválido";

        if (!store.contactPhone) newErrors.phone = "Teléfono requerido";

        const passengerErrors = [];
        for (let i = 0; i < store.passengers.length; i++) {
            const p = store.passengers[i];
            const pe = {};
            if (!p.firstName) pe.firstName = "Requerido";
            if (!p.lastName) pe.lastName = "Requerido";
            if (!p.birthDate) pe.birthDate = "Requerido";
            if (!p.gender) pe.gender = "Seleccione un género";
            if (!p.passportCountry) pe.passportCountry = "Requerido";
            passengerErrors.push(pe);
        }

        newErrors.passengers = passengerErrors;
        errors.value = newErrors;

        const contactOk = Object.keys(newErrors).filter(k => k !== "passengers").length === 0;
        const passengersOk = passengerErrors.every(pe => Object.keys(pe).length === 0);

        if (contactOk && passengersOk && !hasNoDuplicates(store.passengers)) {
            newErrors._duplicate = true;
            errors.value = newErrors;
            return false;
        }

        return contactOk && passengersOk;
    };

    const submitPurchase = async () => {
        if (!validate()) return;
        await store.purchase();
        if (store.error) {
            showErrorModal.value = true;
        } else {
            showSuccessModal.value = true;
            setTimeout(() => {
                router.push({
                    name: "purchaseConfirmation",
                    params: { code: store.purchaseResult?.confirmationCode }
                });
            }, 1500);
        }
    };

    return {
        errors,
        showSuccessModal,
        showErrorModal,
        isFormValid,
        validate,
        submitPurchase
    };
}
