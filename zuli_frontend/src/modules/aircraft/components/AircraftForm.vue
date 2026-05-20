<script setup>
import { reactive, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAircraft } from '../composable/useAircraft';
import ErrorModal from '../../../shared/ErrorModal.vue';
import SuccessModal from '../../../shared/SuccessModal.vue';
import AppButton from '../../../shared/AppButton.vue';
import AppInput from '../../../shared/AppInput.vue';
import { useForm } from '../../../shared/useForm.js';
import authService from "../../auth/services/authService";

const router = useRouter();
const { addAircraft } = useAircraft();
const { showSuccessModal, successMessage, showErrorModal, errorMessage, isLoading, errors, clearErrors, onSuccess, handleSubmit } = useForm();

const form = reactive({
    model: '',
    weight: '',
    numberEconomyClassRows: 0,
    numberSeatingRowsEconomy: 0,
    numberFirstClassRows: 0,
    numberSeatingRowsFirst: 0,
});

const modelPattern = /^[A-Za-z0-9-]{1,15}$/;

const totalSeats = computed(() => {
    const econ = Number(form.numberEconomyClassRows) * Number(form.numberSeatingRowsEconomy);
    const first = Number(form.numberFirstClassRows) * Number(form.numberSeatingRowsFirst);
    return Number.isFinite(econ + first) ? econ + first : 0;
});

function isIntegerLike(value) {
    return Number.isInteger(Number(value)) && String(value) !== '';
}

function validate() {
    clearErrors();

    if (!form.model || !modelPattern.test(form.model)) {
        errors.fields.model = 'Modelo inválido (máx 15 letras/números o guion)';
    }
    if (form.weight === '' || Number(form.weight) <= 0 || isNaN(Number(form.weight))) {
        errors.fields.weight = 'Peso inválido';
    }
    const countFields = ['numberEconomyClassRows', 'numberSeatingRowsEconomy', 'numberFirstClassRows', 'numberSeatingRowsFirst'];
    for (const key of countFields) {
        const val = form[key];
        if (val === '' || val === null || isNaN(Number(val)) || Number(val) < 0 || !isIntegerLike(val)) {
            errors.fields[key] = 'Debe ingresar valores numéricos válidos mayores o iguales a cero';
        }
    }
    if (totalSeats.value >= 1000) {
        errors.fields.totalSeats = 'El número total de asientos debe ser menor a 1000';
    }
    return Object.keys(errors.fields).length === 0;
}

async function submit() {
    if (!validate()) return;

    await handleSubmit(async () => {
        const baggageCapacity = Number((Number(form.weight) * 0.3).toFixed(4));

        const data = await authService.me();

        const aircraft = {
            model: form.model,
            capacity: totalSeats.value,
            weight: Number(form.weight),
            numberEconomyClassRows: Number(form.numberEconomyClassRows),
            numberSeatingRowsEconomy: Number(form.numberSeatingRowsEconomy),
            numberFirstClassRows: Number(form.numberFirstClassRows),
            numberSeatingRowsFirst: Number(form.numberSeatingRowsFirst),
            baggageCapacity,
            businessId: data.businessId,
        };

        await addAircraft(aircraft);
        onSuccess('La aeronave se ha creado correctamente');
    }, 'Error al crear la aeronave');
}

function onSuccessClose() {
    router.push({ name: 'aircraftList' });
}
</script>

<template>
    <form class="form-card" @submit.prevent="submit">
        <div class="grid grid-cols-1 gap-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <AppInput v-model="form.model" label="Modelo" :error="errors.fields.model" />
                <AppInput :model-value="totalSeats" label="Capacidad (Calculada)" disabled />
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <AppInput v-model="form.weight" label="Peso soportado por la aeronave (kg)" type="number" min="1" step="1" :error="errors.fields.weight" />
                <div></div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <AppInput v-model="form.numberEconomyClassRows" label="Filas clase económica" type="number" min="0" step="1" :error="errors.fields.numberEconomyClassRows" />
                <AppInput v-model="form.numberSeatingRowsEconomy" label="Asientos por fila económica" type="number" min="0" step="1" :error="errors.fields.numberSeatingRowsEconomy" />
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <AppInput v-model="form.numberFirstClassRows" label="Filas primera clase" type="number" min="0" step="1" :error="errors.fields.numberFirstClassRows" />
                <AppInput v-model="form.numberSeatingRowsFirst" label="Asientos por fila primera clase" type="number" min="0" step="1" :error="errors.fields.numberSeatingRowsFirst" />
            </div>
        </div>

        <div v-if="errors.fields.totalSeats" class="mt-2 text-sm text-error">{{ errors.fields.totalSeats }}</div>

        <div class="mt-4">
            <AppButton type="submit" variant="primary" :loading="isLoading">Guardar aeronave</AppButton>
        </div>
    </form>

    <SuccessModal v-model="showSuccessModal" :message="successMessage" @close="onSuccessClose" />
    <ErrorModal v-model="showErrorModal" :message="errorMessage" />
</template>

<style scoped>
@reference "../../../style.css";

.form-card {
    @apply w-full max-w-none rounded-lg border border-gray-200 bg-white p-8 shadow-sm;
}

.form-grid {
    @apply grid grid-cols-1 gap-6 md:grid-cols-2;
}
</style>
