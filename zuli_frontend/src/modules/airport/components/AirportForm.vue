<script setup>
import { reactive, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useAirport } from '../composable/useAirport';
import ErrorModal from '../../../shared/ErrorModal.vue';
import SuccessModal from '../../../shared/SuccessModal.vue';
import AppButton from '../../../shared/AppButton.vue';
import AppInput from '../../../shared/AppInput.vue';
import { useForm } from '../../../shared/useForm.js';
import authService from "../../auth/services/authService";

const props = defineProps({
    airport: {
        type: Object,
        default: null
    },
    isEdit: {
        type: Boolean,
        default: false
    }
});

const router = useRouter();
const { addAirport, updateAirport } = useAirport();
const { showSuccessModal, successMessage, showErrorModal, errorMessage, isLoading, errors, clearErrors, onSuccess, handleSubmit } = useForm();
const isAdministrator = () => (sessionStorage.getItem('userRole') ?? '') === 'Administrator';

const form = reactive({
    airportCode: '',
    name: '',
    country: '',
    city: ''
});

function syncForm(airport) {
    if (!airport) {
        return;
    }

    form.airportCode = airport.airportCode ?? '';
    form.name = airport.name ?? '';
    form.country = airport.country ?? '';
    form.city = airport.city ?? '';
}

function canEditField() {
    if (!props.isEdit) {
        return true;
    }

    return isAdministrator();
}

watch(
    () => props.airport,
    (airport) => {
        syncForm(airport);
    },
    { immediate: true, deep: true }
);

function validate() {
    clearErrors();

    if (!form.airportCode || form.airportCode.trim().length < 3) {
        errors.fields.airportCode = 'El código debe tener al menos 3 caracteres';
    }
    if (!form.name.trim()) {
        errors.fields.name = 'El nombre es obligatorio';
    }
    if (!form.country.trim()) {
        errors.fields.country = 'El país es obligatorio';
    }
    if (!form.city.trim()) {
        errors.fields.city = 'La ciudad es obligatoria';
    }

    return Object.keys(errors.fields).length === 0 && errors.global === '';
}

async function submit() {
    if (!validate()) return;

    await handleSubmit(async () => {
        const data = await authService.me();

        const airportPayload = {
            airportCode: form.airportCode.toUpperCase().trim(),
            name: form.name.trim(),
            country: form.country.trim(),
            city: form.city.trim(),
            businessId: data.businessId,
        };

        if (props.isEdit) {
            await updateAirport(props.airport?.airportCode?.toUpperCase().trim() ?? form.airportCode.toUpperCase().trim(), airportPayload);
            onSuccess('El aeropuerto se ha actualizado correctamente');
        } else {
            await addAirport(airportPayload);
            onSuccess('El aeropuerto se ha creado correctamente');
        }
    }, props.isEdit ? 'Error al actualizar el aeropuerto' : 'Error al crear el aeropuerto');

    if (errors.fields.airportCode) {
        form.airportCode = ''
    }
}

function onSuccessClose() {
    router.push({ path: '/admin/airports' });
}
</script>

<template>
    <form class="form-card" @submit.prevent="submit">
        <p v-if="props.isEdit && !isAdministrator()" class="mb-6 rounded-lg border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900">
            Como operario, puedes ver este aeropuerto pero no editarlo.
        </p>

        <div class="form-grid">
            <AppInput v-model="form.airportCode" label="Código del Aeropuerto (ej. SJO)" :error="errors.fields.airportCode" maxlength="10" :disabled="!canEditField()" />

            <AppInput v-model="form.name" label="Nombre del Aeropuerto" :error="errors.fields.name" :disabled="!canEditField()" />

            <AppInput v-model="form.country" label="País" :error="errors.fields.country" :disabled="!canEditField()" />

            <AppInput v-model="form.city" label="Ciudad" :error="errors.fields.city" :disabled="!canEditField()" />
        </div>

        <div class="mt-4">
            <AppButton type="submit" variant="primary" :loading="isLoading" :disabled="props.isEdit && !isAdministrator()">{{ props.isEdit ? 'Guardar' : 'Crear' }}</AppButton>
        </div>
    </form>

    <SuccessModal v-model="showSuccessModal" :message="successMessage" @close="onSuccessClose" />
    <ErrorModal v-model="showErrorModal" :message="errorMessage" :errors="errors.fields" />
</template>

<style scoped>
@reference "../../../style.css";

.form-card {
    @apply w-full rounded-lg border border-gray-200 bg-white p-8 shadow-sm;
}

.form-grid {
    @apply grid grid-cols-1 gap-6 md:grid-cols-2;
}
</style>
