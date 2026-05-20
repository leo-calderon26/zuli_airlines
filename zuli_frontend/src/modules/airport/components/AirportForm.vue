<script setup>
import { reactive } from 'vue';
import { useRouter } from 'vue-router';
import { useAirport } from '../composable/useAirport';
import ErrorModal from '../../../shared/ErrorModal.vue';
import SuccessModal from '../../../shared/SuccessModal.vue';
import AppButton from '../../../shared/AppButton.vue';
import AppInput from '../../../shared/AppInput.vue';
import { useForm } from '../../../shared/useForm.js';
import authService from "../../auth/services/authService";

const router = useRouter();
const { addAirport } = useAirport();
const { showSuccessModal, successMessage, showErrorModal, errorMessage, isLoading, errors, clearErrors, onSuccess, handleSubmit } = useForm();

const form = reactive({
    airportCode: '',
    name: '',
    country: '',
    city: ''
});

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

        await addAirport(airportPayload);
        onSuccess('El aeropuerto se ha creado correctamente');
    }, 'Error al crear el aeropuerto');

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
        <div class="form-grid">
            <AppInput v-model="form.airportCode" label="Código del Aeropuerto (ej. SJO)" :error="errors.fields.airportCode" maxlength="10" />

            <AppInput v-model="form.name" label="Nombre del Aeropuerto" :error="errors.fields.name" />

            <AppInput v-model="form.country" label="País" :error="errors.fields.country" />

            <AppInput v-model="form.city" label="Ciudad" :error="errors.fields.city" />
        </div>

        <div class="mt-4">
            <AppButton type="submit" variant="primary" :loading="isLoading">Crear</AppButton>
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
